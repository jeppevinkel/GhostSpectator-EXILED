using Exiled.API.Features;
using HarmonyLib;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(PlayerInteract))]
    [HarmonyPatch(nameof(PlayerInteract.CallCmdUse914))]
    class Use914Patch
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(PlayerInteract __instance)
        {
            Plugin.Log.Debug("Use914Patch");
            Player ply = Player.Get(__instance.gameObject);
            if (!Plugin.GhostList.Contains(ply)) return true;

            ply.ClearBroadcasts();
            ply.Broadcast(3, Translation.Translation.GetText().Use914Denied);

            return false;
        }
    }
}
