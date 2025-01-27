using System;
using System.Linq;
using CommandSystem;
using RandomEvents.Interfaces;

namespace RandomEvents.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]
public class ListEventsCommand : ICommand
{
	public string Command => "listevents";

	public string Description => "Lists all random events.";

	public string[] Aliases { get; } = Array.Empty<string>();


	public bool SanitizeResponse => false;

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		IRandomEvent[] activeEvents = RandomManager.ActiveEvents;
		string text = $"Active Events ({((activeEvents != null) ? activeEvents.Length : 0)})\n";
		if (RandomManager.ActiveEvents != null)
		{
			IRandomEvent[] activeEvents2 = RandomManager.ActiveEvents;
			foreach (IRandomEvent randomEvent in activeEvents2)
			{
				text = text + "- " + randomEvent.Name + " (" + randomEvent.Description + ")\n";
			}
		}
		text += $"\nAll Events ({RandomManager.AllEvents.Count()})\n";
		foreach (IRandomEvent allEvent in RandomManager.AllEvents)
		{
			text = text + "- " + allEvent.Name + " (" + allEvent.Description + ")\n";
		}
		response = text;
		return true;
	}
}
