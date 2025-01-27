using System;
using CommandSystem;

namespace RandomEvents.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]
public class StopEventsCommand : ICommand
{
	public string Command => "stopevents";

	public string Description => "Stops all running Random Events.";

	public string[] Aliases { get; } = Array.Empty<string>();


	public bool SanitizeResponse => false;

	public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
	{
		if (RandomManager.ActiveEvents == null)
		{
			response = "There aren't any running events.";
			return true;
		}
		RandomManager.StopEvents();
		RandomManager.ForcedEvents = null;
		response = "Stopped all active events.";
		return true;
	}
}
