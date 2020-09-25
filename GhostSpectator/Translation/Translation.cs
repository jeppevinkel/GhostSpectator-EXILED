using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using YamlDotNet.Serialization;

namespace GhostSpectator.Translation
{
    public static class Translation
    {
        public static readonly Dictionary<string, Language> Langs = new Dictionary<string, Language>();

        private static readonly string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static readonly string ExiledDir = Path.Combine(AppData, "EXILED");
        private static readonly string PluginsDir = Path.Combine(ExiledDir, "Plugins");
        private static readonly string MyDir = Path.Combine(PluginsDir, Plugin.Instance.Name);
        private static readonly string TransDir = Path.Combine(MyDir, "Translations");

        private static readonly IDeserializer Deserializer = new DeserializerBuilder().Build();
        private static readonly ISerializer Serializer = new SerializerBuilder().Build();

        private static string LangString
        {
            get
            {
                CultureInfo ci = CultureInfo.DefaultThreadCurrentUICulture;

                if (Langs.ContainsKey(ci.Name))
                {
                    return ci.Name;
                }
                else if (Langs.ContainsKey(ci.TwoLetterISOLanguageName))
                {
                    return ci.TwoLetterISOLanguageName;
                }
                else
                {
                    return "en";
                }
            }
        }

        public static string GetString(string key)
        {
            return (string)Langs[LangString].GetType().GetProperty(key)?.GetValue(Langs[LangString], null);
        }

        public static Language GetText()
        {
            return Langs[LangString];
        }

        public static void LoadTranslations()
        {
            Plugin.Log.Info("Loading translations...");
            foreach (var kvp in DefaultLanguages)
            {
	            if (Langs.ContainsKey(kvp.Key)) continue;
                Langs.Add(kvp.Key, kvp.Value);
            }

            if (!Directory.Exists(TransDir))
            {
                Directory.CreateDirectory(TransDir);
            }

            string[] translationFiles = Directory.GetFiles(TransDir);

            foreach (string file in translationFiles)
            {
                if (file.Contains("BACKUP") || !file.EndsWith(".yml"))
	            {
                    continue;
	            }
                try
                {
                    Language trans;
                    FileInfo fInfo = new FileInfo(file);
                    using (StreamReader f = File.OpenText(file))
                    {
                        trans = Deserializer.Deserialize<Language>(f);
                    }

                    if (Langs.ContainsKey(Path.GetFileNameWithoutExtension(fInfo.Name)))
                    {
                        Langs[Path.GetFileNameWithoutExtension(fInfo.Name)] = trans;
                    }
                    else
                    {
                        Langs.Add(Path.GetFileNameWithoutExtension(fInfo.Name), trans);
                    }
                    Plugin.Log.Debug($"Loaded lang: {Path.GetFileNameWithoutExtension(fInfo.Name)}");
                }
                catch (Exception e)
                {
                    Plugin.Log.Error(e.Message);
                }
            }

            Plugin.Log.Info("Loaded translations.");
            SaveTranslations();
        }

        public static void SaveTranslations()
        {
            Plugin.Log.Debug("Saving translations...");

            if (!Directory.Exists(TransDir))
            {
                Directory.CreateDirectory(TransDir);
            }

            foreach (var kvp in Langs)
            {
                try
                {
                    string yaml = Serializer.Serialize(kvp.Value);

                    if (File.Exists(Path.Combine(TransDir, $"{kvp.Key}.json")))
                    {
                        string oldYaml = File.ReadAllText(Path.Combine(TransDir, $"{kvp.Key}.yml"));
                        if (oldYaml != yaml)
                        {
                            File.Copy(Path.Combine(TransDir, $"{kvp.Key}.yml"), Path.Combine(TransDir, $"{kvp.Key}_BACKUP.yml"));
                        }
                    }

                    File.WriteAllText(Path.Combine(TransDir, $"{kvp.Key}.yml"), yaml);
                    Plugin.Log.Debug($"Saved lang: {kvp.Key}");
                }
                catch (Exception e)
                {
                    Plugin.Log.Error(e.Message);
                }
            }

            Plugin.Log.Debug("Saved translations.");
        }

