using System.Collections.Generic;
using System.Linq;
using EXILED;
using EXILED.Extensions;

namespace GhostSpectator
{
	public static class Extensions
	{
		//These are two commonly used extensions that will make your life considerably easier
		//When sending RaReply's, you need to identify the 'source' of the message with a string followed by '#' at the start of the message, otherwise the message will not be sent
		public static void RaMessage(this CommandSender sender, string message, bool success = true) =>
			sender.RaReply("GhostSpectator#" + message, success, true, string.Empty);

		public static void Broadcast(this ReferenceHub rh, uint time, string message) => rh.GetComponent<Broadcast>().TargetAddElement(rh.scp079PlayerScript.connectionToClient, message, time, false);

        public static void SetGhostMode(this ReferenceHub rh, bool enabled = true)
        {
            switch (enabled)
            {
                case true:
                    //rh.GetComponent<Scp173PlayerScript>()._flash.blinded = true;
                    if (Plugin.GhostGod) rh.SetGodMode(true);

                    if (Plugin.GhostNoclip)
                    {
                        rh.characterClassManager.SetNoclip(true);
                        rh.characterClassManager.CmdSetNoclip(true);
                        rh.characterClassManager.NetworkNoclipEnabled = true;
                    }

                    EventPlugin.TargetGhost.Clear();
                    foreach (var player in Player.GetHubs().Where(hub => !Plugin.GhostList.Contains(hub)))
                    {
                        EventPlugin.TargetGhost.Add(player, Plugin.GhostList.Select(hub => hub.GetPlayerId()).ToList());
                    }
                    //EventPlugin.GhostedIds.Add(rh.GetPlayerId());
                    break;
                default:
                    //rh.GetComponent<Scp173PlayerScript>()._flash.blinded = false;
                    if (Plugin.GhostGod) rh.SetGodMode(false);
                    if (Plugin.GhostNoclip)
                    {
                        rh.characterClassManager.SetNoclip(false);
                        rh.characterClassManager.CmdSetNoclip(false);
                        rh.characterClassManager.NetworkNoclipEnabled = false;
                        rh.characterClassManager.CallCmdSetNoclip(false);
                    }

                    EventPlugin.TargetGhost.Clear();
                    foreach (var player in Player.GetHubs().Where(hub => !Plugin.GhostList.Contains(hub)))
                    {
                        EventPlugin.TargetGhost.Add(player, Plugin.GhostList.Select(hub => hub.GetPlayerId()).ToList());
                    }
                    //EventPlugin.GhostedIds.Remove(rh.GetPlayerId());
                    break;
            }
		}

        public static bool UseGhostItem(this ReferenceHub rh, ItemType item)
        {
            if (!Plugin.GhostList.Contains(rh) || (item != ItemType.Ammo762 && item != ItemType.Ammo556)) return true;
            Plugin.Log.Debug($"{rh.GetNickname()} attempting ghost spectator teleport.");
            List<ReferenceHub> players = Player.GetHubs().Where(p => !Plugin.GhostList.Contains(p) && p.GetRole() != RoleType.None &&
                                                                     p.GetRole() != RoleType.Spectator && p.GetRole() != RoleType.Tutorial).ToList();

            if (players.Count <= 0)
            {

                rh.ClearBroadcasts();
                rh.Broadcast(3, Translation.strings.teleportNone);
                return false;
            }

            if (!Plugin.GhostPos.ContainsKey(rh.GetUserId())) Plugin.GhostPos.Add(rh.GetUserId(), 0);

            switch (item)
            {
                case ItemType.Ammo762:
                    {
                        foreach (var player in players.OrderBy(p => p.GetPlayerId()))
                        {
                            if (player.GetPlayerId() > Plugin.GhostPos[rh.GetUserId()])
                            {
                                rh.SetPosition(player.GetPosition());
                                Plugin.Log.Debug($"Teleporting {rh.GetNickname()} to {player.GetNickname()}.");

                                rh.ClearBroadcasts();
                                rh.Broadcast(3, string.Format(Translation.strings.teleportTo, player.GetNickname()));

                                return false;
                            }
                        }
                        rh.SetPosition(players[0].GetPosition());
                        Plugin.Log.Debug($"Teleporting {rh.GetNickname()} to {players[0].GetNickname()}.");

                        rh.ClearBroadcasts();
                        rh.Broadcast(3, string.Format(Translation.strings.teleportTo, players[0].GetNickname()));

                        break;
                    }
                case ItemType.Ammo556:
                    {
                        foreach (var player in players.OrderByDescending(p => p.GetPlayerId()))
                        {
                            if (player.GetPlayerId() < Plugin.GhostPos[rh.GetUserId()])
                            {
                                rh.SetPosition(player.GetPosition());
                                Plugin.Log.Debug($"Teleporting {rh.GetNickname()} to {player.GetNickname()}.");

                                rh.ClearBroadcasts();
                                rh.Broadcast(3, string.Format(Translation.strings.teleportTo, player.GetNickname()));

                                return false;
                            }
                        }
                        rh.SetPosition(players[players.Count - 1].GetPosition());
                        Plugin.Log.Debug($"Teleporting {rh.GetNickname()} to {players[players.Count - 1].GetNickname()}.");

                        rh.ClearBroadcasts();
                        rh.Broadcast(3, string.Format(Translation.strings.teleportTo, players[0].GetNickname()));

                        break;
                    }
            }

            return false;
        }

        public static void SpawnGhost(this ReferenceHub rh)
        {
            rh.SetRole(Plugin.GhostRole);
            rh.SetGhostMode(true);
        }

        public static void PlayGhostMessage(this ReferenceHub rh)
        {
            rh.Broadcast(12, Plugin.GhostMessage);
        }
    }
}