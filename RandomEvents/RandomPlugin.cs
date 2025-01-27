using PluginAPI.Core;
using PluginAPI.Core.Attributes;

namespace RandomEvents;

public class RandomPlugin
{
	public static RandomPlugin Plugin;

	public static RandomConfig Config;

	public static PluginHandler Handler;

	[PluginConfig]
	public RandomConfig ConfigObject;

	[PluginEntryPoint("RandomEvents", "1.0.0", "A plugin for random events during the round.", "marchellc")]
	public void Load()
	{
		Plugin = this;
		Config = ConfigObject;
		Handler = PluginHandler.Get(this);
		RandomManager.ReloadEvents();
	}
}
