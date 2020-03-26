using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostSpectator
{
	public class GhostSettings
	{
		public int pos = 0;
		public Specmode specmode = Specmode.Normal;
		public bool specboard = false;

		public enum Specmode
		{
			Normal,
			Ghost
		}
	}
}
