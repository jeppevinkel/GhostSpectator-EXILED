using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mirror;
using UnityEngine;

namespace GhostSpectator
{
	public static class CustomSpawnManager
	{
        public static void SetClassCustomSpawn(this ReferenceHub rh, RoleType role, Vector3 pos, float rotY = 0, bool lite = false)
        {
            rh.characterClassManager.NetworkCurClass = role;
            rh.playerStats.MakeHpDirty();
            rh.playerStats.unsyncedArtificialHealth = 0.0f;
            rh.characterClassManager.AliveTime = 0.0f;

            rh.ApplyPropertiesCustom(pos, rotY, lite);
        }

        public static void ApplyPropertiesCustom(this ReferenceHub rh, Vector3 pos, float rotY, bool lite = false)
        {
            Role role = rh.characterClassManager.Classes.SafeGet(rh.characterClassManager.CurClass);
            if (!rh.characterClassManager._wasAnytimeAlive && rh.characterClassManager.CurClass != RoleType.Spectator && rh.characterClassManager.CurClass != RoleType.None)
                rh.characterClassManager._wasAnytimeAlive = true;
            rh.characterClassManager.InitSCPs();
            rh.characterClassManager.AliveTime = 0.0f;
            switch (role.team)
            {
                case Team.MTF:
                    AchievementManager.Achieve("arescue", true);
                    break;
                case Team.CHI:
                    AchievementManager.Achieve("chaos", true);
                    break;
                case Team.RSC:
                case Team.CDP:
                    rh.characterClassManager.EscapeStartTime = (int)Time.realtimeSinceStartup;
                    break;
            }
            rh.characterClassManager.GetComponent<Inventory>();
            try
            {
                rh.characterClassManager.GetComponent<FootstepSync>().SetLoudness(role.team, role.roleId.Is939());
            }
            catch
            {
            }
            if (NetworkServer.active)
            {
                Handcuffs component = rh.characterClassManager.GetComponent<Handcuffs>();
                component.ClearTarget();
                component.NetworkCufferId = -1;
            }
            if (role.team != Team.RIP)
            {
                if (NetworkServer.active && !lite)
                {
                    rh.characterClassManager._pms.OnPlayerClassChange(pos, rotY);
                    if (!rh.characterClassManager.SpawnProtected && rh.characterClassManager.EnableSP && rh.characterClassManager.SProtectedTeam.Contains((int)role.team))
                    {
                        rh.characterClassManager.GodMode = true;
                        rh.characterClassManager.SpawnProtected = true;
                        rh.characterClassManager.ProtectedTime = Time.time;
                    }
                }
                if (!rh.characterClassManager.isLocalPlayer)
                    rh.characterClassManager.GetComponent<PlayerStats>().maxHP = role.maxHP;
            }
            rh.characterClassManager.Scp049.iAm049 = rh.characterClassManager.CurClass == RoleType.Scp049;
            rh.characterClassManager.Scp0492.iAm049_2 = rh.characterClassManager.CurClass == RoleType.Scp0492;
            rh.characterClassManager.Scp096.iAm096 = rh.characterClassManager.CurClass == RoleType.Scp096;
            rh.characterClassManager.Scp106.iAm106 = rh.characterClassManager.CurClass == RoleType.Scp106;
            rh.characterClassManager.Scp173.iAm173 = rh.characterClassManager.CurClass == RoleType.Scp173;
            rh.characterClassManager.Scp939.iAm939 = rh.characterClassManager.CurClass.Is939();
            rh.characterClassManager.RefreshPlyModel(RoleType.None);
        }
    }
}
