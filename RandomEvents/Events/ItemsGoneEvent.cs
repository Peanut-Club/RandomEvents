using System.Collections.Generic;
using System.Linq;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Keycards;
using InventorySystem.Items.Pickups;
using MEC;
using PluginAPI.Core;

namespace RandomEvents.Events;

public class ItemsGoneEvent : RandomEvent
{
	private CoroutineHandle _coroutine;

	public static int Time => Utils.Random.Next(RandomPlugin.Config.ItemsGoneMinTime, RandomPlugin.Config.ItemsGoneMaxTime);

	public override string Name => "Vybavení v tahu";

	public override string Description => "Každých pár minut vám zmizí náhodný počet předmětů z inventáře.";

	public override void Start()
	{
		base.Start();
		_coroutine = Timing.RunCoroutine(Coroutine());
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
			yield return Timing.WaitForSeconds((float)Time * 60f);
			foreach (var hub in ReferenceHub.AllHubs)
			{
				if (!Player.TryGet(hub, out var player) || player.ReferenceHub.Mode != CentralAuth.ClientInstanceMode.ReadyClient ||
					!player.IsAlive || player.IsSCP || player.IsTutorial)
				{
					continue;
				}
				int itemsToLose = Utils.Random.Next(0, 3);
				int itemsRemoved = 0;
				for (int i = 0; i < itemsToLose; i++)
				{
					ItemBase item = player.Items.ToArray().RandomItem();
					if (item is KeycardItem || item is Firearm) {
						continue;
					}

					itemsRemoved++;
					player.ReferenceHub.inventory.ServerRemoveItem(item.ItemSerial, null);
				}

                if (itemsRemoved <= 0) {
                    BroadcastTo(player, "Máš štěstí, tentokrát o nic nepřijdeš.", 5);
                } else {
					BroadcastTo(player, $"Přišel jsi o {itemsRemoved} předmětů!", 5);
				}

            }
		}
	}
}
