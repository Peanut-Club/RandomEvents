using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using RandomEvents.Interfaces;
using Utils.NonAllocLINQ;

namespace RandomEvents.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]
public class ForceEventsCommand : ICommand
{
	public string Command => "forceevent";

	public string Description => "Forces events to start at the start of a round.";

	public string[] Aliases { get; } = Array.Empty<string>();


	public bool SanitizeResponse => false;

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		string[] array = string.Join(" ", arguments).Split(new char[1] { ',' });
		if (array.Length < 1)
		{
			response = "Invalid arguments (forceevent eventName1,eventName2)";
			return false;
		}
		List<IRandomEvent> list = new List<IRandomEvent>();
		List<IRandomEvent> target = RandomManager.AllEvents.ToList();
		string[] array2 = array;
		foreach (string arg in array2)
		{
			if (!target.TryGetFirst((IRandomEvent ev) => ev.Name.ToLower() == arg.ToLower(), out var first))
			{
				response = "Failed to find random event '" + arg + "'";
				return false;
			}
			list.Add(first);
		}
		RandomManager.ForcedEvents = list.ToArray();
		response = "Forced event '" + string.Join(",", list.Select((IRandomEvent ev) => ev.Name)) + "' for the next round.";
		return true;
	}
}
