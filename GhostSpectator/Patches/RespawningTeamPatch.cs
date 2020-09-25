﻿using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.Events.EventArgs;
using HarmonyLib;
using NorthwoodLib.Pools;
using Respawning;
using Respawning.NamingRules;
using UnityEngine;

namespace GhostSpectator.Patches
{
	[HarmonyPatch(typeof(RespawnManager))]
	[HarmonyPatch(nameof(RespawnManager.Spawn))]
	internal static class RespawningTeamPatch
	{
        private static bool Prefix(RespawnManager __instance)
        {
            try
            {
                if (!RespawnWaveGenerator.SpawnableTeams.TryGetValue(__instance.NextKnownTeam, out SpawnableTeam spawnableTeam) ||
                    __instance.NextKnownTeam == SpawnableTeamType.None)
                {
                    ServerConsole.AddLog("Fatal error. Team '" + __instance.NextKnownTeam + "' is undefined.", ConsoleColor.Red);
                }
                else
                {
                    List<Exiled.API.Features.Player> list = Exiled.API.Features.Player.List
                        .Where(p => (p.IsDead || Plugin.GhostList.Contains(p)) && !p.IsOverwatchEnabled).ToList();

                    if (__instance._prioritySpawn)
                    {
                        list = list.OrderBy(item => item.ReferenceHub.characterClassManager.DeathTime).ToList();
                    }
                    else
                    {
                        list.ShuffleList();
                    }

                    RespawnTickets singleton = RespawnTickets.Singleton;
                    int a = singleton.GetAvailableTickets(__instance.NextKnownTeam);
                    if (a == 0)
                    {
                        a = singleton.DefaultTeamAmount;
                        RespawnTickets.Singleton.GrantTickets(singleton.DefaultTeam, singleton.DefaultTeamAmount, true);
                    }

                    int num = Mathf.Min(a, spawnableTeam.MaxWaveSize);

                    List<ReferenceHub> referenceHubList = ListPool<ReferenceHub>.Shared.Rent();

                    var ev = new RespawningTeamEventArgs(list, num, __instance.NextKnownTeam);
                    Exiled.Events.Handlers.Server.OnRespawningTeam(ev);

                    while (list.Count > num)
                        list.RemoveAt(list.Count - 1);
                    list.ShuffleList();

                    foreach (Exiled.API.Features.Player me in list)
                    {
                        try
                        {
                            RoleType classid =
                                spawnableTeam.ClassQueue[
                                    Mathf.Min(referenceHubList.Count, spawnableTeam.ClassQueue.Length - 1)];
                            me.ReferenceHub.characterClassManager.SetPlayersClass(classid, me.ReferenceHub.gameObject);
                            referenceHubList.Add(me.ReferenceHub);
                            ServerLogs.AddLog(ServerLogs.Modules.ClassChange, "Player " + me.ReferenceHub.LoggedNameFromRefHub() + " respawned as " + classid + ".", ServerLogs.ServerLogType.GameEvent);
                        }
                        catch (Exception ex)
                        {
                            if (me != null)
                            {
                                ServerLogs.AddLog(ServerLogs.Modules.ClassChange, "Player " + me.ReferenceHub.LoggedNameFromRefHub() + " couldn't be spawned. Err msg: " + ex.Message, ServerLogs.ServerLogType.GameEvent);
                            }
                            else
                            {
                                ServerLogs.AddLog(ServerLogs.Modules.ClassChange, "Couldn't spawn a player - target's ReferenceHub is null.", ServerLogs.ServerLogType.GameEvent);
                            }
                        }
                    }

                    if (referenceHubList.Count > 0)
                    {
                        ServerLogs.AddLog(ServerLogs.Modules.ClassChange, $"RespawnManager has successfully spawned {referenceHubList.Count} players as {__instance.NextKnownTeam}!", ServerLogs.ServerLogType.GameEvent);
                        RespawnTickets.Singleton.GrantTickets(__instance.NextKnownTeam, -referenceHubList.Count * spawnableTeam.TicketRespawnCost);
                        if (UnitNamingRules.TryGetNamingRule(__instance.NextKnownTeam, out UnitNamingRule rule))
                        {
                            rule.GenerateNew(__instance.NextKnownTeam, out string regular);
                            foreach (ReferenceHub referenceHub in referenceHubList)
                            {
                                referenceHub.characterClassManager.NetworkCurSpawnableTeamType =
                                    (byte)__instance.NextKnownTeam;
                                referenceHub.characterClassManager.NetworkCurUnitName = regular;
                            }

                            rule.PlayEntranceAnnouncement(regular);
                        }

                        RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.UponRespawn, __instance.NextKnownTeam);
                    }

                    __instance.NextKnownTeam = SpawnableTeamType.None;
                }

                return false;
            }
            catch (Exception e)
            {
                Exiled.API.Features.Log.Error($"Exiled.Events.Patches.Events.Server.RespawningTeam: {e}\n{e.StackTrace}");

                return true;
            }
        }
    }
}
