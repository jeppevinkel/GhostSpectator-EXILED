using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using MEC;
using static GhostSpectator.Plugin;

namespace GhostSpectator
{
	public class EventHandlers
	{
		public void OnConsoleCommand(SendingConsoleCommandEventArgs ev)
        {
	        Plugin.Log.Debug($"{ev.Player.Nickname} used the command: '{ev.Name}'");
	        if (ev.Name.ToLower() == "specmode")
	        {
		        if (!Plugin.GhostSettings.ContainsKey(ev.Player.UserId)) Plugin.GhostSettings.Add(ev.Player.UserId, new GhostSettings());

		        if (RateLimited.Contains(ev.Player))
		        {
					ev.Player.ClearBroadcasts();
					ev.Player.Broadcast(1, Translation.Translation.GetText().RateLimited);
					ev.ReturnMessage = Translation.Translation.GetText().RateLimited;
					ev.Color = "red";
					return;
		        }

		        RateLimited.Add(ev.Player);
		        Timing.CallDelayed(Instance.Config.RateLimitTime, () => RateLimited.Remove(ev.Player));

                switch (Plugin.GhostSettings[ev.Player.UserId].Specmode)
                {
                    case GhostSettings.Specmodes.Normal:
	                    Plugin.GhostSettings[ev.Player.UserId].Specmode = GhostSettings.Specmodes.Ghost;

	                    if (Plugin.GhostSettings.ContainsKey(ev.Player.UserId) && Plugin.GhostSettings[ev.Player.UserId].Specmode == GhostSettings.Specmodes.Ghost && ev.Player.Role == RoleType.Spectator)
	                    {
		                    Plugin.Log.Debug($"{ev.Player.Nickname} added to list of ghost spectators.");
		                    GhostList.Add(ev.Player);
		                    Timing.RunCoroutine(SpawnGhost(ev.Player, 0.1f));
                        }

                        ev.ReturnMessage = Translation.Translation.GetText().SpecmodeGhost;
	                    ev.Color = "blue";
	                    ev.IsAllowed = false;
                        break;
                    case GhostSettings.Specmodes.Ghost:
	                    Plugin.GhostSettings[ev.Player.UserId].Specmode = GhostSettings.Specmodes.Normal;

	                    if (GhostList.Contains(ev.Player))
	                    {
		                    Plugin.Log.Debug($"{ev.Player.Nickname} removed from list of ghost spectators.");
		                    GhostList.Remove(ev.Player);
		                    ev.Player.SetGhostMode(false);
		                    ev.Player.ClearInventory();
		                    ev.Player.Role = RoleType.Spectator;
                        }

                        ev.ReturnMessage = Translation.Translation.GetText().SpecmodeNormal;
	                    ev.Color = "blue";
	                    ev.IsAllowed = false;
						break;
                    default:
	                    Plugin.GhostSettings[ev.Player.UserId].Specmode = GhostSettings.Specmodes.Normal;

	                    if (GhostList.Contains(ev.Player))
	                    {
		                    Plugin.Log.Debug($"{ev.Player.Nickname} removed from list of ghost spectators.");
		                    GhostList.Remove(ev.Player);
		                    ev.Player.SetGhostMode(false);
                            ev.Player.ClearInventory();
                            ev.Player.Role = RoleType.Spectator;
						}

                        ev.ReturnMessage = Translation.Translation.GetText().SpecmodeNormal;
	                    ev.Color = "blue";
	                    ev.IsAllowed = false;
						break;
                }
            }
            else if (ev.Name.ToLower() == "specboard")
	        {
		        if (!Plugin.GhostSettings.ContainsKey(ev.Player.UserId)) Plugin.GhostSettings.Add(ev.Player.UserId, new GhostSettings());

                Plugin.GhostSettings[ev.Player.UserId].Specboard =
			        !Plugin.GhostSettings[ev.Player.UserId].Specboard;

                ev.ReturnMessage = "You have " + (Plugin.GhostSettings[ev.Player.UserId].Specboard ? "enabled" : "disabled") + " specboard mode.";
                ev.Color = "Magenta";
                ev.IsAllowed = false;
			}
        }

		public void OnInteractingDoor(InteractingDoorEventArgs ev)
		{
			switch (ev.Door.name)
			{
				case "096":
					// Do stuff when the 096 door is opened...
					break;
				case "173":
					// Do stuff when the 173 door is opened...
					break;
				case "106_PRIMARY":
				case "106_SECONDARY":
					// Do stuff when the 106 door is opened...
					break;
			}
		}

        public IEnumerator<float> SpawnGhost(Exiled.API.Features.Player ply, float delay = 5)
        {
	        yield return Timing.WaitForSeconds(delay);
	        if (ply.Role == Plugin.GhostRole || ply.Role != RoleType.Spectator) yield break;
	        ply.PlayGhostMessage();
	        ply.SpawnGhost();
	        yield return Timing.WaitForSeconds(1f);

	        Plugin.Log.Debug($"{ply.Nickname} given the ghost spectator items.");
	        ply.ClearInventory();
	        ply.AddItem(ItemType.Ammo762);
	        ply.AddItem(ItemType.Ammo556);
	        ply.AddItem(ItemType.Ammo9mm);
	        ply.AddItem(ItemType.Flashlight);
        }
	}
}