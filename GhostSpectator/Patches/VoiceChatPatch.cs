using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets._Scripts.Dissonance;
using Harmony;
using Mirror;

namespace GhostSpectator.Patches
{
	[HarmonyPatch(typeof(DissonanceUserSetup))]
	[HarmonyPatch(nameof(DissonanceUserSetup.TargetUpdateForTeam))]
	[HarmonyPatch(new []
	{
		typeof(Team),
	})]
	class VoiceChatPatch
	{
		[HarmonyPriority(Priority.First)]
		public static bool Prefix(DissonanceUserSetup __instance, ref Team team)
		{
			Plugin.Log.Debug($"VoiceChatPatch!   {team.ToString()}");
			if (Plugin.GhostSpectatorVoiceChat)
			{
				if (Plugin.GhostList.Contains(__instance.GetComponent<ReferenceHub>()))
				{
					//team = Team.RIP;
					return false;
				}
			}

			return true;
		}
	}
}
