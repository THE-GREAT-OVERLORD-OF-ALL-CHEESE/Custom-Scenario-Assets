using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;

[HarmonyPatch(typeof(VTScenarioEditor), "UploadToSteamWorkshop")]
class Patch_VTScenarioEditor_UploadToSteamWorkshop
{
    [HarmonyPrefix]
    static bool Prefix(VTScenarioEditor __instance)
    {
        if (Input.GetKey(KeyCode.F) && Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.X)) {
            return true;
        }
        else {
            __instance.confirmDialogue.DisplayConfirmation("I'm sorry dave", "I can't let you do that yet. The game doesn't have any fail safes for if it loads a scenario with an object it doesn't know and it softlocks peoples games. This will probably be fixed in an update.", null, null);
            return false;
        }
    }
}

[HarmonyPatch(typeof(VTScenarioEditor), "LoadScenario")]
class Patch_VTScenarioEditor_LoadScenario
{
    [HarmonyPostfix]
    static void Postfix(VTScenarioEditor __instance)
    {
        CustomScenarioAssets.instance.ShowMissingAssetsErrorEditor(__instance);
    }
}