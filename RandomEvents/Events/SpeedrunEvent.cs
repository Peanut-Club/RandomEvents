using System;
using CustomPlayerEffects;
using MEC;
using PlayerRoles;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;

namespace RandomEvents.Events;

public class SpeedrunEvent : RandomEvent
{
	private DateTime _prevTick = DateTime.MinValue;

	public override string Name => "Speedrun";

	public override string Description => "O 100% zvýšená rychlost!";

	[PluginEvent]
	public void OnPlayerSpawn(PlayerSpawnEvent ev)
	{
		Timing.CallDelayed(0.5f, delegate
		{
			if (ev.Player.ReferenceHub.roleManager.CurrentRole.RoleTypeId != RoleTypeId.Tutorial && ev.Player.IsAlive && ev.Player.Role != 0)
			{
				ev.Player.ReferenceHub.playerEffectsController.GetEffect<MovementBoost>()?.ServerSetState(100);
			}
		});
	}

	public override void Start()
	{
		base.Start();
		foreach (Player player in Player.GetPlayers())
		{
			if (player.IsTutorial || !player.IsAlive || player.Role == RoleTypeId.Scp173)
			{
				break;
			}
			player.ReferenceHub.playerEffectsController.GetEffect<MovementBoost>()?.ServerSetState(100);
		}
	}

	public override void Stop()
	{
		base.Stop();
		foreach (Player player in Player.GetPlayers())
		{
			MovementBoost effect = player.EffectsManager.GetEffect<MovementBoost>();
			if (effect != null && effect.Intensity == 100)
			{
				effect.ServerDisable();
			}
		}
	}

	public override void Tick()
	{
		base.Tick();
		if ((DateTime.Now - _prevTick).TotalMilliseconds < 200.0)
		{
			return;
		}
		_prevTick = DateTime.Now;
		foreach (Player player in Player.GetPlayers())
		{
			if (player.ReferenceHub.roleManager.CurrentRole.RoleTypeId != RoleTypeId.Tutorial && player.IsAlive)
			{
				MovementBoost effect = player.ReferenceHub.playerEffectsController.GetEffect<MovementBoost>();
				if ((object)effect != null && (!effect.IsEnabled || effect.Intensity != 100))
				{
					effect.ServerSetState(100);
				}
			}
		}
	}
}
