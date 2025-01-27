using PluginAPI.Core;
using RandomEvents.Interfaces;

namespace RandomEvents;

public class RandomEvent : IRandomEvent
{
	public bool IsActive { get; set; }

	public int Chance { get; set; }

	public virtual string Name { get; }

	public virtual string Description { get; }

	public virtual void Start()
	{
	}

	public virtual void Stop()
	{
	}

	public virtual void Tick()
	{
	}

	public void Broadcast(object message, ushort time)
	{
		Map.Broadcast(time, $"<b><color=#ff0000>[{Name}]</color>\n{message}</b>");
	}

	public void BroadcastTo(Player player, object message, ushort time)
	{
		player.SendBroadcast($"<b><color=#ff0000>[{Name}]</color>\n{message}</b>", time);
	}
}
