using Exiled.Events.EventArgs;
using System;
using MEC;

namespace GhostSpectator.Handlers
{
	internal class Scp914
	{
		internal void OnUpgradingItems(UpgradingItemsEventArgs ev)
		{
			ev.Players.RemoveAll(p => Plugin.GhostList.Contains(p));

			//foreach (Exiled.API.Features.Player ply in Plugin.GhostList)
			//{
			//	if (ev.Players.Contains(ply))
			//	{
			//		Timing.CallDelayed(0.2f, () =>
			//		{
			//			ply.ClearInventory();
			//			ply.AddItem(ItemType.Ammo762);
			//			ply.AddItem(ItemType.Ammo556);
			//			ply.AddItem(ItemType.Ammo9mm);
			//			ply.AddItem(ItemType.Flashlight);
			//		});
			//	}
			//}
		}
	}
}
