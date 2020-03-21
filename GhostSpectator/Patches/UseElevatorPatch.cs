using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdUseElevator))]
    class UseElevatorPatch
    {
        public static bool Prefix(PlayerInteract __instance, GameObject elevator)
        {
            return !Plugin.GhostList.Contains(__instance.GetComponent<ReferenceHub>());
        }
    }
}
