using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using NorthwoodLib.Pools;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;
using RandomEvents.Interfaces;

namespace RandomEvents;

public class RandomManager
{
	private static readonly List<IRandomEvent> _events = new List<IRandomEvent>();

	private static int _roundsSince = 0;

	private static bool _roundEver;

	private static bool _eventsRegistered;

	public static IRandomEvent[] ActiveEvents;

	public static IRandomEvent[] ForcedEvents;

	public static IEnumerable<IRandomEvent> AllEvents => _events;

	internal static void ReloadEvents()
	{
		if (!_eventsRegistered)
		{
			EventManager.RegisterEvents(RandomPlugin.Plugin, new RandomManager());
			_eventsRegistered = true;
		}
		foreach (IRandomEvent @event in _events)
		{
			try
			{
				if (@event.IsActive)
				{
					@event.Stop();
				}
				if (@event is IDisposable disposable)
				{
					disposable.Dispose();
				}
			}
			catch (Exception arg)
			{
				Log.Error($"Failed to stop event type '{@event.GetType().FullName}':\n{arg}", "Random Events");
			}
		}
		_events.Clear();
		Type[] types = typeof(RandomManager).Assembly.GetTypes();
		foreach (Type type in types)
		{
			try
			{
				if (type != typeof(IRandomEvent) && type != typeof(RandomEvent) && typeof(IRandomEvent).IsAssignableFrom(type))
				{
					IRandomEvent randomEvent = Activator.CreateInstance(type) as IRandomEvent;
					_events.Add(randomEvent);
					Log.Info("Loaded random event '" + randomEvent.Name + "'!", "Random Events");
					if (!RandomPlugin.Config.Chances.ContainsKey(type.Name))
					{
						Log.Warning("Random event '" + type.Name + "' is missing from the chances config, adding ..", "Random Events");
						RandomPlugin.Config.Chances[type.Name] = 0;
						RandomPlugin.Handler.SaveConfig(RandomPlugin.Plugin, "ConfigObject");
					}
				}
			}
			catch (Exception arg2)
			{
				Log.Error($"Failed to initialize type '{type.FullName}':\n{arg2}", "Random Events");
			}
		}
	}

	public static void StopEvents()
	{
		if (ActiveEvents != null)
		{
			IRandomEvent[] activeEvents = ActiveEvents;
			foreach (IRandomEvent randomEvent in activeEvents)
			{
				EventManager.UnregisterEvents(RandomPlugin.Plugin, randomEvent);
				StaticUnityMethods.OnUpdate -= randomEvent.Tick;
				randomEvent.Stop();
				randomEvent.IsActive = false;
				Log.Info("Disabled event '" + randomEvent.Name + "'", "Random Events");
			}
			ActiveEvents = null;
		}
	}

	public static void RunEvents(IRandomEvent[] events)
	{
		_roundEver = true;
		_roundsSince = 0;
		ActiveEvents = events;
		Log.Info($"Chose {events.Length} events.", "Random Events");
		foreach (IRandomEvent randomEvent in events)
		{
			randomEvent.IsActive = true;
			randomEvent.Start();
			EventManager.RegisterEvents(RandomPlugin.Plugin, randomEvent);
			StaticUnityMethods.OnUpdate += randomEvent.Tick;
			Log.Info("Enabled event '" + randomEvent.Name + "'", "Random Events");
		}
		if (Round.IsRoundStarted)
		{
			Timing.RunCoroutine(RunAnnouncements());
		}
	}

	public static bool TryPickEvents(out IRandomEvent[] events)
	{
		if (RandomPlugin.Config.EventCount <= 0)
		{
			events = null;
			return false;
		}
		List<IRandomEvent> list = ListPool<IRandomEvent>.Shared.Rent();
		int num = Utils.Random.Next(0, RandomPlugin.Config.EventCount);
		Log.Info($"Generated count: {num}", "Random Events");
		for (int i = 0; i < num; i++)
		{
			IRandomEvent randomEvent = PickEvent();
			if (randomEvent == null)
			{
				Log.Info("Picked a null event", "Random Events");
				continue;
			}
			list.Add(randomEvent);
			Log.Info("Picked event: " + randomEvent.Name, "Random Events");
		}
		Log.Info($"Picked {list.Count} events.", "Random Events");
		events = list.ToArray();
		ListPool<IRandomEvent>.Shared.Return(list);
		return events.Length != 0;
		static IRandomEvent PickEvent()
		{
			foreach (IRandomEvent @event in _events)
			{
				if (RandomPlugin.Config.Chances.TryGetValue(@event.GetType().Name, out var value))
				{
					@event.Chance = value;
				}
				else
				{
					@event.Chance = -1;
				}
				Log.Info($"Event '{@event.Name}' chance: {@event.Chance}", "Random Events");
			}
			List<IRandomEvent> eventList = _events.Where((IRandomEvent ev) => ev.Chance > 0).ToList();
			int num2 = eventList.Sum((IRandomEvent ev) => ev.Chance);
			Log.Info($"List count: {eventList.Count} (sum: {num2})", "Random Events");
			if (eventList.Count == 1)
			{
				return eventList[0];
			}
			if (eventList.Count < 1)
			{
				return null;
			}
			return eventList[Utils.PickIndex(num2, eventList.Count, (int index) => eventList[index].Chance)];
		}
	}

	[PluginEvent]
	public void OnPlayerJoined(PlayerJoinedEvent ev)
	{
		if (Round.IsRoundStarted && ActiveEvents != null)
		{
			Timing.RunCoroutine(RunAnnouncementsFor(ev.Player));
		}
	}

	[PluginEvent]
	public void OnStarted(RoundStartEvent ev)
	{
		if (ActiveEvents != null)
		{
			Timing.RunCoroutine(RunAnnouncements());
		}
	}

	[PluginEvent]
	public void OnWaiting(WaitingForPlayersEvent _)
	{
		if (ForcedEvents != null)
		{
			RunEvents(ForcedEvents);
			ForcedEvents = null;
		}
		else if (!_roundEver || _roundsSince >= RandomPlugin.Config.RoundCount)
		{
			if (Utils.PickBool(RandomPlugin.Config.EventChance))
			{
				Log.Info("Picking random events ..", "Random Events");
				if (TryPickEvents(out var events))
				{
					RunEvents(events);
				}
			}
		}
		else
		{
			_roundsSince++;
		}
	}

	[PluginEvent]
	public void OnEnding(RoundEndEvent ev)
	{
		StopEvents();
	}

	private static IEnumerator<float> RunAnnouncements()
	{
		IRandomEvent[] activeEvents = ActiveEvents;
		foreach (IRandomEvent randomEvent in activeEvents)
		{
			Map.Broadcast(10, "<b>Vybrán náhodný event <color=#ff0000>[" + randomEvent.Name + "]</color>\n" + randomEvent.Description + "</b>");
			yield return Timing.WaitForSeconds(10f);
		}
	}

	private static IEnumerator<float> RunAnnouncementsFor(Player player)
	{
		IRandomEvent[] activeEvents = ActiveEvents;
		foreach (IRandomEvent randomEvent in activeEvents)
		{
			player.SendBroadcast("<b>Na serveru probíhá náhodný event <color=#ff0000>[" + randomEvent.Name + "]</color>\n" + randomEvent.Description + "</b>", 10);
			yield return Timing.WaitForSeconds(10f);
		}
	}
}
