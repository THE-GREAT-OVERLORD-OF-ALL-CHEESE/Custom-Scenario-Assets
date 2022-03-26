using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VTNetworking;

/*
[HarmonyPatch(typeof(VTNetworkManager), "GetInstantiatePrefab")]
class Patch_VTNetworkManager_GetInstantiatePrefab
{
    [HarmonyPrefix]
    static bool Prefix(out GameObject __result, string resourcePath)
    {
        CustomUnitBase unit;
        if (CustomScenarioAssets.instance.customUnits.TryGetValue(resourcePath, out unit))
        {
            __result = CustomScenarioAssets.instance.CreateTempCustomUnitPrefab(unit);
            return false;
        }
        __result = null;
        return true;
    }
}
*/