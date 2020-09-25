using Exiled.API.Interfaces;

namespace GhostSpectator
{
	public class Config : IConfig
	{
		public bool IsEnabled { get; set; } = true;

		public bool AllowDamage { get; set; } = false;

		public bool AllowPickup { get; set; } = false;
		
		public bool DebugMode { get; set; } = false;

		public bool GhostGod { get; set; } = true;
		
		public bool GhostInteract { get; set; } = false;
		
		public bool GhostRagdoll { get; set; } = false;
		
		public bool GhostNoclip { get; set; } = true;
		
		public bool GhostSpectatorVoiceChat { get; set; } = false;

		public GhostSettings.Specmodes DefaultSpecmode { get; set; } = GhostSettings.Specmodes.Normal;

		public string GhostMessage { get; set; } = 
			"You have been spawned as a spectator ghost.\n" +
			"Drop your <color=#ff0000>7.62</color> to be <color=#ff0000>teleported</color> to the <color=#ff0000>next</color> player\n" +
			"Drop your <color=#ff0000>5.56</color> to be <color=#ff0000>teleported</color> to the <color=#ff0000>previous</color> player";

		public string SpecMessage { get; set; } = 
			"This server is using <color=#ff0000>GhostSpectator</color>\n" +
			"To enable ghost mode, open your console and type <color=#ff0000>.specmode</color>";

		public string Lang { get; set; } = "en-US".Replace("\"", "");

		public float RateLimitTime { get; set; } = 3;
	}
}
