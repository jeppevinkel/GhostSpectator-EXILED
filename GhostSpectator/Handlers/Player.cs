using System;
using System.Collections.Generic;
using Exiled.Events.EventArgs;
using MEC;
using UnityEngine;
using static GhostSpectator.Plugin;

namespace GhostSpectator.Handlers
{
	internal class Player
	{
		public void OnDroppingItem(DroppingItemEventArgs ev)
		{
			ev.IsAllowed = ev.Player.UseGhostItem(ev.Item.id);
		}

		internal void OnPickingUpItem(PickingUpItemEventArgs ev)
		{
			ev.IsAllowed = false;
			return;
			if (Plugin.Instance.Config.AllowPickup || !Plugin.GhostList.Contains(ev.Player)) return;
			Plugin.Log.Debug($"Prevented {ev.Player.Nickname} from picking up item as ghost spectator.");
			ev.IsAllowed = false;
		}

		internal void OnHurting(HurtingEventArgs ev)
		{
			if (GhostList.Contains(ev.Target) && Instance.Config.GhostGod)
			{
				ev.Amount = 0;
			}

			if (!GhostList.Contains(ev.Attacker) || Instance.Config.AllowDamage) return;
			Log.Debug($"Prevented {ev.Attacker.Nickname} from doing harm as ghost spectator.");
			ev.Amount = 0;
		}

		internal void OnDied(DiedEventArgs ev)
		{
			if (!Plugin.GhostSettings.ContainsKey(ev.Target.UserId) ||
			    Plugin.GhostSettings[ev.Target.UserId].Specmode != GhostSettings.Specmodes.Ghost)
			{
				if (!string.IsNullOrEmpty(Plugin.Instance.Config.SpecMessage)) ev.Target.Broadcast(6, Plugin.Instance.Config.SpecMessage);
				return;
			}

			if (!Plugin.GhostList.Contains(ev.Target))
			{
				Plugin.Log.Debug($"{ev.Target.Nickname} added to list of ghost spectators.");
				Plugin.GhostList.Add(ev.Target);
			}
			Timing.RunCoroutine(SpawnGhost(ev.Target, 0.2f));
		}

		internal void OnChangingRole(ChangingRoleEventArgs ev)
		{
			Plugin.Log.Debug("SetClassEvent");

			if (Plugin.GhostList.Contains(ev.Player))
			{
				Plugin.Log.Debug($"{ev.Player.Nickname} removed from list of ghost spectators.");
				Plugin.GhostList.Remove(ev.Player);
				ev.Player.SetGhostMode(false);
			}

			if (!string.IsNullOrEmpty(ev.Player.UserId) && Plugin.GhostSettings.ContainsKey(ev.Player.UserId) && Plugin.GhostSettings[ev.Player.UserId].Specmode == GhostSettings.Specmodes.Ghost && ev.Player.Role == RoleType.Spectator)
			{
				Plugin.Log.Debug($"{ev.Player.Nickname} added to list of ghost spectators.");
				Plugin.GhostList.Add(ev.Player);
				Timing.RunCoroutine(SpawnGhost(ev.Player, 0.2f));
			}
		}

		internal void OnChangingItem(ChangingItemEventArgs ev)
		{
			if (!ev.Player.UseGhostItem(ev.NewItem.id))
			{
				ev.Player.Inventory.curItem = ItemType.Coin;
			}
		}

		internal void OnJoined(JoinedEventArgs ev)
		{
			ev.Player.ReferenceHub.characterClassManager._enableSyncServerCmdBinding = true;
			ev.Player.ReferenceHub.characterClassManager.CallTargetChangeCmdBinding(ev.Player.Connection, KeyCode.G, ".specmode");
			ev.Player.ReferenceHub.characterClassManager.TargetChangeCmdBinding(ev.Player.Connection, KeyCode.G, ".specmode");
			ev.Player.ReferenceHub.characterClassManager.CallTargetChangeCmdBinding(ev.Player.Connection, KeyCode.B, ".f");
			ev.Player.ReferenceHub.characterClassManager.TargetChangeCmdBinding(ev.Player.Connection, KeyCode.B, ".f");
			ev.Player.ReferenceHub.characterClassManager.SyncServerCmdBinding();
			if (!Plugin.GhostSettings.ContainsKey(ev.Player.UserId))
			{
				Plugin.GhostSettings.Add(ev.Player.UserId, new GhostSettings());
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

		internal void OnInteracted(InteractedEventArgs ev)
		{
			//if (Instance.Config.GhostInteract || !GhostList.Contains(ev.Player)) return;
		}

		public void OnInteractingDoor(InteractingDoorEventArgs ev)
		{
			if (Instance.Config.GhostInteract || !GhostList.Contains(ev.Player)) return;
			ev.IsAllowed = false;
		}

		public void OnInteractingElevator(InteractingElevatorEventArgs ev)
		{
			if (Instance.Config.GhostInteract || !GhostList.Contains(ev.Player)) return;
			ev.IsAllowed = false;
		}

		public void OnInteractingLocker(InteractingLockerEventArgs ev)
		{
			if (Instance.Config.GhostInteract || !GhostList.Contains(ev.Player)) return;
			ev.IsAllowed = false;
		}

		public void OnIntercomSpeaking(IntercomSpeakingEventArgs ev)
		{
			if (Instance.Config.GhostInteract || !GhostList.Contains(ev.Player)) return;
			ev.IsAllowed = false;
		}
	}
}
