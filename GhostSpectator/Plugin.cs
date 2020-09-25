using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Exiled.API.Features;
using Exiled.Loader;
using ExLocalization.Api;
using GhostSpectator.Translation;
using HarmonyLib;
using Respawning;
using PlayerEv = Exiled.Events.Handlers.Player;
using ServerEv = Exiled.Events.Handlers.Server;
using Scp914Ev = Exiled.Events.Handlers.Scp914;

namespace GhostSpectator
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Instance { get; } = new Plugin();
        private Plugin() { }

	    private Handlers.Player _player;
        private Handlers.Server _server;
        private Handlers.Scp914 _scp914;

        private CommandHandler _commandHandler;
        private EventHandlers _eventHandler;

        public static Logger.Logger Log;
        public static List<Player> GhostList = new List<Player>();
        public static readonly Dictionary<string, GhostSettings> GhostSettings = new Dictionary<string, GhostSettings>();
        public static readonly List<Player> GhostsBeingSpawned = new List<Player>();
        public static RoleType GhostRole = RoleType.Tutorial;

        public static HashSet<Player> RateLimited = new HashSet<Player>();

        private Harmony _instance;
        private static int _patchFixer;

        public override void OnEnabled()
        {
            base.OnEnabled();

            foreach (KeyValuePair<string, Language> defaultLanguage in Translation.Translation.DefaultLanguages)
            {
	            this.RegisterTranslation(defaultLanguage.Value, defaultLanguage.Key);
            }

            Language t = this.LoadTranslation<Language>();

            Exiled.API.Features.Log.Debug(t.ContainDenied);
            Exiled.API.Features.Log.Debug(t.NotImplementedYet);
            Exiled.API.Features.Log.Debug(t.IntercomDenied);

            //Exiled.Events.Events.DisabledPatches.Add(new Tuple<Type, string>(typeof(RespawnManager), nameof(RespawnManager.Spawn)));
            //Exiled.Events.Events.Instance.ReloadDisabledPatches();

            try
            {
                Log = new Logger.Logger("GhostSpectator", Loader.ShouldDebugBeShown);

                Log.Info($"Attempting to set language to {Config.Lang}.");

                Translation.Translation.LoadTranslations();

                CultureInfo ci;

                try
                {
                    ci = CultureInfo.GetCultureInfo(Config.Lang);
                }
                catch (Exception e)
                {
                    ci = CultureInfo.GetCultureInfo("en");
                    Log.Error($"{Config.Lang} is not a valid language. Defaulting to English.");
                    Log.Error($"{e.Message}");
                }
                
                CultureInfo.DefaultThreadCurrentCulture = ci;
                CultureInfo.DefaultThreadCurrentUICulture = ci;
                Log.Info($"Language set to {ci.DisplayName}.");
                Log.Debug($"Language test {Translation.Translation.GetText().DoorDenied}.");

                RegisterEvents();

                Log.Debug("Patching...");
                try
                {
                    //You must use an incrementer for the harmony instance name, otherwise the new instance will fail to be created if the plugin is reloaded.
                    _patchFixer++;
                    _instance = new Harmony($"ghostspectator.patches{_patchFixer}");
                    _instance.PatchAll();
                }
                catch (Exception exception)
                {
                    Log.Error($"Patching failed! {exception}");
                }

                //Timing.RunCoroutine(EventHandlers.SlowUpdate());

                Log.Info($"{Name} loaded. c:");
            }
            catch (Exception e)
            {
                Log.Error($"There was an error loading the plugin: {e}");
            }
        }

        public override void OnDisabled()
        {
            base.OnDisabled();

            UnregisterEvents();

            Log.Debug("Unpatching...");
            _instance.UnpatchAll();
            Log.Debug("Unpatching complete. Goodbye.");
        }

        private void RegisterEvents()
        {
	        Log.Debug("Initializing event handlers..");

	        _player = new Handlers.Player();
	        _server = new Handlers.Server();
            _scp914 = new Handlers.Scp914();

            _commandHandler = new CommandHandler();
            _eventHandler = new EventHandlers();

            PlayerEv.DroppingItem += _player.OnDroppingItem;
	        PlayerEv.PickingUpItem += _player.OnPickingUpItem;
	        PlayerEv.Hurting += _player.OnHurting;
	        PlayerEv.Died += _player.OnDied;
	        PlayerEv.ChangingRole += _player.OnChangingRole;
	        PlayerEv.ChangingItem += _player.OnChangingItem;
	        PlayerEv.Joined += _player.OnJoined;
	        PlayerEv.Interacted += _player.OnInteracted;
	        PlayerEv.InteractingDoor += _player.OnInteractingDoor;
	        PlayerEv.InteractingElevator += _player.OnInteractingElevator;
	        PlayerEv.InteractingLocker += _player.OnInteractingLocker;
	        PlayerEv.IntercomSpeaking += _player.OnIntercomSpeaking;


	        ServerEv.WaitingForPlayers += _server.OnWaitingForPlayers;
	        ServerEv.RestartingRound += _server.OnRestartingRound;
	        ServerEv.RespawningTeam += _server.OnRespawningTeam;

	        ServerEv.SendingRemoteAdminCommand += _commandHandler.OnRACommand;
            ServerEv.SendingConsoleCommand += _eventHandler.OnConsoleCommand;

            Scp914Ev.UpgradingItems += _scp914.OnUpgradingItems;
        }

        private void UnregisterEvents()
        {
	        PlayerEv.DroppingItem -= _player.OnDroppingItem;
	        PlayerEv.PickingUpItem -= _player.OnPickingUpItem;
	        PlayerEv.Hurting -= _player.OnHurting;
	        PlayerEv.Died -= _player.OnDied;
	        PlayerEv.ChangingRole -= _player.OnChangingRole;
	        PlayerEv.ChangingItem -= _player.OnChangingItem;
	        PlayerEv.Joined -= _player.OnJoined;

	        ServerEv.WaitingForPlayers -= _server.OnWaitingForPlayers;
	        ServerEv.RestartingRound -= _server.OnRestartingRound;
	        ServerEv.RespawningTeam -= _server.OnRespawningTeam;

	        ServerEv.SendingRemoteAdminCommand -= _commandHandler.OnRACommand;
            ServerEv.SendingConsoleCommand -= _eventHandler.OnConsoleCommand;

            Scp914Ev.UpgradingItems -= _scp914.OnUpgradingItems;

	        _player = null;
	        _server = null;
	        _scp914 = null;

	        _commandHandler = null;
	        _eventHandler = null;
        }

        public override void OnReloaded()
        {
            //This is only fired when you use the EXILED reload command, the reload command will call OnDisable, OnReload, reload the plugin, then OnEnable in that order. There is no GAC bypass, so if you are updating a plugin, it must have a unique assembly name, and you need to remove the old version from the plugins folder
        }
    }
}