using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Harmony;

public class MethodUtils
{

    #region VTS
    
    public static MethodInfo EndsWithVTSInfo() => AccessTools.Method(typeof(MethodUtils),
        nameof(EndsWithVTS), new[] { typeof(string), typeof(string) });
    
    public static MethodInfo EndsWithVTSNoBInfo() => AccessTools.Method(typeof(MethodUtils),
        nameof(EndsWithVTSNoB), new[] { typeof(string), typeof(string) });
    
    public static MethodInfo EndsWithVTSBInfo() => AccessTools.Method(typeof(MethodUtils),
        nameof(EndsWithVTSB), new[] { typeof(string), typeof(string) });
    
    
    
    public static MethodInfo EqualsVTSInfo() => AccessTools.Method(typeof(MethodUtils),
        nameof(EqualsVTS), new[] { typeof(string), typeof(string) });
    
    public static MethodInfo EqualsVTSNoBInfo() => AccessTools.Method(typeof(MethodUtils),
        nameof(EqualsVTSNoB), new[] { typeof(string), typeof(string) });

    public static MethodInfo EqualsVTSBInfo() => AccessTools.Method(typeof(MethodUtils),
        nameof(EqualsVTSB), new[] { typeof(string), typeof(string) });
    
    public static MethodInfo CombineVTSNoBCheckInfo() => AccessTools.Method(typeof(MethodUtils),
        nameof(CombineVTSNoBCheck), new[] { typeof(string), typeof(string) });
    
    public static MethodInfo CombineVTSNoBInfo() => AccessTools.Method(typeof(MethodUtils),
        nameof(CombineVTSNoB), new[] { typeof(string), typeof(string) });
    
    public static MethodInfo ModdedVTSNoBInfo() => AccessTools.Method(typeof(MethodUtils),
        nameof(ModdedVTSNoB), new[] { typeof(VTScenario) });
    
    public static MethodInfo ModdedVTSBInfo() => AccessTools.Method(typeof(MethodUtils),
        nameof(ModdedVTSB), new[] { typeof(VTScenario) });
    
    public static MethodInfo ModdedEditorVTSNoBInfo() => AccessTools.Method(typeof(MethodUtils),
        nameof(ModdedEditorVTSNoB), new[] { typeof(VTScenarioEditor) });
    
    public static MethodInfo ModdedEditorVTSBInfo() => AccessTools.Method(typeof(MethodUtils),
        nameof(ModdedEditorVTSB), new[] { typeof(VTScenarioEditor) });
    
    
    
    public static bool EndsWithVTS(string path, string _)
    {
        // Regex checking if a vts file is any of the following. () is optional stuff
        // .(csa)vts(b)
        string pattern = $@"(\.({CustomScenarioAssets.FileExtension})?vtsb?)$";
        return Regex.IsMatch(path, pattern);
    }

    public static bool EndsWithVTSNoB(string path, string _)
    {
        // Regex checking if a vts file is any of the following. () is optional stuff
        // .(csa)vts
        string pattern = $@"(\.({CustomScenarioAssets.FileExtension})?vts)$";
        return Regex.IsMatch(path, pattern);
    }

    public static bool EndsWithVTSB(string path, string _)
    {
        // Regex checking if a vts file is any of the following. () is optional stuff
        // .(csa)vtsb
        string pattern = $@"(\.({CustomScenarioAssets.FileExtension})?vtsb)$";
        return Regex.IsMatch(path, pattern);
    }
    

    public static bool EqualsVTS(string text, string text2)
    {
        // .(csa)vts(b)
        string pattern = $@"(\.({CustomScenarioAssets.FileExtension})?vtsb?)$";
        return Regex.IsMatch(text, pattern);
    }

    public static bool EqualsVTSNoB(string text, string text2)
    {
        // .(csa)vts
        string pattern = $@"(\.({CustomScenarioAssets.FileExtension})?vts)$";
        return Regex.IsMatch(text, pattern);
    }
    
    public static bool EqualsVTSB(string text, string text2)
    {
        // .(csa)vtsb
        string pattern = $@"(\.({CustomScenarioAssets.FileExtension})?vtsb)$";
        return Regex.IsMatch(text, pattern);
    }
    
