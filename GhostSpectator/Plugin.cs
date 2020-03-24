using System;
using System.Threading;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using EXILED;
using Harmony;
using Logger;
using MEC;
using GhostSpectator.Patches;
using GhostSpectator.Localization;
using Mirror;
using RemoteAdmin;
using UnityEngine;

namespace GhostSpectator
{
    public class Plugin : EXILED.Plugin
    {
        public EventHandlers EventHandlers;
        public CommandHandler CommandHandler;

        public static Logger.Logger Log;
        public static List<ReferenceHub> GhostList = new List<ReferenceHub>();
        public static Dictionary<string, int> GhostPos = new Dictionary<string, int>();
        public static RoleType GhostRole = RoleType.Tutorial;

        public bool Enabled;
        public bool AllowDamage;
        public bool AllowPickup;
        public bool DebugMode;
        public string Lang;
        public static bool GhostInteract;
        public static string GhostMessage;
        public static bool GhostGod;
        public static bool GhostRagdoll;
        public static bool GhostNoclip;

        private HarmonyInstance _instance;
        private static int _patchFixer;

        public override void OnEnable()
        {
            try
            {
                ReloadConfig();
                if (!Enabled)
                {
                    return;
                }
                Log = new Logger.Logger("GhostSpectator", DebugMode || Config.GetBool("exiled_debug"));

                Log.Info($"Attempting to set language to {Lang}.");

                Translation.LoadTranslations();

                CultureInfo ci;
                try
                {
                    ci = CultureInfo.GetCultureInfo(Lang);
                }
                catch (Exception e)
                {
                    ci = CultureInfo.GetCultureInfo("en");
                    Log.Error($"{Lang} is not a valid language. Defaulting to English.");
                    Log.Error($"{e.Message}");
                }
                
                CultureInfo.DefaultThreadCurrentCulture = ci;
                CultureInfo.DefaultThreadCurrentUICulture = ci;
                Log.Info($"Language set to {ci.DisplayName}.");
                Log.Debug($"Language test {Translation.GetText().doorDenied}.");

                Log.Debug("Initializing event handlers..");
                EventHandlers = new EventHandlers(this);
                CommandHandler = new CommandHandler(this);

                Events.WaitingForPlayersEvent += EventHandlers.OnWaitingForPlayers;
                Events.RoundEndEvent += EventHandlers.OnRoundEnd;
                Events.DropItemEvent += EventHandlers.OnDropItem;
                Events.PickupItemEvent += EventHandlers.OnPickupItem;
                Events.PlayerHurtEvent += EventHandlers.OnPlayerHurt;
                Events.TeamRespawnEvent += EventHandlers.OnTeamRespawn;
                Events.PlayerDeathEvent += EventHandlers.OnPlayerDeath;
                Events.PlayerSpawnEvent += EventHandlers.OnPlayerSpawn;
                Events.ItemChangedEvent += EventHandlers.OnItemChanged;

                Events.RemoteAdminCommandEvent += CommandHandler.OnRACommand;

                Log.Debug("Patching...");
                try
                {
                    //You must use an incrementer for the harmony instance name, otherwise the new instance will fail to be created if the plugin is reloaded.
                    _patchFixer++;
                    _instance = HarmonyInstance.Create($"ghostspectator.patches{_patchFixer}");
                    _instance.PatchAll();
                }
                catch (Exception exception)
                {
                    Log.Error($"Patching failed! {exception}");
                }

                Timing.RunCoroutine(EventHandlers.SlowUpdate());

                Log.Info($"{getName} loaded. c:");
            }
            catch (Exception e)
            {
                Log.Error($"There was an error loading the plugin: {e}");
            }
        }

        public override void OnDisable()
        {
            Events.WaitingForPlayersEvent -= EventHandlers.OnWaitingForPlayers;
            Events.RoundEndEvent -= EventHandlers.OnRoundEnd;
            Events.DropItemEvent -= EventHandlers.OnDropItem;
            Events.PickupItemEvent -= EventHandlers.OnPickupItem;
            Events.PlayerHurtEvent -= EventHandlers.OnPlayerHurt;
            Events.TeamRespawnEvent -= EventHandlers.OnTeamRespawn;
            Events.PlayerDeathEvent -= EventHandlers.OnPlayerDeath;
            Events.PlayerSpawnEvent -= EventHandlers.OnPlayerSpawn;
            Events.ItemChangedEvent -= EventHandlers.OnItemChanged;

            Events.RemoteAdminCommandEvent -= CommandHandler.OnRACommand;

            EventHandlers = null;

            Log.Debug("Unpatching...");
            _instance.UnpatchAll();
            Log.Debug("Unpatching complete. Goodbye.");
        }

        public void ReloadConfig()
        {
            Enabled = Config.GetBool("gs_enable", true);
            AllowDamage = Config.GetBool("gs_allow_damage", false);
            AllowPickup = Config.GetBool("gs_allow_pickup", false);
            DebugMode = Config.GetBool("gs_debug", false);
            GhostGod = Config.GetBool("gs_god_mode", true);
            GhostInteract = Config.GetBool("gs_interact", false);
            GhostRagdoll = Config.GetBool("gs_ragdoll", false);
            GhostNoclip = Config.GetBool("gs_noclip", true);
            GhostMessage = Config.GetString("gs_ghost_message",
                "You have been spawned as a spectator ghost.\n" +
                "Drop your <color=#ff0000>7.62</color> to be <color=#ff0000>teleported</color> to the <color=#ff0000>next</color> player\n" +
                "Drop your <color=#ff0000>5.56</color> to be <color=#ff0000>teleported</color> to the <color=#ff0000>previous</color> player");
            Lang = Config.GetString("gs_language",
                "en-US");
        }

        public override void OnReload()
        {
            //This is only fired when you use the EXILED reload command, the reload command will call OnDisable, OnReload, reload the plugin, then OnEnable in that order. There is no GAC bypass, so if you are updating a plugin, it must have a unique assembly name, and you need to remove the old version from the plugins folder
        }

        public static string GetName { get; } = "GhostSpectator";
        public override string getName { get; } = GetName;
    }
}