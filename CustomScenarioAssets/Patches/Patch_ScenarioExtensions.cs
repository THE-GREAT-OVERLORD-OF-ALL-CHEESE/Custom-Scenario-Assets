using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using UnityEngine;
using VTOLVR.Multiplayer;

#region VTResources

[HarmonyPatch(typeof(VTResources), nameof(VTResources.LoadScenariosFromDir))]
public class Patch_VTResources_LoadScenariosFromDir
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeInstructions = new List<CodeInstruction>(instructions);
        for (int i = 0; i < codeInstructions.Count; i++)
        {
            var codeInstruction = codeInstructions[i];
            
            if (codeInstruction.opcode == OpCodes.Ldstr && (string)codeInstruction.operand == ".vts")
            {
                codeInstructions[i + 1] = new CodeInstruction(OpCodes.Call, MethodUtils.EndsWithVTSNoBInfo());
            }
            if (codeInstruction.opcode == OpCodes.Ldstr && (string)codeInstruction.operand == ".vtsb")
            {
                codeInstructions[i + 1] = new CodeInstruction(OpCodes.Call, MethodUtils.EndsWithVTSBInfo());
            }

        }

        return codeInstructions.AsEnumerable();
    }
}
[HarmonyPatch(typeof(VTResources), nameof(VTResources.LoadScenariosFromDirAsync))]
public class Patch_VTResources_LoadScenariosFromDirAsync
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeInstructions = new List<CodeInstruction>(instructions);
        for (int i = 0; i < codeInstructions.Count; i++)
        {
            var codeInstruction = codeInstructions[i];
            
            if (codeInstruction.opcode == OpCodes.Ldstr && (string)codeInstruction.operand == ".vts")
            {
                codeInstructions[i + 1] = new CodeInstruction(OpCodes.Callvirt, MethodUtils.EndsWithVTSNoBInfo());
            }
            if (codeInstruction.opcode == OpCodes.Ldstr && (string)codeInstruction.operand == ".vtsb")
            {
                codeInstructions[i + 1] = new CodeInstruction(OpCodes.Callvirt, MethodUtils.EndsWithVTSBInfo());
            }
        }

        return codeInstructions.AsEnumerable();
    }
}

[HarmonyPatch(typeof(VTResources), nameof(VTResources.DeleteCustomScenario))]
public class Patch_VTResources_DeleteCustomScenario
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeInstructions = new List<CodeInstruction>(instructions);
        for (int i = 0; i < codeInstructions.Count; i++)
        {
            var codeInstruction = codeInstructions[i];
            
            if (codeInstruction.opcode == OpCodes.Ldstr)
            {
                // .(csa)vts
                if (((string)codeInstruction.operand == "*.vts"))
                    codeInstructions[i] = new CodeInstruction(OpCodes.Ldstr, "*.*vts");
                /*if (((string)codeInstruction.operand == "*.vtm"))
                    codeInstructions[i] = new CodeInstruction(OpCodes.Ldstr, "*.*vtm");*/
            }
        }

        return codeInstructions.AsEnumerable();
    }
}

[HarmonyPatch(typeof(VTResources), nameof(VTResources.LoadWorkshopSingleScenario))]
public class Patch_VTResources_LoadWorkshopSingleScenario
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeInstructions = new List<CodeInstruction>(instructions);
        for (int i = 0; i < codeInstructions.Count; i++)
        {
            var codeInstruction = codeInstructions[i];
            
            if (codeInstruction.opcode == OpCodes.Ldstr)
            {
                // .(csa)vts(b)
                if (((string)codeInstruction.operand == "*.vts*"))
                    codeInstructions[i] = new CodeInstruction(OpCodes.Ldstr, "*.*vts*");
                if (((string)codeInstruction.operand == ".vts"))
                    codeInstructions[i + 1] = new CodeInstruction(OpCodes.Callvirt, MethodUtils.EndsWithVTSInfo());
            } 
        }

        return codeInstructions.AsEnumerable();
    }
}

[HarmonyPatch(typeof(VTResources), nameof(VTResources.ReloadCustomScenario))]
public class Patch_VTResources_ReloadCustomScenario
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeInstructions = new List<CodeInstruction>(instructions);
        for (int i = 0; i < codeInstructions.Count; i++)
        {
            var codeInstruction = codeInstructions[i];
            
            if (codeInstruction.opcode == OpCodes.Ldstr)
            {
                if (((string)codeInstruction.operand == ".vts"))
                {
                    codeInstructions[i + 2] = new CodeInstruction(OpCodes.Call, MethodUtils.CombineVTSNoBCheckInfo());
                }
            } 
        }

        return codeInstructions.AsEnumerable();
    }
}

[HarmonyPatch(typeof(VTResources), "RepairAllScenarioFilePaths")]
public class Patch_VTResources_RepairAllScenarioFilePaths
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeInstructions = new List<CodeInstruction>(instructions);
        for (int i = 0; i < codeInstructions.Count; i++)
        {
            var codeInstruction = codeInstructions[i];
            
            if (codeInstruction.opcode == OpCodes.Ldstr)
            {
                // .(csa)vts
                if (((string)codeInstruction.operand == "*.vts"))
                    codeInstructions[i] = new CodeInstruction(OpCodes.Ldstr, "*.*vts");
            } 
        }

        return codeInstructions.AsEnumerable();
    }
}

