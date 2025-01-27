using System.Collections.Generic;
using System.Linq;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Jailbird;
using MEC;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;

namespace RandomEvents.Events;

public class JailbirdShowdownEvent : RandomEvent
{
	public override string Description => "Všichni se místo zbraní spawnout s jedním Jailbirdem.";

	public override string Name => "Jailbird Showdown";

	[PluginEvent]
	public void OnPlayerSpawned(PlayerSpawnEvent ev)
	{
		Timing.CallDelayed(0.5f, delegate
		{
			if (!ev.Player.IsTutorial && !ev.Player.IsSCP && ev.Player.IsAlive)
			{
				List<ItemBase> list = ev.Player.Items.Where((ItemBase item) => item is Firearm).ToList();
				if (list.Any())
				{
					foreach (ItemBase item in list)
					{
						ev.Player.RemoveItem(item);
					}
					(ev.Player.AddItem(ItemType.Jailbird) as JailbirdItem).TotalChargesPerformed = 0;
				}
			}
		});
	}

	public override void Start()
	{
		base.Start();
		foreach (Player player in Player.GetPlayers())
		{
			if (player.IsTutorial || player.IsSCP || !player.IsAlive)
			{
				break;
			}
			List<ItemBase> list = player.Items.Where((ItemBase item) => item is Firearm).ToList();
			if (list.Any())
			{
				foreach (ItemBase item in list)
				{
					player.RemoveItem(item);
				}
			}
			(player.AddItem(ItemType.Jailbird) as JailbirdItem).TotalChargesPerformed = 0;
		}
	}
}
