using System.Collections.Generic;
using System.Linq;
using EXILED;
using Grenades;
using MEC;
using EXILED.Extensions;
using System;
using GhostSpectator.Localization;
using Mirror;
using UnityEngine;

namespace GhostSpectator
{
	public class EventHandlers
	{
		public Plugin Plugin;
		public EventHandlers(Plugin plugin) => this.Plugin = plugin;

        public void OnWaitingForPlayers()
        {
            Plugin.GhostList.Clear();
        }

		public void OnRoundEnd()
		{
            Plugin.GhostList.Clear();
        }

		public void OnPlayerJoin(PlayerJoinEvent ev)
		{
			if (!Plugin.GhostSettings.ContainsKey(ev.Player.GetUserId()))
			{
				Plugin.GhostSettings.Add(ev.Player.GetUserId(), new GhostSettings());
			}
		}

        public void OnPickupItem(ref PickupItemEvent ev)
        {
            if (Plugin.AllowPickup || !Plugin.GhostList.Contains(ev.Player)) return;
            Plugin.Log.Debug($"Prevented {ev.Player.GetNickname()} from picking up item as ghost spectator.");
            ev.Allow = false;
        }

        public void OnTeamRespawn(ref TeamRespawnEvent ev)
        {
            //if (Plugin.GhostList.Count <= 0) return;
            Plugin.Log.Debug($"Ghost spectators added to respawn list.");
            ev.ToRespawn.AddRange(Plugin.GhostList);
        }

        public void OnPlayerDeath(ref PlayerDeathEvent ev)
        {
	        if (!Plugin.GhostSettings.ContainsKey(ev.Player.GetUserId()) ||
	            Plugin.GhostSettings[ev.Player.GetUserId()].specmode != GhostSettings.Specmode.Ghost)
	        {
                if (!string.IsNullOrEmpty(Plugin.SpecMessage)) ev.Player.BroadcastMessage(Plugin.SpecMessage);
                return;
	        }

            if (!Plugin.GhostList.Contains(ev.Player))
            {
                Plugin.Log.Debug($"{ev.Player.GetNickname()} added to list of ghost spectators.");
                Plugin.GhostList.Add(ev.Player);
            }
            Timing.RunCoroutine(SpawnGhost(ev.Player), 3);
        }

        public void OnPlayerSpawn(PlayerSpawnEvent ev)
        {
            if (Plugin.GhostList.Contains(ev.Player) && ev.Role != Plugin.GhostRole)
            {
                Plugin.Log.Debug($"{ev.Player.GetNickname()} removed from list of ghost spectators.");
                Plugin.GhostList.Remove(ev.Player);
                ev.Player.SetGhostMode(false);
            }
        }

        public void OnPlayerHurt(ref PlayerHurtEvent ev)
        {
	        if (Plugin.GhostList.Contains(ev.Player) && Plugin.GhostGod)
	        {
		        ev.Amount = 0;
	        }

            if (!Plugin.GhostList.Contains(ev.Attacker) || Plugin.AllowDamage) return;
            Plugin.Log.Debug($"Prevented {ev.Attacker.GetNickname()} from doing harm as ghost spectator.");
            ev.Amount = 0;
        }

        public void OnItemChanged(ItemChangedEvent ev)
        {
            if (!ev.Player.UseGhostItem(ev.NewItem.id))
            {
                ev.Player.inventory.curItem = ItemType.Coin;
            }
        }

        public void OnDropItem(ref DropItemEvent ev)
        {
            ev.Allow = ev.Player.UseGhostItem(ev.Item.id);
        }

