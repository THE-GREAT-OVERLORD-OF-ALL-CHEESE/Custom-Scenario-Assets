using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
using Harmony;
using System.IO;
using VTNetworking;

public class CustomScenarioAssets : VTOLMOD
{
    public static CustomScenarioAssets instance;

    public List<AssetBundle> assetBundles;

    public Dictionary<string, VTStaticObject> customProps;

    public bool updatedAircraft;
    public Dictionary<string, UnitSpawn> customUnits;
    //public Dictionary<string, CustomUnitBase> customUnits;
    public Dictionary<string, UnitCatalogue.Unit> unitCatalogUnits;

    public Dictionary<string, HPEquippable> customEquips;

    //public Dictionary<string, CustomHPEquipBase> customHPEquips;//scrapped, no player weapons in csa for now

    public List<string> baseProps;
    public List<string> baseUnits;
    //public List<string> baseHPEquips;

    public List<string> uniqueMissingProps;
    public List<string> uniqueMissingUnits;
    //public List<string> uniqueMissingHPEquips;

    public override void ModLoaded()
    {
        HarmonyInstance harmony = HarmonyInstance.Create("cheese.customScenarioAssets");
        harmony.PatchAll(Assembly.GetExecutingAssembly());

        VTOLAPI.SceneLoaded += SceneLoaded;
        base.ModLoaded();

        instance = this;

        VTMapEdResources.LoadAll();
        GetBaseAssetLists();

        ReloadAssets();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
        {
            ReloadAssets();
        }
    }

    private void ReloadAssets()
    {
        if (assetBundles != null)
        {
            foreach (AssetBundle assetBundle in assetBundles)
            {
                assetBundle.Unload(false);
            }
        }
        assetBundles = new List<AssetBundle>();

        customProps = new Dictionary<string, VTStaticObject>();

        LoadAssetBundles();

        customUnits = new Dictionary<string, UnitSpawn>();
        unitCatalogUnits = new Dictionary<string, UnitCatalogue.Unit>();

        customEquips = new Dictionary<string, HPEquippable>();

        uniqueMissingProps = new List<string>();
        uniqueMissingUnits = new List<string>();

        updatedAircraft = false;
    }

