using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;

[HarmonyPatch(typeof(VTResources), "GetAllStaticObjectPrefabs")]
class Patch_VTResources_GetAllStaticObjectPrefabs
{
    [HarmonyPostfix]
    static void Postfix(ref List<VTStaticObject> __result)
    {
        Debug.Log("Adding Custom Assets");

        List<VTStaticObject> objs = CustomScenarioAssets.instance.GenerateCustomVTStaticObjectsList();

        __result.AddRange(objs);
    }
}

[HarmonyPatch(typeof(VTResources), "GetStaticObjectPrefab")]
class Patch_VTResources_GetStaticObjectPrefab
{
    [HarmonyPrefix]
    static bool Prefix(out GameObject __result, string id)
    {
        VTStaticObject prop;
        if (CustomScenarioAssets.instance.customProps.TryGetValue(id, out prop)) {
            __result = prop.gameObject;
            return false;
        }
        if (CustomScenarioAssets.instance.baseProps.Contains(id))
        {
            __result = null;
            return true;
        }
        else
        {
            CustomScenarioAssets.instance.ReportMissingProp(id);
            __result = CustomScenarioAssets.instance.customProps["cheese_failsafeobject"].gameObject;
            return false;
        }
    }
}