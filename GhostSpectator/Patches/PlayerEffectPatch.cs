using Exiled.API.Features;
using HarmonyLib;

namespace GhostSpectator.Patches
{
	//[HarmonyPatch(typeof(PlayerEffectsController))]
	//[HarmonyPatch(nameof(PlayerEffectsController.EnableEffect))]
	//[HarmonyPatch(new[]
	//{
	//	typeof(string),
	//})]
	//class PlayerEnableEffectPatch
	//{
	//	public static bool Prefix(PlayerEffectsController __instance, string apiName)
	//	{
	//		return !Plugin.GhostList.Contains(Player.Get(__instance.gameObject));
	//	}
	//}
	//[HarmonyPatch(typeof(PlayerEffectsController))]
	//[HarmonyPatch(nameof(PlayerEffectsController.DisableEffect))]
	//[HarmonyPatch(new[]
	//{
	//	typeof(string),
	//})]
	//class PlayerDisableEffectPatch
	//{
	//	public static bool Prefix(PlayerEffectsController __instance)
	//	{
	//		return !Plugin.GhostList.Contains(Player.Get(__instance.gameObject));
	//	}
	//}
}
