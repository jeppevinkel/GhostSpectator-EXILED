using System;
using Exiled.API.Features;
using HarmonyLib;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(Intercom))]
    [HarmonyPatch(nameof(Intercom.CallCmdSetTransmit))]
    [HarmonyPatch(new Type[]
    {
        typeof(bool)
    })]
    class SetTransmitPatch
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(Intercom __instance, bool player)
        {
            Plugin.Log.Debug("SetTransmitPatch");
            Player ply = Player.Get(__instance.gameObject);
            if (!Plugin.GhostList.Contains(ply)) return true;

            ply.ClearBroadcasts();
            ply.Broadcast(3, Translation.Translation.GetText().IntercomDenied);

            return false;
        }
    }
}
