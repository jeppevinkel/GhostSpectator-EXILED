using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EXILED.Extensions;
using Harmony;
using UnityEngine;
using System.Globalization;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(PlayerInteract))]
    [HarmonyPatch(nameof(PlayerInteract.CallCmdUseLocker))]
    [HarmonyPatch(new Type[]
    {
        typeof(int),
        typeof(int)
    })]
    class UseLockerPatch
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(PlayerInteract __instance, int lockerId, int chamberNumber)
        {
            Plugin.Log.Debug("UseLockerPatch");
            if (!Plugin.GhostList.Contains(__instance.GetComponent<ReferenceHub>())) return true;
            ReferenceHub rh = __instance.GetComponent<ReferenceHub>();

            rh.ClearBroadcasts();
            rh.Broadcast(3, Translation.strings.lockerDenied);

            return false;
        }
    }
}
