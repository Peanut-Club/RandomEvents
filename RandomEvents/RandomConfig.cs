using System;
using System.Collections.Generic;
using System.ComponentModel;
using RandomEvents.Interfaces;

namespace RandomEvents;

public class RandomConfig
{
	[Description("Sets chance of an event actually happening.")]
	public int EventChance { get; set; } = 20;


	[Description("Sets the amount of rounds that needs to pass between each event.")]
	public int RoundCount { get; set; } = 3;


	[Description("Sets the maximum number of events that can be happening at once.")]
	public int EventCount { get; set; } = 2;


	[Description("Sets chances for each event")]
	public Dictionary<string, int> Chances { get; set; } = GetDefaultChances();


	[Description("Sets the chance of a door exploding in the ExplodingDoorsEvent.")]
	public int DoorExplosionChance { get; set; } = 10;


	[Description("Sets the damage dealt after a door explodes to the user that interacted with it.")]
	public int DoorExplosionDamage { get; set; } = 40;


	[Description("Sets the maximum time allowed for lockdown time.")]
	public byte LockdownTimeMax { get; set; } = 5;


	[Description("Sets the minimum time allowed for lockdown time.")]
	public byte LockdownTimeMin { get; set; } = 1;


	[Description("Sets the maximum time allowed for lockdown duration.")]
	public int LockdownDurationMax { get; set; } = 15;


	[Description("Sets the minimum time required for lockdown duration.")]
	public int LockdownDurationMin { get; set; } = 5;


	[Description("Sets the time delay for the Randomizer event.")]
	public byte RandomizerTime { get; set; } = 3;


	[Description("Sets the minimum time required for Items Gone event.")]
	public byte ItemsGoneMinTime { get; set; } = 1;


	[Description("Sets the maximum time allowed for Items Gone event.")]
	public byte ItemsGoneMaxTime { get; set; } = 5;


	private static Dictionary<string, int> GetDefaultChances()
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		Type[] types = typeof(RandomConfig).Assembly.GetTypes();
		foreach (Type type in types)
		{
			if (typeof(IRandomEvent).IsAssignableFrom(type) && !(type == typeof(IRandomEvent)) && !(type == typeof(RandomEvent)))
			{
				dictionary[type.Name] = 0;
			}
		}
		return dictionary;
	}
}
