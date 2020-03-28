using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Utf8Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace GhostSpectator.Localization
{
    public static class Translation
    {
        public static readonly Dictionary<string, Language> Langs = new Dictionary<string, Language>();

        private static readonly string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static readonly string PluginsDir = Path.Combine(AppData, "Plugins");
        private static readonly string MyDir = Path.Combine(PluginsDir, Plugin.GetName);
        private static readonly string TransDir = Path.Combine(MyDir, "Translations");

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
                if (file.Contains("BACKUP"))
	            {
                    continue;
	            }
                try
                {
                    Language trans;
                    FileInfo fInfo = new FileInfo(file);
                    using (StreamReader f = File.OpenText(file))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        trans = (Language) serializer.Deserialize(f, typeof(Language));
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
                    JsonSerializer serializer = new JsonSerializer();
                    string json = JsonConvert.SerializeObject(kvp.Value, Formatting.Indented);

                    if (File.Exists(Path.Combine(TransDir, $"{kvp.Key}.json")))
                    {
                        string oldJson = File.ReadAllText(Path.Combine(TransDir, $"{kvp.Key}.json"));
                        if (oldJson != json)
                        {
                            File.Copy(Path.Combine(TransDir, $"{kvp.Key}.json"), Path.Combine(TransDir, $"{kvp.Key}_BACKUP.json"));
                        }
                    }

                    File.WriteAllText(Path.Combine(TransDir, $"{kvp.Key}.json"), json);
                    Plugin.Log.Debug($"Saved lang: {kvp.Key}");
                }
                catch (Exception e)
                {
                    Plugin.Log.Error(e.Message);
                }
            }

            Plugin.Log.Debug("Saved translations.");
        }

        private static readonly Dictionary<string, Language> DefaultLanguages = new Dictionary<string, Language>
        {
            {
                "en",
                new Language()
            },
            {
	            "da",
	            new Language
	            {
		            change914KnobDenied = "Tilskuere kan ikke indstille SCP-914",
		            containDenied = "Tilskuere kan ikke fange SCP'er.",
		            detonateWarheadDenied = "Tilskuere kan ikke detonere sprænghovedet.",
		            detonationCancelDenied = "Tilskuere kan ikke annullere detonationen.",
		            doorDenied = "Tilskuere kan ikke åbne døre.",
		            elevatorTeleport = "Teleporterer igennem elevatoren.",
		            generatorDenied = "Tilskuere kan ikke bruge generatore.",
		            intercomDenied = "Tilskuere kan ikke bruge samtaleanlægget.",
		            leverDenied = "Tilskuere kan ikke bruge håndtag.",
		            lockerDenied = "Tilskuere kan ikke åbne skabe.",
		            notImplementedYet = "Denne funktion er ikke implementeret endnu.",
		            teleportNone = "Ingen spillere fundet.",
		            teleportTo = "Tilskurer nu {0}.",
		            use914Denied = "Tilskuere kan ikke bruge SCP-914.",
		            warheadEnableDenied = "Tilskuere kan ikke aktivere sprænghovedet.",
		            warheadKeycardDenied = "Tilskuere kan ikke aktivere sprænghovedknappen.",
		            workstationPlaceDenied = "Tilskuere kan ikke bruge arbejdstationer.",
		            workstationTakeDenied = "Tilskuere kan ikke bruge arbejdstationer.",
		            specmodeNormal = "Du har slået ghost mode fra.",
		            specmodeGhost = "Du har slået ghost mode til."
	            }
            },
            {
	            "de",
	            new Language
	            {
		            change914KnobDenied = "Zuschauer können SCP-914 nicht verändern.",
		            containDenied = "Zuschauer können SCP's nicht eindämmen.",
		            detonateWarheadDenied = "Zuschauer können den Alpha Warhead nicht zünden.",
		            detonationCancelDenied = "Zuschauer können die Detonation nicht verhindern.",
		            doorDenied = "Zuschauer können nicht mit Türen interagieren.",
		            elevatorTeleport = "Teleportiere durch den Aufzug.",
		            generatorDenied = "Zuschauer können nicht mit Generatoren interagieren.",
		            intercomDenied = "Zuschauer können das Intercom nicht benutzen.",
		            leverDenied = "Zuschauer können nicht mit Hebel interagieren.",
		            lockerDenied = "Zuschauer können nicht mit Schränken interagieren.",
		            notImplementedYet = "Dieses Feature wurde noch nicht implementiert.",
		            teleportNone = "Kein Spieler gefunden.",
		            teleportTo = "Du schaust nun {0} zu.",
		            use914Denied = "Zuschauer können SCP-914 nicht benutzen.",
		            warheadEnableDenied = "Zuschauer können den Alpha Warhead nicht aktivieren.",
		            warheadKeycardDenied = "Zuschauer können den Alpha Warhead Knopf nicht aktivieren.",
		            workstationPlaceDenied = "Zuschauer können keine Workstations verwenden.",
		            workstationTakeDenied = "Zuschauer können keine Workstations verwenden.",
		            specmodeNormal = "Du bist nun im normalen Zuschauermodus.",
		            specmodeGhost = "Du bist nun im Geister-Zuschauermodus."
                }
            }
        };

        public class Language
        {
            public string change914KnobDenied = "Spectators can't change the knob of SCP-914.";
            public string containDenied = "Spectators can't contain SCPs.";
            public string detonateWarheadDenied = "Spectators can't detonate the warhead.";
            public string detonationCancelDenied = "Spectators can't cancel the detonation.";
            public string doorDenied = "Spectators can't open doors.";
            public string elevatorTeleport = "Teleporting through the elevator.";
            public string generatorDenied = "Spectators can't interact with generators.";
            public string intercomDenied = "Spectators can't use the intercom.";
            public string leverDenied = "Spectators can't use levers.";
            public string lockerDenied = "Spectators can't open lockers.";
            public string notImplementedYet = "This feature has not been implemented yet.";
            public string teleportNone = "No players found.";
            public string teleportTo = "Now spectating {0}.";
            public string use914Denied = "Spectators can't use SCP-914.";
            public string warheadEnableDenied = "Spectators can't enable the warhead.";
            public string warheadKeycardDenied = "Spectators can't activate the warhead button.";
            public string workstationPlaceDenied = "Spectators can't use workstations.";
            public string workstationTakeDenied = "Spectators can't use workstations.";
            public string specmodeNormal = "You have toggled normal spectating mode.";
            public string specmodeGhost = "You have toggled ghost spectating mode.";
            public string rateLimited = "You have been rate-limited, please wait a few seconds before using this command.";
        }
    }
}