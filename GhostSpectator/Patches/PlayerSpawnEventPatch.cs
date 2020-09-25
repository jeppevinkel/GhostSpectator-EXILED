using Exiled.API.Features;
using HarmonyLib;

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
			if (!Plugin.GhostsBeingSpawned.Contains(Player.Get(__instance.gameObject))) return true;
			Plugin.Log.Debug("Caught the PlayerSpawnEvent.");
			Plugin.GhostsBeingSpawned.Remove(Player.Get(__instance.gameObject));
			return false;

		}
	}
}
