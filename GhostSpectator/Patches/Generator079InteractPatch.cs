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
    [HarmonyPatch(typeof(Generator079))]
    [HarmonyPatch(nameof(Generator079.Interact))]
    [HarmonyPatch(new Type[]
    {
        typeof(GameObject),
        typeof(string)
    })]
    class Generator079InteractPatch
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(Generator079 __instance, GameObject person, string command)
        {
            Plugin.Log.Debug("Generator079InteractPatch");
            if (!Plugin.GhostList.Contains(person.GetComponent<ReferenceHub>())) return true;
            ReferenceHub rh = person.GetComponent<ReferenceHub>();

            rh.ClearBroadcasts();
            rh.Broadcast(3, Translation.strings.generatorDenied);

            return false;
        }
    }
}
