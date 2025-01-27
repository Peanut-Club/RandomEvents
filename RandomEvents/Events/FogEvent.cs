using System.Linq;
using CustomPlayerEffects;
using CustomRendering;
using MEC;
using PlayerRoles;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;

namespace RandomEvents.Events;

public class FogEvent : RandomEvent
{
	public static readonly FogType[] AllFogTypes = EnumUtils<FogType>.Values.Where((FogType f) => f != 0 && f != FogType.Inside && f != FogType.Outside && f != FogType.Nuke).ToArray();

	private FogType _curFog;

	private string _curFogName;

	private string _curFogDesc;

	public override string Name => _curFogName ?? GetType().Name;

	public override string Description => _curFogDesc ?? "Vybere náhodný fog control efekt.";

	public override void Start()
	{
		base.Start();
		_curFog = AllFogTypes.RandomItem();
		_curFogName = FogToName(_curFog);
		_curFogDesc = FogToDesc(_curFog);
	}

	public override void Stop()
	{
		base.Stop();
		_curFog = FogType.None;
		_curFogName = null;
		_curFogName = null;
	}

	[PluginEvent]
	public void OnRole(PlayerChangeRoleEvent ev)
	{
		Timing.CallDelayed(0.5f, delegate
		{
			if (ev.Player.ReferenceHub.roleManager.CurrentRole.RoleTypeId != RoleTypeId.Tutorial)
			{
				ev.Player.ReferenceHub.playerEffectsController.GetEffect<FogControl>()?.SetFogType(_curFog);
			}
		});
	}

	public static string FogToName(FogType fogType)
	{
		return fogType switch
		{
			FogType.Decontamination => "Rozsáhlá dekontaminace", 
			FogType.Amnesia => "Krátkozrakost", 
			FogType.Scp244 => "Mlha přichází", 
			_ => null, 
		};
	}

	public static string FogToDesc(FogType fogType)
	{
		return fogType switch
		{
			FogType.Decontamination => "Po celé mapě máte efekt dekontaminace.", 
			FogType.Amnesia => "Po celé mapě máte temno.", 
			FogType.Scp244 => "Po celé mapě máte efekt z SCP-244.", 
			_ => null, 
		};
	}
}
