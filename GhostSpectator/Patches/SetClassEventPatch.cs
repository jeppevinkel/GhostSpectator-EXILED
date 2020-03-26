using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets._Scripts.Dissonance;
using Harmony;

namespace GhostSpectator.Patches
{
	[HarmonyPatch(typeof(CharacterClassManager))]
	[HarmonyPatch(nameof(CharacterClassManager.SetClassID))]
	[HarmonyPatch(new[]
	{
		typeof(RoleType ),
	})]
	class SetClassEventPatch
	{
		[HarmonyPriority(Priority.First)]
		public static bool Prefix(CharacterClassManager __instance, RoleType id)
		{
			if (Plugin.GhostsBeingSpawned.Contains(__instance.GetComponent<ReferenceHub>()))
			{
				Plugin.Log.Debug("Caught the SetClassEvent.");
				Plugin.GhostsBeingSpawned.Remove(__instance.GetComponent<ReferenceHub>());
				return false;
			}

			return true;
		}
	}
}