    public static string CombineVTSNoBCheck(string text, string text2)
    {
        // .(csa)vts
        var dir = new DirectoryInfo(text);
        if (dir != null)
        {
            foreach (var file in dir.GetFiles("*.*vts"))
            {
                if (file.Extension == $".{CustomScenarioAssets.FileExtension}vts")
                    return file.FullName;
            }
        }

        return Path.Combine(text, text2);
    }
    
    public static string CombineVTSNoB(string text, string text2)
    {
        text2 = text2.Replace(".vts", $".{CustomScenarioAssets.FileExtension}vts");

        return Path.Combine(text, text2);
    }

    public static string ModdedVTSNoB(VTScenario scenario)
    {
        return IsVTScenarioModded(scenario) ? $".{CustomScenarioAssets.FileExtension}vts" : ".vts";
    }
    
    public static string ModdedVTSB(VTScenario scenario)
    {
        return IsVTScenarioModded(scenario) ? $".{CustomScenarioAssets.FileExtension}vtsb" : ".vtsb";
    }
    
    public static string ModdedEditorVTSNoB(VTScenarioEditor scenario)
    {
        return IsVTScenarioModded(scenario.currentScenario) ? $".{CustomScenarioAssets.FileExtension}vts" : ".vts";
    }
    
    public static string ModdedEditorVTSB(VTScenarioEditor scenario)
    {
        return IsVTScenarioModded(scenario.currentScenario) ? $".{CustomScenarioAssets.FileExtension}vtsb" : ".vtsb";
    }


    public static bool IsVTScenarioModded(VTScenario scenario)
    {
        if (scenario.units.units.Any(e => CustomScenarioAssets.instance.customUnits.ContainsKey(e.Value.prefabUnitSpawn.name)))
        {
            return true;
        }

        if (scenario.staticObjects.GetAllObjects()
            .Any(e => CustomScenarioAssets.instance.customProps.ContainsKey(e.name)))
        {
            return true;
        }

        return false;
    }
    
    #endregion

    #region VTM
    
    public static MethodInfo EqualsVTMInfo() => AccessTools.Method(typeof(MethodUtils),
        nameof(EqualsVTM), new[] { typeof(string), typeof(string) });
    
    public static MethodInfo EqualsVTMNoBInfo() => AccessTools.Method(typeof(MethodUtils),
        nameof(EqualsVTMNoB), new[] { typeof(string), typeof(string) });
    
    public static MethodInfo EqualsVTMBInfo() => AccessTools.Method(typeof(MethodUtils),
        nameof(EqualsVTMB), new[] { typeof(string), typeof(string) });
    
    public static MethodInfo CombineVTMNoBCheckInfo() => AccessTools.Method(typeof(MethodUtils),
        nameof(CombineVTMNoBCheck), new[] { typeof(String), typeof(String) });
    
    public static MethodInfo CombineVTMNoBInfo() => AccessTools.Method(typeof(MethodUtils),
        nameof(CombineVTMNoB), new[] { typeof(String), typeof(String) });
    
    
    public static bool EqualsVTM(string text, string text2)
    {
        // .(csa)vtm(b)
        string pattern = $@"(\.({CustomScenarioAssets.FileExtension})?vtmb?)$";
        return Regex.IsMatch(text, pattern);
    }

    public static bool EqualsVTMNoB(string text, string text2)
    {
        // .(csa)vtm
        string pattern = $@"(\.({CustomScenarioAssets.FileExtension})?vtm)$";
        return Regex.IsMatch(text, pattern);
    }

    public static bool EqualsVTMB(string text, string text2)
    {
        // .(csa)vtm
        string pattern = $@"(\.({CustomScenarioAssets.FileExtension})?vtmb)$";
        return Regex.IsMatch(text, pattern);
    }
    
    public static string CombineVTMNoBCheck(string text, string text2)
    {
        // .(csa)vts
        var dir = new DirectoryInfo(text);
        if (dir != null)
        {
            foreach (var file in dir.GetFiles("*.*vtm"))
            {
                if (file.Extension == $".{CustomScenarioAssets.FileExtension}vtm")
                    return file.FullName;
            }
        }
        return Path.Combine(text, text2);
    }
    
    public static string CombineVTMNoB(string text, string text2)
    {
        text2 = text2.Replace(".vtm", $".{CustomScenarioAssets.FileExtension}vtm");

        return Path.Combine(text, text2);
    }
    
    #endregion
}