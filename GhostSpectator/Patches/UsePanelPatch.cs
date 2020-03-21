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
    [HarmonyPatch(nameof(PlayerInteract.CallCmdUsePanel))]
    [HarmonyPatch(new Type[]
    {
        typeof(string)
    })]
    class UsePanelPatch
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(PlayerInteract __instance, string n)
        {
            Plugin.Log.Debug("UsePanelPatch");
            if (!Plugin.GhostList.Contains(__instance.GetComponent<ReferenceHub>())) return true;
            ReferenceHub rh = __instance.GetComponent<ReferenceHub>();

            if (n.Contains("cancel"))
            {
                rh.ClearBroadcasts();
                rh.Broadcast(3, Translation.GetText().detonationCancelDenied);
            }
            else if (n.Contains("lever"))
            {
                rh.ClearBroadcasts();
                rh.Broadcast(3, Translation.GetText().detonationCancelDenied);
            }
            else
            {
                rh.ClearBroadcasts();
                rh.Broadcast(3, Translation.GetText().leverDenied);
            }

            return false;
        }
    }
}
