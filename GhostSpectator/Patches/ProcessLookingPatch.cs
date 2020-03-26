using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;

namespace GhostSpectator.Patches
{
	[HarmonyPatch(typeof(Scp096PlayerScript), nameof(Scp096PlayerScript.ProcessLooking))]
	class ProcessLookingPatch
	{
		[HarmonyPriority(Priority.First)]
		public static bool Prefix(Scp096PlayerScript __instance)
		{
			Plugin.Log.Debug("ProcessLookingPatch");
			return !Plugin.GhostList.Contains(__instance.GetComponent<ReferenceHub>());
		}
	}
}