[HarmonyPatch(typeof(VTResources), "RepairScenarioFilePath")]
public class Patch_VTResources_RepairScenarioFilePath
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeInstructions = new List<CodeInstruction>(instructions);
        for (int i = 0; i < codeInstructions.Count; i++)
        {
            var codeInstruction = codeInstructions[i];
            
            if (codeInstruction.opcode == OpCodes.Ldstr)
            {
                // .(csa)vts
                if (((string)codeInstruction.operand == "*.vts"))
                    codeInstructions[i] = new CodeInstruction(OpCodes.Ldstr, "*.*vts");
                if (((string)codeInstruction.operand == ".vts"))
                    codeInstructions[i + 2] = new CodeInstruction(OpCodes.Call, MethodUtils.CombineVTSNoBInfo());
            } 
        }

        return codeInstructions.AsEnumerable();
    }
}

[HarmonyPatch(typeof(VTResources), nameof(VTResources.SaveCustomScenario))]
public class Patch_VTResources_SaveCustomScenario
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var codeInstruction in instructions)
        {
            // .(csa)vts
            if (codeInstruction.opcode == OpCodes.Ldstr && (string)codeInstruction.operand == ".vts")
            {
                yield return new CodeInstruction(OpCodes.Ldstr, $".{CustomScenarioAssets.FileExtension}vts");
            }
            else yield return codeInstruction;
        }
    }
}
#endregion

#region Workshop

[HarmonyPatch(typeof(MPMissionBrowser.BrowserCampaignInfo), nameof(MPMissionBrowser.BrowserCampaignInfo.LoadWsRoutine))]
public class Patch_MPMissionBrowser_BrowserCampaignInfo_LoadWsRoutine
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeInstructions = new List<CodeInstruction>(instructions);
        for (int i = 0; i < codeInstructions.Count; i++)
        {
            var codeInstruction = codeInstructions[i];
            
            if (codeInstruction.opcode == OpCodes.Ldstr && (string)codeInstruction.operand == "*.vtsb")
            {
                // .(csa)vts(b)
                codeInstructions[i] = new CodeInstruction(OpCodes.Ldstr, "*.*vts*");
            }
        }

        return codeInstructions.AsEnumerable();
    }
}

[HarmonyPatch(typeof(VTSteamWorkshopUtils), nameof(VTSteamWorkshopUtils.EncodeAllWorkshopConfigs))]
public class Patch_VTSteamWorkshopUtils_EncodeAllWorkshopConfigs
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeInstructions = new List<CodeInstruction>(instructions);
        for (int i = 0; i < codeInstructions.Count; i++)
        {
            var codeInstruction = codeInstructions[i];
            
            if (codeInstruction.opcode == OpCodes.Ldstr)
            {
                if ((string)codeInstruction.operand == ".vts")
                    codeInstructions[i + 1] = new CodeInstruction(OpCodes.Call, MethodUtils.EqualsVTSNoBInfo());
                /*if ((string)codeInstruction.operand == ".vtm")
                    codeInstructions[i + 1] = new CodeInstruction(OpCodes.Call, MethodUtils.EqualsVTMNoBInfo());*/
            }
        }

        return codeInstructions.AsEnumerable();
    }
}

#endregion

#region VTEditorAndMisc

[HarmonyPatch(typeof(VTCampaignInfo), nameof(VTCampaignInfo.RepairCampaignFileStructure))]
public class Patch_VTCampaignInfo_RepairCampaignFileStructure
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeInstructions = new List<CodeInstruction>(instructions);
        for (int i = 0; i < codeInstructions.Count; i++)
        {
            var codeInstruction = codeInstructions[i];
            
            if (codeInstruction.opcode == OpCodes.Ldstr )
            {
                // Maps
                
                /*if ((string)codeInstruction.operand == ".vtm")
                    codeInstructions[i + 1] = new CodeInstruction(OpCodes.Call, MethodUtils.EqualsVTMNoBInfo());
                if ((string)codeInstruction.operand == ".vtmb")
                    codeInstructions[i + 1] = new CodeInstruction(OpCodes.Call, MethodUtils.EqualsVTMBInfo());*/
                
                // Scenarios
                
                if ((string)codeInstruction.operand == "*.vts*")
                    codeInstructions[i] = new CodeInstruction(OpCodes.Ldstr, "*.*vts*");
                if ((string)codeInstruction.operand == ".vts")
                {
                    if ((MethodInfo)codeInstructions[i + 1].operand == AccessTools.Method(typeof(String), nameof(String.EndsWith), new []{ typeof(string) }))
                        codeInstructions[i + 1] = new CodeInstruction(OpCodes.Callvirt, MethodUtils.EndsWithVTSNoBInfo());
                    else 
                    if ((MethodInfo)codeInstructions[i + 1].operand == AccessTools.Method(typeof(String), nameof(String.Concat), new []{ typeof(string), typeof(string) }))
                        codeInstructions[i] = new CodeInstruction(OpCodes.Ldloc_S, 25);
                    else
                        codeInstructions[i + 1] = new CodeInstruction(OpCodes.Call, MethodUtils.EqualsVTSNoBInfo());
                }
                if ((string)codeInstruction.operand == ".vtsb")
                {
                    if ((MethodInfo)codeInstructions[i + 1].operand == AccessTools.Method(typeof(String),
                            nameof(String.EndsWith), new[] { typeof(string) }))
                        codeInstructions[i + 1] = new CodeInstruction(OpCodes.Callvirt, MethodUtils.EndsWithVTSBInfo());
                    else
                        codeInstructions[i + 1] = new CodeInstruction(OpCodes.Callvirt, MethodUtils.EqualsVTSBInfo());
                }
            }
        }

        return codeInstructions.AsEnumerable();
    }
}

