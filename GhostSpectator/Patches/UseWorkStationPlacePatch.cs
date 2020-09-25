using System;
using Exiled.API.Features;
using HarmonyLib;
using UnityEngine;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(PlayerInteract))]
    [HarmonyPatch(nameof(PlayerInteract.CallCmdUseWorkStationPlace))]
    [HarmonyPatch(new Type[]
    {
        typeof(GameObject)
    })]
    class UseWorkStationPlacePatch
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(PlayerInteract __instance, GameObject station)
        {
            Plugin.Log.Debug("UseWorkStationPlacePatch");
            Player ply = Player.Get(__instance.gameObject);
            if (!Plugin.GhostList.Contains(ply)) return true;

            ply.ClearBroadcasts();
            ply.Broadcast(3, Translation.Translation.GetText().WorkstationPlaceDenied);

            return false;
        }
    }
}
