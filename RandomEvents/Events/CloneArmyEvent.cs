using System;
using System.Linq;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;

namespace RandomEvents.Events;

public class CloneArmyEvent : RandomEvent
{
	public static readonly RoleTypeId[] ScpRoles = { RoleTypeId.Scp049, RoleTypeId.Scp096, RoleTypeId.Scp106, RoleTypeId.Scp173, RoleTypeId.Scp939 };

	public override string Name => "Klonová armáda";

	public override string Description => "Může se spawnout SCP pouze jednoho náhodného typu.";

	[PluginEvent]
	public void OnRoundStart(RoundStartEvent ev)
	{
		Timing.CallDelayed(2f, delegate
		{
			RoleTypeId roleTypeId = ScpRoles[Utils.Random.Next(0, ScpRoles.Length)];
			Broadcast($"Vybráno SCP: <color=#ff0000>{roleTypeId}</color>", 10);
			foreach (Player player in Player.GetPlayers())
			{
				if (player.IsSCP)
				{
					player.SetRole(roleTypeId, RoleChangeReason.RoundStart);
				}
			}
		});
	}
}
