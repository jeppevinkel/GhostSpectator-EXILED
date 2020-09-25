using System;
using Exiled.API.Features;
using HarmonyLib;
using UnityEngine;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(PlayerInteract))]
    [HarmonyPatch(nameof(PlayerInteract.CallCmdOpenDoor))]
    [HarmonyPatch(new Type[] { typeof(GameObject) })]
    class OpenDoorPatch
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(PlayerInteract __instance, GameObject doorId)
        {
            Plugin.Log.Debug("OpenDorPatch");
            Player ply = Player.Get(__instance.gameObject);
            if (!Plugin.GhostList.Contains(ply)) return true;

            ply.ClearBroadcasts();
            ply.Broadcast(3, Translation.Translation.GetText().DoorDenied);

            return false;
        }
    }
}
