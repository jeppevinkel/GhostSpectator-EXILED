using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace GhostSpectator.Patches
{
	//[HarmonyPatch(typeof(Scp096PlayerScript), nameof(Scp096PlayerScript.ProcessLooking))]
	//class ProcessLookingPatch
	//{
	//	[HarmonyPriority(Priority.First)]
	//	public static bool Prefix(Scp096PlayerScript __instance, Queue<GameObject> ____processLookingQueue)
	//	{
	//		Plugin.Log.Debug("ProcessLookingPatch");

	//		if (!____processLookingQueue.IsEmpty())
	//		{
	//			List<GameObject> gameObjects = ____processLookingQueue.ToList();

	//			gameObjects.RemoveAll(gO => Plugin.GhostList.Contains(gO.GetComponent<ReferenceHub>()));

	//			____processLookingQueue.Clear();

	//			foreach (GameObject gameObject in gameObjects)
	//			{
	//				____processLookingQueue.Enqueue(gameObject);
	//			}
	//		}

	//		return !Plugin.GhostList.Contains(__instance.GetComponent<ReferenceHub>());
	//	}
	//}
}