    private void LoadAssetBundles()
    {
        Debug.Log("Searching for .csa files in the mod folder");
        string address = Directory.GetCurrentDirectory() + @"\VTOLVR_ModLoader\mods\";
        Debug.Log("Checking for: " + address);

        if (Directory.Exists(address))
        {
            Debug.Log(address + " exists!");
            DirectoryInfo info = new DirectoryInfo(address);

            Debug.Log("Searching " + address + info.Name + " for .csa");
            foreach (FileInfo file in info.GetFiles("*.csa"))
            {
                Debug.Log("Found " + file.FullName);
                StartCoroutine(LoadAssetBundle(file));
            }
            foreach (FileInfo file in info.GetFiles("*.csu"))
            {
                Debug.Log("Found " + file.FullName);
                StartCoroutine(LoadAssetBundle(file));
            }

            foreach (DirectoryInfo directory in info.GetDirectories())
            {
                Debug.Log("Searching " + address + directory.Name + " for .csa");
                foreach (FileInfo file in directory.GetFiles("*.csa"))
                {
                    Debug.Log("Found " + file.FullName);
                    StartCoroutine(LoadAssetBundle(file));
                }
                foreach (FileInfo file in directory.GetFiles("*.csu"))
                {
                    Debug.Log("Found " + file.FullName);
                    StartCoroutine(LoadAssetBundle(file));
                }
            }
        }
        else
        {
            Debug.Log(address + " doesn't exist.");
        }

        Debug.Log("Searching for .csa files in the project folders");
        string projectSettingsAdress = Directory.GetCurrentDirectory() + @"\VTOLVR_ModLoader\settings.json";
        LauncherSettings.LoadSettings(projectSettingsAdress);
        if (LauncherSettings.Settings != null)
        {
            string projectAdress = LauncherSettings.Settings.ProjectsFolder + @"\My Mods\";
            if (projectAdress != null)
            {
                Debug.Log("Checking for: " + projectAdress);

                if (Directory.Exists(projectAdress))
                {
                    Debug.Log(projectAdress + " exists!");
                    DirectoryInfo info = new DirectoryInfo(projectAdress);
                    foreach (DirectoryInfo directory in info.GetDirectories())
                    {
                        DirectoryInfo buildDir = new DirectoryInfo(directory.FullName + @"\Builds\");
                        if (Directory.Exists(buildDir.FullName))
                        {
                            Debug.Log("Searching " + directory.FullName + " for .csa");
                            foreach (FileInfo file in buildDir.GetFiles("*.csa"))
                            {
                                Debug.Log("Found " + file.FullName);
                                StartCoroutine(LoadAssetBundle(file));
                            }
                            foreach (FileInfo file in buildDir.GetFiles("*.csu"))
                            {
                                Debug.Log("Found " + file.FullName);
                                StartCoroutine(LoadAssetBundle(file));
                            }
                        }
                        else
                        {
                            Debug.Log(directory.FullName + @"\Builds\" + " does not exist");
                        }
                    }
                }
                else
                {
                    Debug.Log(projectAdress + " does not exist");
                }
            }
            else
            {
                Debug.Log("Project adress was null.");
            }
        }
        else
        {
            Debug.Log("Mod loader settings were null.");
        }
    }

    private IEnumerator LoadAssetBundle(FileInfo file)
    {
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(file.FullName);
        yield return request;

        if (request.assetBundle != null)
        {
            assetBundles.Add(request.assetBundle);

            UnityEngine.Object[] objects = request.assetBundle.LoadAllAssets(typeof(GameObject));
            foreach (UnityEngine.Object prefabObj in objects)
            {
                GameObject prefab = (GameObject)prefabObj;
                VTStaticObject prop = prefab.GetComponent<VTStaticObject>();
                UnitSpawn spawn = prefab.GetComponent<UnitSpawn>();
                HPEquippable equip = prefab.GetComponent<HPEquippable>();
                if (prop != null)
                {
                    AddCustomStaticProp(prop);
                    Debug.Log("Added " + prefab.name + " as a static object");
                }
                else if (spawn != null)
                {
                    AddCustomUnit(spawn);
                    Debug.Log("Added " + prefab.name + " as a custom unit");
                }
                else if (equip != null)
                {
                    AddCustomEquip(equip);
                    Debug.Log("Added " + prefab.name + " as a custom equip");
                }
                else
                {
                    SetAudioMixerGroup(prefab);
                }
            }
        }
        else
        {
            Debug.Log("Asset bundle was null");
        }
    }

    private void GetBaseAssetLists()
    {
        Debug.Log("Getting base game assets.");

        Debug.Log("Getting base game static object names.");
        baseProps = new List<string>();
        List<VTStaticObject> staticObjects = VTResources.GetAllStaticObjectPrefabs();
        foreach (VTStaticObject staticObject in staticObjects)
        {
            baseProps.Add(staticObject.name);
        }

        Debug.Log("Getting base game units.");
        UnitCatalogue.UpdateCatalogue();
        baseUnits = new List<string>();
        foreach (KeyValuePair<Teams, UnitCatalogue.UnitTeam> team in UnitCatalogue.catalogue)
        {
            foreach (UnitCatalogue.Unit unit in team.Value.allUnits)
            {
                baseUnits.Add(unit.prefabName);
            }
        }
    }

    private void SceneLoaded(VTOLScenes scene)
    {
        switch (scene)
        {
            case VTOLScenes.Akutan:
            case VTOLScenes.CustomMapBase:
            case VTOLScenes.CustomMapBase_OverCloud:
                StartCoroutine("SetupScene");
                break;
        }
    }

    private IEnumerator SetupScene()
    {
        while (VTMapManager.fetch == null || !VTMapManager.fetch.scenarioReady || FlightSceneManager.instance.switchingScene)
        {
            yield return null;
        }

        ShowMissingAssetsErrorGame();
    }

    public void AddCustomStaticProp(VTStaticObject prop)
    {
        if (customProps.ContainsKey(prop.name) == false)
        {
            Debug.Log("custom prop id: " + prop.name + " is unique, adding to custom prop list");
            customProps.Add(prop.name, prop);

            SetAudioMixerGroup(prop.gameObject);
            VTNetworkManager.RegisterOverrideResource($"csa/staticObjects/{prop.name}", prop.gameObject);
        }
        else
        {
            Debug.Log("A prop with the id: " + prop.name + " already exists.");
        }
    }

    public void AddCustomUnit(UnitSpawn unit)
    {
        if (customUnits.ContainsKey(unit.name) == false)
        {
            Debug.Log("custom unit id: " + unit.name + " is unique, adding to custom unit list");
            customUnits.Add(unit.name, unit);
            updatedAircraft = false;

            SetAudioMixerGroup(unit.gameObject);
            VTNetworkManager.RegisterOverrideResource($"csa/units/{unit.name}", unit.gameObject);
        }
        else
        {
            Debug.Log("A unit with the id: " + unit.name + " already exists.");
        }
    }

    public void AddCustomEquip(HPEquippable equip)
    {
        if (customEquips.ContainsKey(equip.name) == false)
        {
            Debug.Log("custom equip id: " + equip.name + " is unique, adding to custom equip list");
            customEquips.Add($"csa/{equip.name}", equip);
            updatedAircraft = false;

            SetAudioMixerGroup(equip.gameObject);
            VTNetworkManager.RegisterOverrideResource($"csa/{equip.name}", equip.gameObject);
        }
        else
        {
            Debug.Log("A equip with the id: " + equip.name + " already exists.");
        }
    }

    public void ReportMissingProp(string propID)
    {
        if (uniqueMissingProps.Contains(propID) == false)
        {
            Debug.Log("prop: " + propID + " is missing.");
            uniqueMissingProps.Add(propID);
        }
    }

    public void ReportMissingUnit(string unitID)
    {
        if (uniqueMissingUnits.Contains(unitID) == false)
        {
            Debug.Log("unit: " + unitID + " is missing.");
            uniqueMissingUnits.Add(unitID);
        }
    }

    public void ShowMissingAssetsErrorGame()
    {
        Debug.Log("Checking for missing assets.");
        if (uniqueMissingProps.Count > 0 || uniqueMissingUnits.Count > 0)
        {
            string warning = "Your game is missing assets needed for this scenario! It will not behave as intended. ";
            warning += "Please make sure all the required assets for this scenario are installed and loaded!\n";

            if (uniqueMissingProps.Count > 0)
            {
                warning += "Missing Static Objects: ";
                for (int i = 0; i < uniqueMissingProps.Count; i++)
                {
                    warning += uniqueMissingProps[i];
                    if (i < uniqueMissingProps.Count - 1)
                    {
                        warning += ",";
                    }
                    warning += "\n";
                }
            }

            if (uniqueMissingUnits.Count > 0)
            {
                warning += "Missing Units: ";
                for (int i = 0; i < uniqueMissingUnits.Count; i++)
                {
                    warning += uniqueMissingUnits[i];
                    if (i < uniqueMissingUnits.Count - 1)
                    {
                        warning += ",";
                    }
                    warning += "\n";
                }
            }

            warning += "\nThis message will disappear in 60 seconds.";
            TutorialLabel.instance.DisplayLabel(warning,
            null,
            60);
            Debug.Log(warning);

            uniqueMissingProps = new List<string>();
            uniqueMissingUnits = new List<string>();
        }
        else
        {
            Debug.Log("No assets are missing!");
        }
    }

    public void ShowMissingAssetsErrorEditor(VTScenarioEditor editor)
    {
        Debug.Log("Checking for missing assets.");
        if (uniqueMissingProps.Count > 0 || uniqueMissingUnits.Count > 0)
        {
            string warning = "You have loaded a scenario that contains assets you are missing. ";
            warning += "Please make sure all the required assets are installed and loaded!\n";

            if (uniqueMissingProps.Count > 0)
            {
                warning += "Missing Static Objects: ";
                for (int i = 0; i < uniqueMissingProps.Count; i++)
                {
                    warning += uniqueMissingProps[i];
                    if (i < uniqueMissingProps.Count - 1)
                    {
                        warning += ",";
                    }
                    warning += "\n";
                }
            }

            if (uniqueMissingUnits.Count > 0)
            {
                warning += "Missing Units: ";
                for (int i = 0; i < uniqueMissingUnits.Count; i++)
                {
                    warning += uniqueMissingUnits[i];
                    if (i < uniqueMissingUnits.Count - 1)
                    {
                        warning += ",";
                    }
                    warning += "\n";
                }
            }

            editor.confirmDialogue.DisplayConfirmation("Missing Assets!", warning, null, null);
            Debug.Log(warning);

            uniqueMissingProps = new List<string>();
            uniqueMissingUnits = new List<string>();
        }
        else
        {
            Debug.Log("No assets are missing!");
        }
    }

    public List<VTStaticObject> GenerateCustomVTStaticObjectsList()
    {
        Debug.Log("Generating VTStaticObjects Prefabs List");
        List<VTStaticObject> staticObjectPrefabs = new List<VTStaticObject>();

        if (customProps != null)
        {
            foreach (KeyValuePair<string, VTStaticObject> entry in customProps)
            {
                staticObjectPrefabs.Add(entry.Value);
            }
        }

        return staticObjectPrefabs;
    }

    public void SetAudioMixerGroup(GameObject prefab)
    {
        foreach (AudioSource source in prefab.GetComponentsInChildren<AudioSource>(true))
        {
            source.outputAudioMixerGroup = VTResources.GetExteriorMixerGroup();
        }
    }
}