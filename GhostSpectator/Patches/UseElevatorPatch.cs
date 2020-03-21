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
    [HarmonyPatch(typeof(PlayerInteract))]
    [HarmonyPatch(nameof(PlayerInteract.CallCmdUseElevator))]
    [HarmonyPatch(new Type[] { typeof(GameObject) })]
    class UseElevatorPatch
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(PlayerInteract __instance, GameObject elevator)
        {
            Plugin.Log.Debug("UseElevatorPatch");
            if (!Plugin.GhostList.Contains(__instance.GetComponent<ReferenceHub>())) return true;
            ReferenceHub rh = __instance.GetComponent<ReferenceHub>();

            Lift lift = elevator.GetComponent<Lift>();
            Lift.Elevator[] elevators = lift.elevators;

            float furthestDistance = -1;
            Transform furthestObject = null;

            foreach (Lift.Elevator Object in elevators)
            {
                float objectDistance = Vector3.Distance(rh.transform.position, Object.target.position);
                if (objectDistance > furthestDistance)
                {
                    furthestObject = Object.target;
                    furthestDistance = objectDistance;
                }
            }

            if (furthestObject != null) rh.SetPosition(furthestObject.position + furthestObject.right * 5);

            rh.ClearBroadcasts();
            rh.Broadcast(3, Translation.strings.elevatorTeleport);

            return false;
        }
    }
}
