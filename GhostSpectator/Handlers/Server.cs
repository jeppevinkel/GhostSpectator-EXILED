using Exiled.Events.EventArgs;
using System;

namespace GhostSpectator.Handlers
{
	internal class Server
	{
		internal void OnWaitingForPlayers()
		{
			Plugin.GhostList.Clear();
		}

		internal void OnRestartingRound()
		{
			Plugin.GhostList.Clear();
		}

		internal void OnRespawningTeam(RespawningTeamEventArgs ev)
		{
			Plugin.Log.Debug($"Ghost spectators added to respawn list.");
			//ev.ToRespawn.AddRange(Plugin.GhostList);
		}
	}
}
