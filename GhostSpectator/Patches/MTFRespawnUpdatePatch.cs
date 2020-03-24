using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCore;
using Harmony;
using MEC;
using Mirror;
using Security;
using UnityEngine;

namespace GhostSpectator.Patches
{
	[HarmonyPatch(typeof(MTFRespawn))]
	[HarmonyPatch(nameof(MTFRespawn.Start))]
	class MTFRespawnUpdatePatch
	{
		[HarmonyPriority(Priority.First)]
        public static bool Prefix(MTFRespawn __instance)
		{
			__instance._mtfCustomRateLimit = new RateLimit(4, 2.8f, (NetworkConnection)null);
			__instance._ciThemeRateLimit = new RateLimit(1, 3.5f, (NetworkConnection)null);
			__instance.maxMTFRespawnAmount = ConfigFile.ServerConfig.GetInt("maximum_MTF_respawn_amount", __instance.maxMTFRespawnAmount);
			__instance.maxCIRespawnAmount = ConfigFile.ServerConfig.GetInt("maximum_CI_respawn_amount", __instance.maxCIRespawnAmount);
			__instance.minMtfTimeToRespawn = ConfigFile.ServerConfig.GetInt("minimum_MTF_time_to_spawn", 200);
			__instance.maxMtfTimeToRespawn = ConfigFile.ServerConfig.GetInt("maximum_MTF_time_to_spawn", 400);
			__instance.CI_Percent = (float)ConfigFile.ServerConfig.GetInt("ci_respawn_percent", 35);
			if (!NetworkServer.active || !__instance.isServer || (!__instance.isLocalPlayer || TutorialManager.status))
				return false;
			Timing.RunCoroutine(_Update(__instance), Segment.FixedUpdate);
			return false;
		}

        private static IEnumerator<float> _Update(MTFRespawn instance)
        {
            MTFRespawn mtfRespawn = instance;
            mtfRespawn._hostCcm = mtfRespawn.GetComponent<CharacterClassManager>();
            if (NonFacilityCompatibility.currentSceneSettings.enableRespawning)
            {
                while ((UnityEngine.Object)mtfRespawn != (UnityEngine.Object)null)
                {
                    if ((UnityEngine.Object)mtfRespawn.mtf_a == (UnityEngine.Object)null)
                        mtfRespawn.mtf_a = UnityEngine.Object.FindObjectOfType<ChopperAutostart>();
                    if (!mtfRespawn._hostCcm.RoundStarted)
                    {
                        yield return float.NegativeInfinity;
                    }
                    else
                    {
                        mtfRespawn.timeToNextRespawn -= 0.02f;
                        if ((double)mtfRespawn.respawnCooldown >= 0.0)
                            mtfRespawn.respawnCooldown -= 0.02f;
                        if ((double)mtfRespawn.timeToNextRespawn < (mtfRespawn.nextWaveIsCI ? 13.5 : 18.0) && !mtfRespawn.loaded)
                        {
                            mtfRespawn.loaded = true;
                            if (PlayerManager.players.Any<GameObject>((Func<GameObject, bool>)(ply =>
                            {
	                            return (ply.GetComponent<CharacterClassManager>().CurClass == RoleType.Spectator || Plugin.GhostList.Contains(ply.GetComponent<ReferenceHub>()));
                            })))
                            {
                                mtfRespawn.chopperStarted = true;
                                if (mtfRespawn.nextWaveIsCI && !AlphaWarheadController.Host.detonated)
                                    mtfRespawn.SummonVan();
                                else
                                    mtfRespawn.SummonChopper(true);
                            }
                        }
                        if ((double)mtfRespawn.timeToNextRespawn < 0.0)
                        {
                            float maxDelay = 0.0f;
                            if (!mtfRespawn.nextWaveIsCI && PlayerManager.players.Any<GameObject>((Func<GameObject, bool>)(item =>
                            {
	                            return (item.GetComponent<CharacterClassManager>().CurClass == RoleType.Spectator &&
	                                   !item.GetComponent<ServerRoles>().OverwatchEnabled) || Plugin.GhostList.Contains(item.GetComponent<ReferenceHub>());
                            })))
                            {
                                bool warheadInProgress;
                                bool cassieFree;
                                do
                                {
                                    warheadInProgress = (UnityEngine.Object)AlphaWarheadController.Host != (UnityEngine.Object)null && AlphaWarheadController.Host.inProgress && !AlphaWarheadController.Host.detonated;
                                    cassieFree = NineTailedFoxAnnouncer.singleton.Free && (double)mtfRespawn.decontaminationCooldown <= 0.0;
                                    yield return float.NegativeInfinity;
                                    maxDelay += 0.02f;
                                }
                                while ((double)maxDelay <= 70.0 && !cassieFree | warheadInProgress);
                            }
                            mtfRespawn.loaded = false;
                            if (mtfRespawn.GetComponent<CharacterClassManager>().RoundStarted)
                                mtfRespawn.SummonChopper(false);
                            if (mtfRespawn.chopperStarted)
                            {
                                mtfRespawn.respawnCooldown = 35f;
                                mtfRespawn.RespawnDeadPlayers();
                            }
                            mtfRespawn.nextWaveIsCI = (double)UnityEngine.Random.Range(0, 100) <= (double)mtfRespawn.CI_Percent;
                            mtfRespawn.timeToNextRespawn = (float)UnityEngine.Random.Range(mtfRespawn.minMtfTimeToRespawn, mtfRespawn.maxMtfTimeToRespawn) * (mtfRespawn.nextWaveIsCI ? 1f / mtfRespawn.CI_Time_Multiplier : 1f);
                            mtfRespawn.chopperStarted = false;
                        }
                        yield return float.NegativeInfinity;
                    }
                }
            }
        }
	}
}
