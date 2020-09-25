using System;
using System.Linq;
using Exiled.API.Features;
using HarmonyLib;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(PlayerInteract))]
    [HarmonyPatch(nameof(PlayerInteract.CallCmdUseLocker))]
    [HarmonyPatch(new Type[]
    {
        typeof(byte),
        typeof(byte)
    })]
    class UseLockerPatch
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(PlayerInteract __instance, byte lockerId, byte chamberNumber)
        {
            Plugin.Log.Debug("UseLockerPatch");
            Player ply = Player.Get(__instance.gameObject);
            if (!Plugin.GhostList.Contains(ply)) return true;

            ply.ClearBroadcasts();
            ply.Broadcast(3, Translation.Translation.GetText().LockerDenied);

            return false;
        }
    }
}
