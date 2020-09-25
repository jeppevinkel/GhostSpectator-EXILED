using System;
using Exiled.API.Features;
using HarmonyLib;
using UnityEngine;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(PlayerInteract))]
    [HarmonyPatch(nameof(PlayerInteract.CallCmdUseWorkStationTake))]
    [HarmonyPatch(new Type[]
    {
        typeof(GameObject)
    })]
    class UseWorkStationTakePatch
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(PlayerInteract __instance, GameObject station)
        {
            Plugin.Log.Debug("UseWorkStationTakePatch");
            Player ply = Player.Get(__instance.gameObject);
            if (!Plugin.GhostList.Contains(ply)) return true;

            ply.ClearBroadcasts();
            ply.Broadcast(3, Translation.Translation.GetText().WorkstationTakeDenied);

            return false;
        }
    }
}
