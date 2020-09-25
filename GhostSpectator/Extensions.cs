using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets._Scripts.Dissonance;
using Exiled.API.Features;
using MEC;
using Mirror;
using UnityEngine;
using static GhostSpectator.Plugin;

namespace GhostSpectator
{
	public static class Extensions
	{
		//These are two commonly used extensions that will make your life considerably easier
		//When sending RaReply's, you need to identify the 'source' of the message with a string followed by '#' at the start of the message, otherwise the message will not be sent
		public static void RaMessage(this CommandSender sender, string message, bool success = true) =>
			sender.RaReply("GhostSpectator#" + message, success, true, string.Empty);

        public static void SetGhostMode(this Player ply, bool enabled = true)
        {
            switch (enabled)
            {
                case true:
                    if (Instance.Config.GhostGod) ply.IsGodModeEnabled = true;

                    if (Instance.Config.GhostNoclip)
                    {
                        GameCore.Console.singleton.TypeCommand($"/NOCLIP {ply.Id}. ENABLE");
                    }

                    foreach (Player player in Player.List)
                    {
                        player.TargetGhosts.Clear();
                        if (GhostList.Contains(player)) continue;
                        player.TargetGhosts.AddRange(GhostList.Select(p => p.Id));
                    }

                    if (!Plugin.GhostSettings.ContainsKey(ply.UserId)) Plugin.GhostSettings.Add(ply.UserId, new GhostSettings());

                    if (Plugin.GhostSettings[ply.UserId].Specboard)
                    {
                        ply.Scale = new Vector3(0.6f, 0.6f, 0.6f);
                    }
                    else
                    {
	                    ply.Scale = new Vector3(1f, 1f, 1f);
                    }
                    break;
                default:
                    if (Instance.Config.GhostGod) ply.IsGodModeEnabled = false;

                    if (Instance.Config.GhostNoclip)
                    {
                        GameCore.Console.singleton.TypeCommand($"/NOCLIP {ply.Id}. DISABLE");
                    }

                    foreach (Player player in Player.List)
                    {
	                    player.TargetGhosts.Clear();
	                    if (GhostList.Contains(player)) continue;
	                    player.TargetGhosts.AddRange(GhostList.Select(p => p.Id));
                    }

                    ply.Scale = new Vector3(1, 1, 1);
                    SetPlayerScale(ply.GameObject, 1f, 1f, 1f);
                    break;
            }
		}

        public static bool UseGhostItem(this Player ply, ItemType item)
        {
            if (!GhostList.Contains(ply) || (item != ItemType.Ammo762 && item != ItemType.Ammo556 && item != ItemType.Ammo9mm && item != ItemType.Flashlight)) return true;
            Plugin.Log.Debug($"{ply.Nickname} attempting ghost spectator teleport.");
            List<Player> players = Player.List.Where(p => !GhostList.Contains(p) && p.Role != RoleType.None &&
                                                                     p.Role != RoleType.Spectator && p.Role != RoleType.Tutorial).ToList();

            switch (item)
            {
                case ItemType.Ammo762:
                    {
	                    if (players.Count <= 0)
	                    {
		                    ply.ClearBroadcasts();
                            ply.Broadcast(3, Translation.Translation.GetText().TeleportNone);
		                    return false;
	                    }

                        if (!Plugin.GhostSettings.ContainsKey(ply.UserId)) Plugin.GhostSettings.Add(ply.UserId, new GhostSettings());

                        foreach (var player in players.OrderBy(p => p.Id))
                        {
                            if (player.Id > Plugin.GhostSettings[ply.UserId].Pos)
                            {
	                            ply.Position = player.Position;
                                Plugin.GhostSettings[ply.UserId].Pos = player.Id;
                                Plugin.Log.Debug($"Teleporting {ply.Nickname} to {player.Nickname}.");

                                ply.ClearBroadcasts();
                                ply.Broadcast(3, string.Format(Translation.Translation.GetText().TeleportTo, string.IsNullOrEmpty(player.DisplayNickname) ? player.Nickname : player.DisplayNickname));

                                return false;
                            }
                        }
                        ply.Position = players[0].Position;
                        Plugin.GhostSettings[ply.UserId].Pos = players[0].Id;
                        Plugin.Log.Debug($"Teleporting {ply.Nickname} to {players[0].Nickname}.");

                        ply.ClearBroadcasts();
                        ply.Broadcast(3, string.Format(Translation.Translation.GetText().TeleportTo, string.IsNullOrEmpty(players[0].DisplayNickname) ? players[0].Nickname : players[0].DisplayNickname));

                        break;
                    }
                case ItemType.Ammo556:
                {
	                if (players.Count <= 0)
	                {
		                ply.ClearBroadcasts();
		                ply.Broadcast(3, Translation.Translation.GetText().TeleportNone);
		                return false;
	                }

	                if (!Plugin.GhostSettings.ContainsKey(ply.UserId)) Plugin.GhostSettings.Add(ply.UserId, new GhostSettings());

	                foreach (var player in players.OrderByDescending(p => p.Id))
                    {
                        if (player.Id < Plugin.GhostSettings[ply.UserId].Pos)
                        {
                            ply.Position = player.Position;
                            Plugin.GhostSettings[ply.UserId].Pos = player.Id;
                            Plugin.Log.Debug($"Teleporting {ply.Nickname} to {player.Nickname}.");

                            ply.ClearBroadcasts();
                            ply.Broadcast(3, string.Format(Translation.Translation.GetText().TeleportTo, string.IsNullOrEmpty(player.DisplayNickname) ? player.Nickname : player.DisplayNickname));

                            return false;
                        }
                    }
                    ply.Position = players[players.Count - 1].Position;
                    Plugin.GhostSettings[ply.UserId].Pos = players[players.Count - 1].Id;
                        Plugin.Log.Debug($"Teleporting {ply.Nickname} to {players[players.Count - 1].Nickname}.");

                        ply.ClearBroadcasts();
                        ply.Broadcast(3, string.Format(Translation.Translation.GetText().TeleportTo, string.IsNullOrEmpty(players[players.Count - 1].DisplayNickname) ? players[players.Count - 1].Nickname : players[players.Count - 1].DisplayNickname));

                    break;
                }
                case ItemType.Ammo9mm:
                {
	                ply.ClearBroadcasts();
	                ply.Broadcast(3, Translation.Translation.GetText().NotImplementedYet);

                    break;
                }
            }

            return false;
        }

