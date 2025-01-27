using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;

namespace RandomEvents.Events;

public class PermanentAccessEvent : RandomEvent
{
	public override string Name => "Permanentní přístup";

	public override string Description => "Všechny brány ve hře jsou permanentně otevřené.";

	[PluginEvent]
	public void OnRoundStart(RoundStartEvent ev)
	{
		foreach (DoorVariant allDoor in DoorVariant.AllDoors)
		{
			if (allDoor is PryableDoor)
			{
				allDoor.NetworkTargetState = true;
				allDoor.ServerChangeLock(DoorLockReason.AdminCommand, newState: true);
			}
		}
	}

	public override void Start()
	{
		base.Start();
		if (!Round.IsRoundStarted)
		{
			return;
		}
		foreach (DoorVariant allDoor in DoorVariant.AllDoors)
		{
			if (allDoor is PryableDoor)
			{
				allDoor.NetworkTargetState = true;
				allDoor.ServerChangeLock(DoorLockReason.AdminCommand, newState: true);
			}
		}
	}

	public override void Stop()
	{
		base.Stop();
		foreach (DoorVariant allDoor in DoorVariant.AllDoors)
		{
			if (allDoor is PryableDoor)
			{
				allDoor.NetworkTargetState = true;
				allDoor.ServerChangeLock(DoorLockReason.AdminCommand, newState: false);
			}
		}
	}
}
