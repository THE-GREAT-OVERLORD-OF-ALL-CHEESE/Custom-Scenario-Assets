using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;

/*[HarmonyPatch(typeof(Actor), "Awake")]
class Patch_Actor_Awake
{
    [HarmonyPrefix]
    static bool Prefix(Actor __instance)
    {
		Health h = null;
		if (!h)
		{
			h = __instance.GetComponent<Health>();
			Debug.Log("Found health");
		}
		if (h)
		{
			//h.OnDeath.AddListener(new UnityAction(__instance.H_OnDeath));
			Debug.Log("Added listener");
		}
		return true;
	}
}*/

/*
[HarmonyPatch(typeof(AIUnitSpawn), "IsNonTarget")]
class Patch_AIUnitSpawn_IsNonTarget
{
    [HarmonyPrefix]
    static bool Prefix(AIUnitSpawn __instance, ref bool __result)
    {
		__result = true;
		if (__instance.actor == null)
		{
			Debug.Log("Actor was null, assigning");
			__instance.actor = __instance.GetComponent<Actor>();
			return false;
		}
		return true;
	}
}
*/