        internal static readonly Dictionary<string, Language> DefaultLanguages = new Dictionary<string, Language>
        {
            {
                "en",
                new Language()
            },
            {
	            "da",
	            new Language
	            {
		            Change914KnobDenied = "Tilskuere kan ikke indstille SCP-914",
		            ContainDenied = "Tilskuere kan ikke fange SCP'er.",
		            DetonateWarheadDenied = "Tilskuere kan ikke detonere sprænghovedet.",
		            DetonationCancelDenied = "Tilskuere kan ikke annullere detonationen.",
		            DoorDenied = "Tilskuere kan ikke åbne døre.",
		            ElevatorTeleport = "Teleporterer igennem elevatoren.",
		            GeneratorDenied = "Tilskuere kan ikke bruge generatore.",
		            IntercomDenied = "Tilskuere kan ikke bruge samtaleanlægget.",
		            LeverDenied = "Tilskuere kan ikke bruge håndtag.",
		            LockerDenied = "Tilskuere kan ikke åbne skabe.",
		            NotImplementedYet = "Denne funktion er ikke implementeret endnu.",
		            TeleportNone = "Ingen spillere fundet.",
		            TeleportTo = "Tilskurer nu {0}.",
		            Use914Denied = "Tilskuere kan ikke bruge SCP-914.",
		            WarheadEnableDenied = "Tilskuere kan ikke aktivere sprænghovedet.",
		            WarheadKeycardDenied = "Tilskuere kan ikke aktivere sprænghovedknappen.",
		            WorkstationPlaceDenied = "Tilskuere kan ikke bruge arbejdstationer.",
		            WorkstationTakeDenied = "Tilskuere kan ikke bruge arbejdstationer.",
		            SpecmodeNormal = "Du har slået ghost mode fra.",
		            SpecmodeGhost = "Du har slået ghost mode til."
	            }
            },
            {
	            "de",
	            new Language
	            {
		            Change914KnobDenied = "Zuschauer können SCP-914 nicht verändern.",
		            ContainDenied = "Zuschauer können SCP's nicht eindämmen.",
		            DetonateWarheadDenied = "Zuschauer können den Alpha Warhead nicht zünden.",
		            DetonationCancelDenied = "Zuschauer können die Detonation nicht verhindern.",
		            DoorDenied = "Zuschauer können nicht mit Türen interagieren.",
		            ElevatorTeleport = "Teleportiere durch den Aufzug.",
		            GeneratorDenied = "Zuschauer können nicht mit Generatoren interagieren.",
		            IntercomDenied = "Zuschauer können das Intercom nicht benutzen.",
		            LeverDenied = "Zuschauer können nicht mit Hebel interagieren.",
		            LockerDenied = "Zuschauer können nicht mit Schränken interagieren.",
		            NotImplementedYet = "Dieses Feature wurde noch nicht implementiert.",
		            TeleportNone = "Kein Spieler gefunden.",
		            TeleportTo = "Du schaust nun {0} zu.",
		            Use914Denied = "Zuschauer können SCP-914 nicht benutzen.",
		            WarheadEnableDenied = "Zuschauer können den Alpha Warhead nicht aktivieren.",
		            WarheadKeycardDenied = "Zuschauer können den Alpha Warhead Knopf nicht aktivieren.",
		            WorkstationPlaceDenied = "Zuschauer können keine Workstations verwenden.",
		            WorkstationTakeDenied = "Zuschauer können keine Workstations verwenden.",
		            SpecmodeNormal = "Du bist nun im normalen Zuschauermodus.",
		            SpecmodeGhost = "Du bist nun im Geister-Zuschauermodus."
                }
            }
        };
    }

    public class Language
    {
        public string Change914KnobDenied { get; set; } = "Spectators can't change the knob of SCP-914.";
        public string ContainDenied { get; set; } = "Spectators can't contain SCPs.";
        public string DetonateWarheadDenied { get; set; } = "Spectators can't detonate the warhead.";
        public string DetonationCancelDenied { get; set; } = "Spectators can't cancel the detonation.";
        public string DoorDenied { get; set; } = "Spectators can't open doors.";
        public string ElevatorTeleport { get; set; } = "Teleporting through the elevator.";
        public string GeneratorDenied { get; set; } = "Spectators can't interact with generators.";
        public string IntercomDenied { get; set; } = "Spectators can't use the intercom.";
        public string LeverDenied { get; set; } = "Spectators can't use levers.";
        public string LockerDenied { get; set; } = "Spectators can't open lockers.";
        public string NotImplementedYet { get; set; } = "This feature has not been implemented yet.";
        public string TeleportNone { get; set; } = "No players found.";
        public string TeleportTo { get; set; } = "Now spectating {0}.";
        public string Use914Denied { get; set; } = "Spectators can't use SCP-914.";
        public string WarheadEnableDenied { get; set; } = "Spectators can't enable the warhead.";
        public string WarheadKeycardDenied { get; set; } = "Spectators can't activate the warhead button.";
        public string WorkstationPlaceDenied { get; set; } = "Spectators can't use workstations.";
        public string WorkstationTakeDenied { get; set; } = "Spectators can't use workstations.";
        public string SpecmodeNormal { get; set; } = "You have toggled normal spectating mode.";
        public string SpecmodeGhost { get; set; } = "You have toggled ghost spectating mode.";

        public string RateLimited { get; set; } =
            "You have been rate-limited, please wait a few seconds before using this command.";
    }
}