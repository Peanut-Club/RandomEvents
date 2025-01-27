using System;
using System.Collections.Generic;
using Interactables.Interobjects.DoorUtils;
using MEC;
using PluginAPI.Core;

namespace RandomEvents.Events;

public class LockdownEvent : RandomEvent
{
	private CoroutineHandle _coroutine;

	public static int Time => Utils.Random.Next(RandomPlugin.Config.LockdownTimeMin, RandomPlugin.Config.LockdownTimeMax);

	public static int Duration => Utils.Random.Next(RandomPlugin.Config.LockdownDurationMin, RandomPlugin.Config.LockdownDurationMax);

	public override string Name => "Bezpečnostní uzavírka";

	public override string Description => "Každých pár minut se uzavřou všechny dveře.";

	public override void Start()
	{
		base.Start();
		if (RandomPlugin.Config.LockdownTimeMax >= 1)
		{
			_coroutine = Timing.RunCoroutine(Coroutine());
		}
	}

	public override void Stop()
	{
		base.Stop();
		Timing.KillCoroutines(_coroutine);
	}

	private IEnumerator<float> Coroutine()
	{
		while (base.IsActive)
		{
			int time = Time;
			int duration = Duration;
			Map.Broadcast(10, $"<b><color=#ff0000>[Bezpečnostní uzavírka]</color>\nDveře se zamknou za <color=#ff0000>{time}</color> minut(u/y) na <color=#ff0000>{duration}</color> sekund.</b>");
			yield return Timing.WaitForSeconds((float)time * 60f);
			List<Tuple<DoorVariant, bool>> doors = new List<Tuple<DoorVariant, bool>>();
			foreach (DoorVariant allDoor in DoorVariant.AllDoors) {
                if (DoorLockUtils.GetMode((DoorLockReason)allDoor.ActiveLocks) != DoorLockMode.FullLock) {
					doors.Add(new Tuple<DoorVariant, bool>(allDoor, allDoor.NetworkTargetState));
					allDoor.NetworkTargetState = false;
					allDoor.ServerChangeLock(DoorLockReason.AdminCommand, newState: true);
				}
            }
            for (int i = duration; i > 0; i--)
			{
				Map.Broadcast(1, $"<b><color=#ff0000>[Bezpečnostní uzavírka]</color>\nDveře se odemknou za <color=#ff0000>{i}</color> sekund!</b>");
				yield return Timing.WaitForSeconds(1f);
			}
			foreach (Tuple<DoorVariant, bool> item in doors)
			{
				if (item.Item2)
				{
					item.Item1.NetworkTargetState = item.Item2;
				}
				item.Item1.ServerChangeLock(DoorLockReason.AdminCommand, newState: false);
			}
		}
	}
}
