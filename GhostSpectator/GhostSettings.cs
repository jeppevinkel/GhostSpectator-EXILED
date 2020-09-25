using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostSpectator
{
	public class GhostSettings
	{
		public int Pos = 0;
		public Specmodes Specmode = Plugin.Instance.Config.DefaultSpecmode;
		public bool Specboard = true;

		public enum Specmodes
		{
			Normal,
			Ghost
		}
	}
}
