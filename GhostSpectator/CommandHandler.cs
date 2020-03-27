using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EXILED;
using GhostSpectator.Localization;

namespace GhostSpectator
{
    public class CommandHandler
    {
        public Plugin Plugin;
        public CommandHandler(Plugin plugin) => this.Plugin = plugin;

        public void OnRACommand(ref RACommandEvent ev)
        {
            List<string> args = ev.Command.Split(' ').ToList();
            string cmd = args[0];
            args = args.Skip(1).ToList();

            switch (cmd)
            {
                case "gs_translation":
                    ev.Allow = false;
                    switch (args[0])
                    {
                        case "reload":
	                        try
	                        {
		                        Plugin.Log.Info($"{ev.Sender.Nickname} has reloaded the translation files...");
		                        ev.Sender.RaMessage("Reloading the translations...", true);
		                        Translation.LoadTranslations();
                            }
	                        catch (Exception e)
	                        {
		                        Plugin.Log.Error($"{e}");
                                ev.Sender.RaMessage($"An error occured: {e}");
	                        }
                            
                            break;
                        case "set":
                        {
                            CultureInfo ci;
                            string lang = args[1].Replace("\"", "");
	                        try
                            {
                                
	                            ci = CultureInfo.GetCultureInfo(lang);

                                CultureInfo.DefaultThreadCurrentCulture = ci;
                                CultureInfo.DefaultThreadCurrentUICulture = ci;
                                Log.Info($"Language set to {ci.DisplayName}.");
                                ev.Sender.RaMessage($"Language set to {ci.DisplayName}.", true);

                                Log.Debug($"Language test {Translation.GetString("workstationTakeDenied")}.");
                                Log.Debug($"Language test {Translation.GetText().doorDenied}.");
                            }
                            catch (Exception e)
                            {
                                ci = CultureInfo.GetCultureInfo("en");
                                Log.Error($"{lang} is not a valid language. Defaulting to English.");
                                Log.Error($"{e}");
                                ev.Sender.RaMessage($"{lang} is not a valid language.", false);
                            }

                            break;
                        }
                    }

                    break;
            }
        }
    }
}
