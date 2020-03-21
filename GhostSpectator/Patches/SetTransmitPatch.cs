using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EXILED.Extensions;
using Harmony;
using UnityEngine;

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
            if (!Plugin.GhostList.Contains(__instance.GetComponent<ReferenceHub>())) return true;
            ReferenceHub rh = __instance.GetComponent<ReferenceHub>();

            rh.ClearBroadcasts();
            rh.Broadcast(3, Translation.strings.intercomDenied);

            return false;
        }
    }
}
