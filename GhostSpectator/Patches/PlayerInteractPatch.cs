using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;

namespace GhostSpectator.Patches
{

    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdOpenDoor))]
    class PlayerInteractOpenDoorPatch
    {
        public static bool Prefix(PlayerInteract __instance, ref GameObject doorId)
        {
            return !Plugin.GhostList.Contains(__instance.GetComponent<ReferenceHub>());
        }
    }
}
