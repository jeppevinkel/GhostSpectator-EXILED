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
	[HarmonyPatch(nameof(CharacterClassManager.ApplyProperties))]
	[HarmonyPatch(new[]
	{
		typeof(bool),
		typeof(bool),
	})]
	class PlayerSpawnEventPatch
	{
		[HarmonyPriority(Priority.First)]
		public static bool Prefix(CharacterClassManager __instance, bool lite = false, bool escape = false)
		{
			if (Plugin.GhostsBeingSpawned.Contains(__instance.GetComponent<ReferenceHub>()))
			{
				Plugin.Log.Debug("Caught the PlayerSpawnEvent.");
				Plugin.GhostsBeingSpawned.Remove(__instance.GetComponent<ReferenceHub>());
				return false;
			}

			return true;
		}
	}
}
