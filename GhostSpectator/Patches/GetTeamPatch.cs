using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EXILED.Extensions;
using Harmony;

namespace GhostSpectator.Patches
{
	[HarmonyPatch(typeof(Player))]
	[HarmonyPatch(nameof(Player.GetTeam))]
	[HarmonyPatch(new Type[]
	{
		typeof(ReferenceHub)
	})]
	class GetTeamPatch
	{
		[HarmonyPriority(Priority.First)]
		public static bool Prefix(ReferenceHub __instance, Team __result)
		{
			Plugin.Log.Debug("GetTeamPatch");
			if (!Plugin.GhostList.Contains(__instance)) return true;

			__result = Team.RIP;

			return false;
		}
	}
}
