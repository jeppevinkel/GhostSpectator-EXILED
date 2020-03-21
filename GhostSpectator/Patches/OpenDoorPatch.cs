using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EXILED.Extensions;
using GhostSpectator.Localization;
using Harmony;
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
            if (!Plugin.GhostList.Contains(__instance.GetComponent<ReferenceHub>())) return true;
            ReferenceHub rh = __instance.GetComponent<ReferenceHub>();

            rh.ClearBroadcasts();
            rh.Broadcast(3, Translation.GetText().doorDenied);

            return false;
        }
    }
}
