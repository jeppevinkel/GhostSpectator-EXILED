using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets._Scripts.Dissonance;
using Harmony;

namespace GhostSpectator.Patches
{
	[HarmonyPatch(typeof(PlayerEffectsController))]
	[HarmonyPatch(nameof(PlayerEffectsController.EnableEffect))]
	[HarmonyPatch(new[]
	{
		typeof(string),
	})]
	class PlayerEnableEffectPatch
	{
		public static bool Prefix(PlayerEffectsController __instance, string apiName)
		{
			if (Plugin.GhostList.Contains(__instance.GetComponent<ReferenceHub>()))
			{
				return false;
			}

			return true;
		}
	}
	[HarmonyPatch(typeof(PlayerEffectsController))]
	[HarmonyPatch(nameof(PlayerEffectsController.DisableEffect))]
	[HarmonyPatch(new[]
	{
		typeof(string),
	})]
	class PlayerDisableEffectPatch
	{
		public static bool Prefix(PlayerEffectsController __instance, string apiName)
		{
			if (Plugin.GhostList.Contains(__instance.GetComponent<ReferenceHub>()))
			{
				return false;
			}

			return true;
		}
	}
}
