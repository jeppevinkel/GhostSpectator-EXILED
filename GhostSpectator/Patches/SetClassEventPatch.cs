using Exiled.API.Features;
using HarmonyLib;

namespace GhostSpectator.Patches
{
	[HarmonyPatch(typeof(CharacterClassManager))]
	[HarmonyPatch(nameof(CharacterClassManager.SetClassID))]
	[HarmonyPatch(new[]
	{
		typeof(RoleType),
	})]
	class SetClassEventPatch
	{
		[HarmonyPriority(Priority.First)]
		public static bool Prefix(CharacterClassManager __instance, RoleType id)
		{
			Player ply = Player.Get(__instance.gameObject);
			if (!Plugin.GhostsBeingSpawned.Contains(ply)) return true;
			Plugin.Log.Debug("Caught the SetClassEvent.");
			Plugin.GhostsBeingSpawned.Remove(ply);
			return false;

		}
	}
}
