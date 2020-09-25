using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Exiled.Events.EventArgs;

namespace GhostSpectator
{
    public class CommandHandler
    {
	    public void OnRACommand(SendingRemoteAdminCommandEventArgs ev)
        {
	        switch (ev.Name)
            {
                case "gs_translation":
                    ev.IsAllowed = false;
                    switch (ev.Arguments[0])
                    {
                        case "reload":
	                        try
	                        {
		                        Plugin.Log.Info($"{ev.Sender.Nickname} has reloaded the translation files...");
		                        ev.ReplyMessage = "Reloading the translations...";
		                        ev.Success = true;

		                        Translation.Translation.LoadTranslations();
                            }
	                        catch (Exception e)
	                        {
		                        Plugin.Log.Error($"{e}");
		                        ev.ReplyMessage = ($"An error occured: {e}");
		                        ev.Success = false;
	                        }
                            
                            break;
                        case "set":
                        {
                            CultureInfo ci;
                            string lang = ev.Arguments[1].Replace("\"", "");
	                        try
                            {
                                
	                            ci = CultureInfo.GetCultureInfo(lang);

                                CultureInfo.DefaultThreadCurrentCulture = ci;
                                CultureInfo.DefaultThreadCurrentUICulture = ci;
                                Plugin.Log.Info($"Language set to {ci.DisplayName}.");
                                ev.ReplyMessage = ($"Language set to {ci.DisplayName}.");
                                ev.Success = true;

                                Plugin.Log.Debug($"Language test {Translation.Translation.GetString("workstationTakeDenied")}.");
                                Plugin.Log.Debug($"Language test {Translation.Translation.GetText().DoorDenied}.");
                            }
                            catch (Exception e)
                            {
                                ci = CultureInfo.GetCultureInfo("en");
                                Plugin.Log.Error($"{lang} is not a valid language. Defaulting to English.");
                                Plugin.Log.Error($"{e}");
                                ev.ReplyMessage = ($"{lang} is not a valid language.");
                                ev.Success = false;
                            }

                            break;
                        }
                    }

                    break;
            }
        }
    }
}