        public void OnConsoleCommand(ConsoleCommandEvent ev)
        {
	        Log.Debug($"{ev.Player.GetNickname()} used the command: '{ev.Command}'");
	        if (ev.Command.ToLower() == "specmode")
	        {
		        if (!Plugin.GhostSettings.ContainsKey(ev.Player.GetUserId())) Plugin.GhostSettings.Add(ev.Player.GetUserId(), new GhostSettings());

                switch (Plugin.GhostSettings[ev.Player.GetUserId()].specmode)
                {
                    case GhostSettings.Specmode.Normal:
	                    Plugin.GhostSettings[ev.Player.GetUserId()].specmode = GhostSettings.Specmode.Ghost;
	                    ev.ReturnMessage = Translation.GetText().specmodeGhost;
	                    ev.Color = "blue";
                        break;
                    case GhostSettings.Specmode.Ghost:
	                    Plugin.GhostSettings[ev.Player.GetUserId()].specmode = GhostSettings.Specmode.Normal;

	                    if (Plugin.GhostList.Contains(ev.Player))
	                    {
		                    Plugin.Log.Debug($"{ev.Player.GetNickname()} removed from list of ghost spectators.");
		                    Plugin.GhostList.Remove(ev.Player);
		                    ev.Player.SetGhostMode(false);
		                    ev.Player.ClearInventory();
                            ev.Player.characterClassManager.SetClassID(RoleType.Spectator);
                        }

                        ev.ReturnMessage = Translation.GetText().specmodeNormal;
	                    ev.Color = "blue";
                        break;
                    default:
	                    Plugin.GhostSettings[ev.Player.GetUserId()].specmode = GhostSettings.Specmode.Normal;

	                    if (Plugin.GhostList.Contains(ev.Player))
	                    {
		                    Plugin.Log.Debug($"{ev.Player.GetNickname()} removed from list of ghost spectators.");
		                    Plugin.GhostList.Remove(ev.Player);
		                    ev.Player.SetGhostMode(false);
                            ev.Player.ClearInventory();
		                    ev.Player.characterClassManager.SetClassID(RoleType.Spectator);
                        }

                        ev.ReturnMessage = Translation.GetText().specmodeNormal;
	                    ev.Color = "blue";
                        break;
                }
            }
            else if (ev.Command.ToLower() == "specboard")
	        {
		        if (!Plugin.GhostSettings.ContainsKey(ev.Player.GetUserId())) Plugin.GhostSettings.Add(ev.Player.GetUserId(), new GhostSettings());

                Plugin.GhostSettings[ev.Player.GetUserId()].specboard =
			        !Plugin.GhostSettings[ev.Player.GetUserId()].specboard;

                ev.ReturnMessage = "You have " + (Plugin.GhostSettings[ev.Player.GetUserId()].specboard ? "enabled" : "disabled") + " specboard mode.";
                ev.Color = "Magenta";
            }
        }

        public IEnumerator<float> SlowUpdate()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(5);

                foreach (var player in Player.GetHubs())
                {
                    if (player.GetRole() != RoleType.Spectator || !Plugin.GhostSettings.ContainsKey(player.GetUserId()) || Plugin.GhostSettings[player.GetUserId()].specmode != GhostSettings.Specmode.Ghost) continue;
                    if (!Plugin.GhostList.Contains(player))
                    {
                        Plugin.Log.Debug($"{player.GetNickname()} added to list of ghost spectators.");
                        Plugin.GhostList.Add(player);
                    }
                    Timing.RunCoroutine(SpawnGhost(player, 0));
                }
            }
        }

        public IEnumerator<float> SpawnGhost(ReferenceHub rh, float delay = 5)
        {
            yield return Timing.WaitForSeconds(delay);
            if (rh.GetRole() == Plugin.GhostRole) yield break;
            rh.PlayGhostMessage();
            rh.SpawnGhost();
            yield return Timing.WaitForSeconds(1f);

            Plugin.Log.Debug($"{rh.GetNickname()} given the ghost spectator items.");
            rh.ClearInventory();
            rh.AddItem(ItemType.Ammo762);
            rh.AddItem(ItemType.Ammo556);
            rh.AddItem(ItemType.Ammo9mm);
        }
    }
}