        public static void SpawnGhost(this Player ply)
        {
            if (!GhostsBeingSpawned.Contains(ply))
            {
                GhostsBeingSpawned.Add(ply);
            }
            //rh.SetRole(Plugin.GhostRole);
            Vector3 spawnPos = GameObject.Find("TUT Spawn").transform.position;
            if (spawnPos == null) spawnPos = new Vector3(0, 0, 0);
            ply.SetClassCustomSpawn(GhostRole, spawnPos);
            ply.SetGhostMode(true);
        }

        public static void SetSpectatorVoice(this Player ply)
        {
	        var dissonanceUser = ply.GameObject.GetComponent<DissonanceUserSetup>();
	        dissonanceUser.SpectatorChat = true;
	        dissonanceUser.EnableListening(TriggerType.Role, Assets._Scripts.Dissonance.RoleType.Ghost);
	        dissonanceUser.EnableSpeaking(TriggerType.Role, Assets._Scripts.Dissonance.RoleType.Ghost);
	        dissonanceUser.CallTargetUpdateForTeam(Team.RIP);
	        dissonanceUser.TargetUpdateForTeam(Team.RIP);
        }

        public static void PlayGhostMessage(this Player ply)
        {
            ply.Broadcast(12, Instance.Config.GhostMessage);
        }


        public static void SetPlayerScale(GameObject target, float x, float y, float z)
        {
	        try
	        {
		        NetworkIdentity identity = target.GetComponent<NetworkIdentity>();


		        target.transform.localScale = new Vector3(1 * x, 1 * y, 1 * z);

		        ObjectDestroyMessage destroyMessage = new ObjectDestroyMessage();
		        destroyMessage.netId = identity.netId;


		        foreach (GameObject player in PlayerManager.players)
		        {
			        NetworkConnection playerCon = player.GetComponent<NetworkIdentity>().connectionToClient;

			        if (player != target)
				        playerCon.Send(destroyMessage, 0);

			        object[] parameters = new object[] { identity, playerCon };
			        typeof(NetworkServer).InvokeStaticMethod("SendSpawnMessage", parameters);
		        }
	        }
	        catch (Exception e)
	        {
		        Plugin.Log.Info($"Set Scale error: {e}");
	        }
        }

        public static void SetPlayerScale(GameObject target, float scale)
        {
	        try
	        {
		        NetworkIdentity identity = target.GetComponent<NetworkIdentity>();


		        target.transform.localScale = Vector3.one * scale;

		        ObjectDestroyMessage destroyMessage = new ObjectDestroyMessage();
		        destroyMessage.netId = identity.netId;


		        foreach (GameObject player in PlayerManager.players)
		        {
			        if (player == target)
				        continue;

			        NetworkConnection playerCon = player.GetComponent<NetworkIdentity>().connectionToClient;

			        playerCon.Send(destroyMessage, 0);

			        object[] parameters = new object[] { identity, playerCon };
			        typeof(NetworkServer).InvokeStaticMethod("SendSpawnMessage", parameters);
		        }
	        }
	        catch (Exception e)
	        {
		        Plugin.Log.Info($"Set Scale error: {e}");
	        }
        }

        public static void InvokeStaticMethod(this Type type, string methodName, object[] param)
        {
	        BindingFlags flags = BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic |
	                             BindingFlags.Static | BindingFlags.Public;
	        MethodInfo info = type.GetMethod(methodName, flags);
	        info?.Invoke(null, param);
        }
    }
}