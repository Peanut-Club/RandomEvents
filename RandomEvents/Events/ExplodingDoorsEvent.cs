using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;
using UnityEngine;
using Utils;

namespace RandomEvents.Events;

public class ExplodingDoorsEvent : RandomEvent
{
	public static int ExplosionChance => RandomPlugin.Config.DoorExplosionChance;

	public static int ExplosionDamage => RandomPlugin.Config.DoorExplosionDamage;

	public override string Description => $"Máte šanci {ExplosionChance}% na explozi dveří při úspěšné interakci.";

	public override string Name => "Výbušné dveře";

	[PluginEvent]
	public void OnInteracted(PlayerInteractDoorEvent ev)
	{
		if (ev.CanOpen && ev.Player.IsAlive && !ev.Player.IsSCP && Utils.PickBool(ExplosionChance))
		{
			ExplodeDoor(ev.Door, ev.Player);
		}
	}

	public static void ExplodeDoor(DoorVariant door, Player player)
	{
		if (door is BreakableDoor breakableDoor && !breakableDoor.Network_destroyed)
		{
			ExplosionUtils.ServerSpawnEffect(((Component)(object)breakableDoor).transform.position, ItemType.GrenadeHE);
			breakableDoor.Network_destroyed = true;
			if (ExplosionDamage > 0)
			{
				player.Damage(ExplosionDamage, "Door explosion.");
			}
		}
	}
}
