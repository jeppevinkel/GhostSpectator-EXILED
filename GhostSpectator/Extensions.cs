using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using Assets._Scripts.Dissonance;
using EXILED;
using EXILED.Extensions;
using GhostSpectator.Localization;
using MEC;
using Mirror;
using RemoteAdmin;
using UnityEngine;

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
                    rh.GetComponent<Scp173PlayerScript>()._flash.blinded = true;
                    if (Plugin.GhostGod) rh.SetGodMode(true);

                    if (Plugin.GhostNoclip)
                    {
                        GameCore.Console.singleton.TypeCommand($"/NOCLIP {rh.GetPlayerId()}. ENABLE");
                    }

                    EventPlugin.TargetGhost.Clear();
                    foreach (var player in Player.GetHubs().Where(hub => !Plugin.GhostList.Contains(hub)))
                    {
                        EventPlugin.TargetGhost.Add(player, Plugin.GhostList.Select(hub => hub.GetPlayerId()).ToList());
                    }

                    if (!Plugin.GhostSettings.ContainsKey(rh.GetUserId())) Plugin.GhostSettings.Add(rh.GetUserId(), new GhostSettings());

                    if (Plugin.GhostSettings[rh.GetUserId()].specboard)
                    {
	                    SetPlayerScale(rh.gameObject, 0.6f, 0.6f, 0.06f);
                    }
                    else
                    {
	                    SetPlayerScale(rh.gameObject, 0.6f, 0.6f, 0.6f);
                    }
                    break;
                default:
                    rh.GetComponent<Scp173PlayerScript>()._flash.blinded = false;
                    if (Plugin.GhostGod) rh.SetGodMode(false);

                    if (Plugin.GhostNoclip)
                    {
                        GameCore.Console.singleton.TypeCommand($"/NOCLIP {rh.GetPlayerId()}. DISABLE");
                    }

                    EventPlugin.TargetGhost.Clear();
                    foreach (var player in Player.GetHubs().Where(hub => !Plugin.GhostList.Contains(hub)))
                    {
                        EventPlugin.TargetGhost.Add(player, Plugin.GhostList.Select(hub => hub.GetPlayerId()).ToList());
                    }

                    SetPlayerScale(rh.gameObject, 1f, 1f, 1f);
                    break;
            }
		}

        public static bool UseGhostItem(this ReferenceHub rh, ItemType item)
        {
            if (!Plugin.GhostList.Contains(rh) || (item != ItemType.Ammo762 && item != ItemType.Ammo556 && item != ItemType.Ammo9mm && item != ItemType.Flashlight)) return true;
            Plugin.Log.Debug($"{rh.GetNickname()} attempting ghost spectator teleport.");
            List<ReferenceHub> players = Player.GetHubs().Where(p => !Plugin.GhostList.Contains(p) && p.GetRole() != RoleType.None &&
                                                                     p.GetRole() != RoleType.Spectator && p.GetRole() != RoleType.Tutorial).ToList();

            switch (item)
            {
                case ItemType.Ammo762:
                    {
	                    if (players.Count <= 0)
	                    {

		                    rh.ClearBroadcasts();
		                    rh.Broadcast(3, Translation.GetText().teleportNone);
		                    return false;
	                    }

                        if (!Plugin.GhostSettings.ContainsKey(rh.GetUserId())) Plugin.GhostSettings.Add(rh.GetUserId(), new GhostSettings());

                        foreach (var player in players.OrderBy(p => p.GetPlayerId()))
                        {
                            if (player.GetPlayerId() > Plugin.GhostSettings[rh.GetUserId()].pos)
                            {
                                rh.SetPosition(player.GetPosition());
                                Plugin.GhostSettings[rh.GetUserId()].pos = player.GetPlayerId();
                                Plugin.Log.Debug($"Teleporting {rh.GetNickname()} to {player.GetNickname()}.");

                                rh.ClearBroadcasts();
                                rh.Broadcast(3, string.Format(Translation.GetText().teleportTo, player.GetNickname()));

                                return false;
                            }
                        }
                        rh.SetPosition(players[0].GetPosition());
                        Plugin.GhostSettings[rh.GetUserId()].pos = players[0].GetPlayerId();
                        Plugin.Log.Debug($"Teleporting {rh.GetNickname()} to {players[0].GetNickname()}.");

                        rh.ClearBroadcasts();
                        rh.Broadcast(3, string.Format(Translation.GetText().teleportTo, players[0].GetNickname()));

                        break;
                    }
                case ItemType.Ammo556:
                {
	                if (players.Count <= 0)
	                {

		                rh.ClearBroadcasts();
		                rh.Broadcast(3, Translation.GetText().teleportNone);
		                return false;
	                }

	                if (!Plugin.GhostSettings.ContainsKey(rh.GetUserId())) Plugin.GhostSettings.Add(rh.GetUserId(), new GhostSettings());

	                foreach (var player in players.OrderByDescending(p => p.GetPlayerId()))
                    {
                        if (player.GetPlayerId() < Plugin.GhostSettings[rh.GetUserId()].pos)
                        {
                            rh.SetPosition(player.GetPosition());
                            Plugin.GhostSettings[rh.GetUserId()].pos = player.GetPlayerId();
                            Plugin.Log.Debug($"Teleporting {rh.GetNickname()} to {player.GetNickname()}.");

                            rh.ClearBroadcasts();
                            rh.Broadcast(3, string.Format(Translation.GetText().teleportTo, player.GetNickname()));

                            return false;
                        }
                    }
                    rh.SetPosition(players[players.Count - 1].GetPosition());
                    Plugin.GhostSettings[rh.GetUserId()].pos = players[players.Count - 1].GetPlayerId();
                        Plugin.Log.Debug($"Teleporting {rh.GetNickname()} to {players[players.Count - 1].GetNickname()}.");

                    rh.ClearBroadcasts();
                    rh.Broadcast(3, string.Format(Translation.GetText().teleportTo, players[players.Count - 1].GetNickname()));

                    break;
                }
                case ItemType.Ammo9mm:
                {
                    rh.ClearBroadcasts();
                    rh.Broadcast(3, Translation.GetText().notImplementedYet);

                    break;
                }
            }

            return false;
        }

        public static void SpawnGhost(this ReferenceHub rh)
        {
            if (!Plugin.GhostsBeingSpawned.Contains(rh))
            {
                Plugin.GhostsBeingSpawned.Add(rh);
            }
            //rh.SetRole(Plugin.GhostRole);
            Vector3 spawnPos = GameObject.Find("TUT Spawn").transform.position;
            if (spawnPos == null) spawnPos = new Vector3(0, 0, 0);
            rh.SetClassCustomSpawn(Plugin.GhostRole, spawnPos);
            rh.SetGhostMode(true);
        }

        public static void SetSpectatorVoice(this ReferenceHub rh)
        {
	        rh.GetComponent<DissonanceUserSetup>().SpectatorChat = true;
	        rh.GetComponent<DissonanceUserSetup>().EnableListening(TriggerType.Role, Assets._Scripts.Dissonance.RoleType.Ghost);
	        rh.GetComponent<DissonanceUserSetup>().EnableSpeaking(TriggerType.Role, Assets._Scripts.Dissonance.RoleType.Ghost);
	        rh.GetComponent<DissonanceUserSetup>().CallTargetUpdateForTeam(Team.RIP);
	        rh.GetComponent<DissonanceUserSetup>().TargetUpdateForTeam(Team.RIP);
        }

        public static void PlayGhostMessage(this ReferenceHub rh)
        {
            rh.Broadcast(12, Plugin.GhostMessage);
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
		        Log.Info($"Set Scale error: {e}");
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
		        Log.Info($"Set Scale error: {e}");
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