using Exiled.API.Features;
using HarmonyLib;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(PlayerInteract))]
    [HarmonyPatch(nameof(PlayerInteract.CallCmdUsePanel))]
    [HarmonyPatch(new[]
    {
        typeof(PlayerInteract.AlphaPanelOperations)
    })]
    class UsePanelPatch
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(PlayerInteract __instance, PlayerInteract.AlphaPanelOperations n)
        {
            Plugin.Log.Debug("UsePanelPatch");
            Player ply = Player.Get(__instance.gameObject);
            if (!Plugin.GhostList.Contains(ply)) return true;

            if (n == PlayerInteract.AlphaPanelOperations.Cancel)
            {
	            ply.ClearBroadcasts();
                ply.Broadcast(3, Translation.Translation.GetText().DetonationCancelDenied);
            }
            else if (n == PlayerInteract.AlphaPanelOperations.Lever)
            {
	            ply.ClearBroadcasts();
                ply.Broadcast(3, Translation.Translation.GetText().DetonationCancelDenied);
            }
            else
            {
	            ply.ClearBroadcasts();
                ply.Broadcast(3, Translation.Translation.GetText().LeverDenied);
            }

            return false;
        }
    }
}