[HarmonyPatch(typeof(VTScenarioInfo), "ReadConfigNode")]
public class Patch_VTScenarioInfo_ReadConfigNode
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeInstructions = new List<CodeInstruction>(instructions);
        for (int i = 0; i < codeInstructions.Count; i++)
        {
            var codeInstruction = codeInstructions[i];
            
            if (codeInstruction.opcode == OpCodes.Ldstr)
            {
                // .(csa)vts(b)
                if (((string)codeInstruction.operand == ".vts"))
                    codeInstructions[i + 1] = new CodeInstruction(OpCodes.Callvirt, MethodUtils.EndsWithVTSNoBInfo());
                if (((string)codeInstruction.operand == ".vtsb"))
                    codeInstructions[i + 1] = new CodeInstruction(OpCodes.Callvirt, MethodUtils.EndsWithVTSBInfo());
            } 
        }

        return codeInstructions.AsEnumerable();
    }
}

[HarmonyPatch(typeof(VTEdCampaignEditWindow), nameof(VTEdCampaignEditWindow.OnImportedMission))]
public class Patch_VTEdCampaignEditWindow_OnImportedMission
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeInstructions = new List<CodeInstruction>(instructions);
        for (int i = 0; i < codeInstructions.Count; i++)
        {
            var codeInstruction = codeInstructions[i];
            
            if (codeInstruction.opcode == OpCodes.Ldstr)
            {
                // .(csa)vts(b)
                if (((string)codeInstruction.operand == ".vts"))
                    codeInstructions[i + 2] = new CodeInstruction(OpCodes.Call, MethodUtils.CombineVTSNoBCheckInfo());
                /*if (((string)codeInstruction.operand == "*.vtm"))
                    codeInstructions[i] = new CodeInstruction(OpCodes.Ldstr, "*.*vtm");
                if (((string)codeInstruction.operand == ".vtm"))
                    codeInstructions[i + 2] = new CodeInstruction(OpCodes.Call, MethodUtils.CombineVTMNoBCheckInfo());*/
            } 
        }

        return codeInstructions.AsEnumerable();
    }
}

[HarmonyPatch(typeof(VTEdNewMissionMenu), nameof(VTEdNewMissionMenu.Save))]
public class Patch_VTEdNewMissionMenu_Save
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeInstructions = new List<CodeInstruction>(instructions);
        for (int i = 0; i < codeInstructions.Count; i++)
        {
            var codeInstruction = codeInstructions[i];
            
            if (codeInstruction.opcode == OpCodes.Ldstr)
            {
                // .(csa)vts(b)
                if (((string)codeInstruction.operand == ".vts"))
                    codeInstructions[i + 2] = new CodeInstruction(OpCodes.Call, MethodUtils.CombineVTSNoBInfo());
                /*if (((string)codeInstruction.operand == ".vtm"))
                    codeInstructions[i + 2] = new CodeInstruction(OpCodes.Call, MethodUtils.CombineVTMNoBInfo());*/
            } 
        }

        return codeInstructions.AsEnumerable();
    }
}

[HarmonyPatch(typeof(VTScenarioEditor), nameof(VTScenarioEditor.SaveToNewName))]
public class Patch_VTEdNewMissionMenu_SaveToNewName
{
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codeInstructions = new List<CodeInstruction>(instructions);
        bool first = true;
        for (int i = 0; i < codeInstructions.Count; i++)
        {
            var codeInstruction = codeInstructions[i];
            
            if (codeInstruction.opcode == OpCodes.Ldstr)
            {
                if (((string)codeInstruction.operand == ".vts"))
                {
                    if (first)
                    {
                        codeInstructions[i + 2] =
                            new CodeInstruction(OpCodes.Call, MethodUtils.CombineVTSNoBCheckInfo());
                        first = false;
                    }
                    else
                        codeInstructions[i] = new CodeInstruction(OpCodes.Ldstr, $".{CustomScenarioAssets.FileExtension}vts");
                }
            } 
        }

        return codeInstructions.AsEnumerable();
    }
}

#endregion