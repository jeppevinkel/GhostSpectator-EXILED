using System.Collections.Generic;
using System.Linq;
using EXILED;
using Grenades;
using MEC;
using EXILED.Extensions;
using System;

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

        public void OnPickupItem(ref PickupItemEvent ev)
        {
            if (Plugin.AllowPickup || !Plugin.GhostList.Contains(ev.Player)) return;
            Plugin.Log.Debug($"Prevented {ev.Player.GetNickname()} from picking up item as ghost spectator.");
            ev.Allow = false;
        }

        public void OnTeamRespawn(ref TeamRespawnEvent ev)
        {
            if (Plugin.GhostList.Count <= 0) return;
            Plugin.Log.Debug($"Ghost spectators added to respawn list.");
            ev.ToRespawn.AddRange(Plugin.GhostList);
        }

        public void OnPlayerDeath(ref PlayerDeathEvent ev)
        {
            if (!Plugin.GhostList.Contains(ev.Player))
            {
                Plugin.Log.Debug($"{ev.Player.GetNickname()} added to list of ghost spectators.");
                Plugin.GhostList.Add(ev.Player);
            }
            Timing.RunCoroutine(SpawnGhost(ev.Player));
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
            if (!Plugin.GhostList.Contains(ev.Attacker) || Plugin.AllowDamage) return;
            Plugin.Log.Debug($"Prevented {ev.Attacker.GetNickname()} from doing harm as ghost spectator.");
            ev.Amount = 0;
        }

        public void OnDropItem(ref DropItemEvent ev)
        {
            if (!Plugin.GhostList.Contains(ev.Player) || (ev.Item.id != ItemType.Ammo762 && ev.Item.id != ItemType.Ammo556)) return;
            ev.Allow = false;
            Plugin.Log.Debug($"{ev.Player.GetNickname()} attempting ghost spectator teleport.");
            List<ReferenceHub> players = Player.GetHubs().Where(p => !Plugin.GhostList.Contains(p) && p.GetRole() != RoleType.None &&
                                                                     p.GetRole() != RoleType.Spectator && p.GetRole() != RoleType.Tutorial).ToList();

            if (players.Count <= 0) return;

            //Random random = new Random();
            //int _rand = random.Next(0, players.Count);
            if (!Plugin.GhostPos.ContainsKey(ev.Player.GetUserId())) Plugin.GhostPos.Add(ev.Player.GetUserId(), 0);

            switch (ev.Item.id)
            {
                case ItemType.Ammo762:
                {
                    foreach (var player in players.OrderBy(p => p.GetPlayerId()))
                    {
                        if (player.GetPlayerId() > Plugin.GhostPos[ev.Player.GetUserId()])
                        {
                            ev.Player.SetPosition(player.GetPosition());
                            Plugin.Log.Debug($"Teleporting {ev.Player.GetNickname()} to {player.GetNickname()}.");
                            return;
                        }
                    }
                    ev.Player.SetPosition(players[0].GetPosition());
                    Plugin.Log.Debug($"Teleporting {ev.Player.GetNickname()} to {players[0].GetNickname()}.");

                    break;
                }
                case ItemType.Ammo556:
                {
                    foreach (var player in players.OrderByDescending(p => p.GetPlayerId()))
                    {
                        if (player.GetPlayerId() < Plugin.GhostPos[ev.Player.GetUserId()])
                        {
                            ev.Player.SetPosition(player.GetPosition());
                            Plugin.Log.Debug($"Teleporting {ev.Player.GetNickname()} to {player.GetNickname()}.");
                            return;
                        }
                    }
                    ev.Player.SetPosition(players[players.Count - 1].GetPosition());
                    Plugin.Log.Debug($"Teleporting {ev.Player.GetNickname()} to {players[players.Count - 1].GetNickname()}.");

                    break;
                }
            }
        }

        //public void OnScp914KnobChange(ref Scp914KnobChangeEvent ev)
        //{
        //    if (!Plugin.GhostInteract && Plugin.GhostList.Contains(ev.Player))
        //    {
        //        ev.Allow = false;
        //    }
        //}

        //public void OnScp914Activation(ref Scp914ActivationEvent ev)
        //{
        //    if (!Plugin.GhostInteract && Plugin.GhostList.Contains(ev.Player))
        //    {
        //        ev.Allow = false;
        //    }
        //}

        //public void OnWarheadCancel(WarheadCancelEvent ev)
        //{
        //    if (!Plugin.GhostInteract && Plugin.GhostList.Contains(ev.Player))
        //    {
        //        ev.Allow = false;
        //    }
        //}

        //public void OnWarheadKeycardAccess(WarheadKeycardAccessEvent ev)
        //{
        //    if (!Plugin.GhostInteract && Plugin.GhostList.Contains(ev.Player))
        //    {
        //        ev.Allow = false;
        //    }
        //}

        //public void OnDoorInteraction(ref DoorInteractionEvent ev)
        //{
        //    if (!Plugin.GhostInteract && Plugin.GhostList.Contains(ev.Player))
        //    {
        //        ev.Allow = false;
        //    }
        //}

        public IEnumerator<float> SlowUpdate()
        {
            while (true)
            {
                yield return Timing.WaitForSeconds(10);

                foreach (var player in Player.GetHubs())
                {
                    if (player.GetRole() != RoleType.Spectator) continue;
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