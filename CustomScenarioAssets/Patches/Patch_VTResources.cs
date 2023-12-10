using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;
using VTOLVR.SteamWorkshop;

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

[HarmonyPatch(typeof(VTResources), "SaveCustomScenario")]
class Patch_VTResources_SaveCustomScenario
{
    [HarmonyPrefix]
    static void Postfix(VTScenario scenario, string fileName, string campaignID)
	{
		string text;
		if (string.IsNullOrEmpty(campaignID))
		{
			text = Path.Combine(VTResources.customScenariosDir, fileName);
		}
		else
		{
			text = Path.Combine(Path.Combine(VTResources.customCampaignsDir, campaignID), fileName);
		}

		VTSFileHelper.DeleteVTSFile(Path.Combine(text, fileName));
	}
}

[HarmonyPatch(typeof(VTResources), "UploadScenarioToSteamWorkshop")]
class Patch_VTResources_UploadScenarioToSteamWorkshop
{
    [HarmonyPrefix]
    static bool Prefix(VTScenario currentScenario, VTResources.RequestChangeNoteDelegate onRequestChangeNote, Action<WorkshopItemUpdate> onBeginUpdate, Action<WorkshopItemUpdateEventArgs> onComplete, bool autoSubscribe)
	{
		VTSFileHelper.DeleteVTSFile(Path.Combine(VTResources.GetScenarioDirectoryPath(currentScenario.scenarioID, currentScenario.campaignID), currentScenario.scenarioID));
        return true;
    }
}

public static class VTSFileHelper
{
    public static void DeleteVTSFile(string filepath)
    {
        if (File.Exists($"{filepath}.vts"))
        {
            File.Delete($"{filepath}.vts");
        }
    }
}