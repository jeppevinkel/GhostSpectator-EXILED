using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EXILED.Extensions;
using GhostSpectator.Localization;
using Harmony;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(PlayerInteract))]
    [HarmonyPatch(nameof(PlayerInteract.CallCmdChange914Knob))]
    class Change914KnobPatch
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(PlayerInteract __instance)
        {
            Plugin.Log.Debug("Change914KnobPatch");
            if (!Plugin.GhostList.Contains(__instance.GetComponent<ReferenceHub>())) return true;
            ReferenceHub rh = __instance.GetComponent<ReferenceHub>();

            rh.ClearBroadcasts();
            rh.Broadcast(3, Translation.GetText().change914KnobDenied);

            return false;
        }
    }
}
