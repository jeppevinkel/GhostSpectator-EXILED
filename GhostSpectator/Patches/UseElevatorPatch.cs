using System;
using Exiled.API.Features;
using HarmonyLib;
using UnityEngine;

namespace GhostSpectator.Patches
{
    [HarmonyPatch(typeof(PlayerInteract))]
    [HarmonyPatch(nameof(PlayerInteract.CallCmdUseElevator))]
    [HarmonyPatch(new[] { typeof(GameObject) })]
    internal class UseElevatorPatch
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(PlayerInteract __instance, GameObject elevator)
        {
            Plugin.Log.Debug("UseElevatorPatch");
            Player ply = Player.Get(__instance.gameObject);
            if (!Plugin.GhostList.Contains(ply)) return true;

            var lift = elevator.GetComponent<Lift>();
            Lift.Elevator[] elevators = lift.elevators;

            float furthestDistance = -1;
            Transform furthestObject = null;

            foreach (Lift.Elevator Object in elevators)
            {
                float objectDistance = Vector3.Distance(ply.Position, Object.target.position);
                if (!(objectDistance > furthestDistance)) continue;
                furthestObject = Object.target;
                furthestDistance = objectDistance;
            }

            if (furthestObject != null) ply.Position = (furthestObject.position + furthestObject.right * 4.5f);

            ply.ClearBroadcasts();
            ply.Broadcast(3, Translation.Translation.GetText().ElevatorTeleport);

            return false;
        }
    }
}
