using System;
using Exiled.API.Features;
using HarmonyLib;
using UnityEngine;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(Generator079))]
    [HarmonyPatch(nameof(Generator079.Interact))]
    [HarmonyPatch(new Type[]
    {
        typeof(GameObject),
        typeof(PlayerInteract.Generator079Operations)
    })]
    class Generator079InteractPatch
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(Generator079 __instance, GameObject person, PlayerInteract.Generator079Operations command)
        {
            Plugin.Log.Debug("Generator079InteractPatch");
            Player ply = Player.Get(__instance.gameObject);
            if (!Plugin.GhostList.Contains(ply)) return true;

            ply.ClearBroadcasts();
            ply.Broadcast(3, Translation.Translation.GetText().GeneratorDenied);

            return false;
        }
    }
}
