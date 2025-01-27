using System;
using System.Collections.Generic;
using System.Linq;
using Interactables.Interobjects.DoorUtils;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Keycards;
using MEC;
using PlayerRoles;
using PluginAPI.Core;

namespace RandomEvents.Events;

public class RandomizerEvent : RandomEvent
{
	private static DateTime _nextRandomizer;

	public static readonly ItemType[] Items = EnumUtils<ItemType>.Values.Where((ItemType it) => it != ItemType.None && InventoryItemLoader.TryGetItem<ItemBase>(it, out var result4) && result4.Category != ItemCategory.Ammo).ToArray();

	public static readonly ItemType[] GateKeycards = EnumUtils<ItemType>.Values.Where((ItemType it) => it != ItemType.None && InventoryItemLoader.TryGetItem<ItemBase>(it, out var result3) && result3 is KeycardItem keycardItem3 && keycardItem3.Permissions.HasFlagFast(KeycardPermissions.ExitGates)).ToArray();

	public static readonly ItemType[] CheckpointKeycards = EnumUtils<ItemType>.Values.Where((ItemType it) => it != ItemType.None && InventoryItemLoader.TryGetItem<ItemBase>(it, out var result2) && result2 is KeycardItem keycardItem2 && keycardItem2.Permissions.HasFlagFast(KeycardPermissions.Checkpoints)).ToArray();

	public static readonly ItemType[] OtherKeycards = EnumUtils<ItemType>.Values.Where((ItemType it) => it != ItemType.None && InventoryItemLoader.TryGetItem<ItemBase>(it, out var result) && result is KeycardItem keycardItem && !keycardItem.Permissions.HasFlagFast(KeycardPermissions.ExitGates) && !keycardItem.Permissions.HasFlagFast(KeycardPermissions.Checkpoints)).ToArray();

	private CoroutineHandle _coroutine;

	public static byte Time => RandomPlugin.Config.RandomizerTime;

	public override string Name => "Randomizer";

	public override string Description => $"Dostanete náhodný inventář každé {Time} minuty.";

	public override void Start()
	{
		base.Start();
		_nextRandomizer = DateTime.Now.AddMinutes((int)Time);
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
			yield return float.NegativeInfinity;
			if (DateTime.Now < _nextRandomizer)
			{
				continue;
			}
			_nextRandomizer = DateTime.Now.AddMinutes((int)Time);
			Broadcast("Čas na náhodný inventář!", 5);
			foreach (Player player in Player.GetPlayers())
			{
				if (!player.IsAlive || player.IsSCP || player.IsTutorial)
				{
					continue;
				}
				player.ClearInventory(clearAmmo: false);
				ItemType[] array = GenerateInventory(player.Team == Team.ChaosInsurgency || player.Role == RoleTypeId.NtfCaptain || player.Role == RoleTypeId.NtfSergeant || player.Role == RoleTypeId.NtfSpecialist || player.Items.Any((ItemBase it) => it is KeycardItem keycardItem && keycardItem.Permissions.HasFlagFast(KeycardPermissions.ExitGates)));
				BroadcastTo(player, $"Dostaneš <color=#ff0000>{array.Length}</color> předmět(ů).", 5);
				ItemType[] array2 = array;
				foreach (ItemType item in array2)
				{
					ItemBase itemBase = player.AddItem(item);
					if (itemBase != null && itemBase is Firearm firearm)
					{
						foreach(var at in firearm.Attachments) {
							if (at is FlashlightAttachment) {
								at.IsEnabled = true;
								break;
							}
						}
					}
				}
			}
		}
	}

	private static ItemType[] GenerateInventory(bool hasGatePermit)
	{
		int num = Utils.Random.Next(1, 7);
		ItemType[] array = new ItemType[num + 1];
		for (int i = 0; i < num; i++)
		{
			array[i] = Items[Utils.PickIndex(((IEnumerable<ItemType>)Items).Sum((Func<ItemType, int>)GetChance), Items.Length, (int index) => GetChance(Items[index]))];
		}
		if (hasGatePermit)
		{
			array[num] = GateKeycards.RandomItem();
		}
		else if (Utils.PickBool(50))
		{
			array[num] = CheckpointKeycards.RandomItem();
		}
		else
		{
			array[num] = OtherKeycards.RandomItem();
		}
		return array;
	}

	private static int GetChance(ItemType item)
	{
		if (!InventoryItemLoader.TryGetItem<ItemBase>(item, out var result))
		{
			return 0;
		}
		if (result.Category == ItemCategory.Grenade)
		{
			return 5;
		}
		if (result.Category == ItemCategory.Medical)
		{
			return 20;
		}
		if (result.Category == ItemCategory.SpecialWeapon)
		{
			return 2;
		}
		if (result.Category == ItemCategory.SCPItem)
		{
			return 5;
		}
		return 50;
	}
}
