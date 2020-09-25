using Exiled.API.Features;
using HarmonyLib;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(PlayerInteract))]
    [HarmonyPatch(nameof(PlayerInteract.CallCmdSwitchAWButton))]
    class SwitchAWButtonPatch
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(PlayerInteract __instance)
        {
            Plugin.Log.Debug("SwitchAWButtonPatch");
            Player ply = Player.Get(__instance.gameObject);
            if (!Plugin.GhostList.Contains(ply)) return true;

            ply.ClearBroadcasts();
            ply.Broadcast(3, Translation.Translation.GetText().WarheadKeycardDenied);

            return false;
        }
    }
}
