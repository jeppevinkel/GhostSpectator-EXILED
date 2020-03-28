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
            Plugin.Log.Debug($"Ghost spectators added to respawn list.");
            ev.ToRespawn.AddRange(Plugin.GhostList);
        }

        public void OnPlayerDeath(ref PlayerDeathEvent ev)
        {
	        if (!Plugin.GhostSettings.ContainsKey(ev.Player.GetUserId()) ||
	            Plugin.GhostSettings[ev.Player.GetUserId()].specmode != GhostSettings.Specmode.Ghost)
	        {
                if (!string.IsNullOrEmpty(Plugin.SpecMessage)) ev.Player.Broadcast(6, Plugin.SpecMessage);
                return;
	        }

	        if (!Plugin.GhostList.Contains(ev.Player))
	        {
		        Plugin.Log.Debug($"{ev.Player.GetNickname()} added to list of ghost spectators.");
		        Plugin.GhostList.Add(ev.Player);
	        }
	        Timing.RunCoroutine(SpawnGhost(ev.Player, 0.2f));
        }

        public void OnSetClass(SetClassEvent ev)
        {
	        Plugin.Log.Debug("SetClassEvent");
	        if (Plugin.GhostList.Contains(ev.Player))
	        {
		        Plugin.Log.Debug($"{ev.Player.GetNickname()} removed from list of ghost spectators.");
		        Plugin.GhostList.Remove(ev.Player);
		        ev.Player.SetGhostMode(false);
	        }

	        if (!string.IsNullOrEmpty(ev.Player.GetUserId()) && Plugin.GhostSettings.ContainsKey(ev.Player.GetUserId()) && Plugin.GhostSettings[ev.Player.GetUserId()].specmode == GhostSettings.Specmode.Ghost && ev.Player.GetRole() == RoleType.Spectator)
	        {
		        Plugin.Log.Debug($"{ev.Player.GetNickname()} added to list of ghost spectators.");
		        Plugin.GhostList.Add(ev.Player);
		        Timing.RunCoroutine(SpawnGhost(ev.Player, 0.2f));
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

		internal void OnScp914Upgrade(ref SCP914UpgradeEvent ev)
		{
			foreach (ReferenceHub hub in Plugin.GhostList)
			{
				if (ev.Players.Contains(hub))
				{
					Timing.CallDelayed(0.2f, () =>
					{
						hub.ClearInventory();
						hub.AddItem(ItemType.Ammo762);
						hub.AddItem(ItemType.Ammo556);
						hub.AddItem(ItemType.Ammo9mm);
						hub.AddItem(ItemType.Flashlight);
					});
				}
			}
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

		        if (Plugin.RateLimited.Contains(ev.Player))
		        {
					ev.Player.ClearBroadcasts();
					ev.Player.Broadcast(1, Translation.GetText().rateLimited);
					ev.ReturnMessage = Translation.GetText().rateLimited;
					ev.Color = "red";
					return;
		        }

		        Plugin.RateLimited.Add(ev.Player);
		        Timing.CallDelayed(Plugin.RateLimitTime, () => Plugin.RateLimited.Remove(ev.Player));

                switch (Plugin.GhostSettings[ev.Player.GetUserId()].specmode)
                {
                    case GhostSettings.Specmode.Normal:
	                    Plugin.GhostSettings[ev.Player.GetUserId()].specmode = GhostSettings.Specmode.Ghost;

	                    if (Plugin.GhostSettings.ContainsKey(ev.Player.GetUserId()) && Plugin.GhostSettings[ev.Player.GetUserId()].specmode == GhostSettings.Specmode.Ghost && ev.Player.GetRole() == RoleType.Spectator)
	                    {
		                    Plugin.Log.Debug($"{ev.Player.GetNickname()} added to list of ghost spectators.");
		                    Plugin.GhostList.Add(ev.Player);
		                    Timing.RunCoroutine(SpawnGhost(ev.Player, 0.1f));
                        }

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

        public IEnumerator<float> SpawnGhost(ReferenceHub rh, float delay = 5)
        {
            yield return Timing.WaitForSeconds(delay);
            if (rh.GetRole() == Plugin.GhostRole || rh.GetRole() != RoleType.Spectator) yield break;
            rh.PlayGhostMessage();
            rh.SpawnGhost();
            yield return Timing.WaitForSeconds(1f);

            Plugin.Log.Debug($"{rh.GetNickname()} given the ghost spectator items.");
            rh.ClearInventory();
            rh.AddItem(ItemType.Ammo762);
            rh.AddItem(ItemType.Ammo556);
            rh.AddItem(ItemType.Ammo9mm);
            rh.AddItem(ItemType.Flashlight);
		}
    }
}