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

public class CustomScenarioAssets : VTOLMOD
{
    public static CustomScenarioAssets instance;

    public Dictionary<string, CustomStaticPropBase> customProps;

    public bool updatedAircraft;
    public Dictionary<string, CustomUnitBase> customUnits;
    public Dictionary<string, UnitCatalogue.Unit> unitCatalogUnits;

    public List<string> baseProps;
    public List<string> baseUnits;

    public List<string> uniqueMissingProps;
    public List<string> uniqueMissingUnits;

    public CustomStaticPropBase failSafeProp;

    public GameObject firePrefab;
    public GameObject largeFirePrefab;
    public AudioClip cannonClip;

    public override void ModLoaded()
    {
        HarmonyInstance harmony = HarmonyInstance.Create("cheese.customScenarioAssets");
        harmony.PatchAll(Assembly.GetExecutingAssembly());

        VTOLAPI.SceneLoaded += SceneLoaded;
        base.ModLoaded();

        instance = this;

        VTMapEdResources.LoadAll();
        GetBaseAssetLists();

        customProps = new Dictionary<string, CustomStaticPropBase>();

        AddCustomStaticProp(new CustomStaticProp_ExampleCube("Cheeses Example Props", "cheese_1m_cube", "1m Cube", UnitSpawn.PlacementModes.Any, false, 1));
        AddCustomStaticProp(new CustomStaticProp_ExampleCube("Cheeses Example Props", "cheese_10m_cube", "10m Cube", UnitSpawn.PlacementModes.Any, false, 10));
        AddCustomStaticProp(new CustomStaticProp_ExampleCube("Cheeses Example Props", "cheese_100m_cube", "100m Cube", UnitSpawn.PlacementModes.Any, false, 100));
        AddCustomStaticProp(new CustomStaticProp_ExampleCube("Cheeses Example Props", "cheese_1000m_cube", "1000m Cube", UnitSpawn.PlacementModes.Any, false, 1000));

        AddCustomStaticProp(new CustomStaticProp_CarrierCatapult("Carrier Accessories", "cheese_carrier_catapult", "Catapult", UnitSpawn.PlacementModes.Any, true));
        AddCustomStaticProp(new CustomStaticProp_CarrierArrestor("Carrier Accessories", "cheese_carrier_arrestor", "Arrestor", UnitSpawn.PlacementModes.Any, true));

        AddCustomStaticProp(new CustomStaticProp_ATCTower("Airport Parts", "cheese_airport_tower", "Tower", UnitSpawn.PlacementModes.Any, true));
        AddCustomStaticProp(new CustomStaticProp_AirportTentHangar("Airport Parts", "cheese_airport_tentHangar", "Tent Hangar", UnitSpawn.PlacementModes.Any, true));
        AddCustomStaticProp(new CustomStaticProp_AirportJumboHangar("Airport Parts", "cheese_airport_jumboHangar", "Jumbo Hangar", UnitSpawn.PlacementModes.Any, true));
        
        failSafeProp = new CustomStaticProp_FailSafe("Fail Safe", "cheese_failsafeobject", "Missing Asset", UnitSpawn.PlacementModes.Any, true, false);
        AddCustomStaticProp(failSafeProp);

        LoadAssetBundles();

        customUnits = new Dictionary<string, CustomUnitBase>();
        unitCatalogUnits = new Dictionary<string, UnitCatalogue.Unit>();

        AddCustomUnit(new CustomUnitBase(Teams.Allied, "Test Category", "cheese_testunit", "Test Unit", "This is an example test unit to test adding custom units!", UnitSpawn.PlacementModes.Any, true));

        AddCustomUnit(new CustomUnit_AngryCube(Teams.Allied, "Test Category", "cheese_angrycube_allied", "Angry Cube", "This is an angry cube to test custom VT events and behaviours! It can't do anything intersting right now.", UnitSpawn.PlacementModes.Any, true));
        AddCustomUnit(new CustomUnit_AngryCube(Teams.Enemy, "Test Category", "cheese_angrycube_enemy", "Angry Cube", "This is an angry cube to test custom VT events and behaviours! It can't do anything intersting right now.", UnitSpawn.PlacementModes.Any, true));

        AddCustomUnit(new CustomUnit_ExampleAAGun(Teams.Allied, "Test Category", "cheese_example_aabofors_allied", "Example AA Gun", "This is an example anti-aircraft gun vaugly based on a Bofors 40mm gun", UnitSpawn.PlacementModes.Any, true));
        AddCustomUnit(new CustomUnit_ExampleAAGun(Teams.Enemy, "Test Category", "cheese_example_aabofors_enemy", "Example AA Gun", "This is an example anti-aircraft gun vaugly based on a Bofors 40mm gun", UnitSpawn.PlacementModes.Any, true));

        uniqueMissingProps = new List<string>();
        uniqueMissingUnits = new List<string>();

        GetFirePrefabs();
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

            foreach (DirectoryInfo directory in info.GetDirectories())
            {
                Debug.Log("Searching " + address + directory.Name + " for .csa");
                foreach (FileInfo file in directory.GetFiles("*.csa")) {
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
                        if (Directory.Exists(buildDir.FullName)) {
                            Debug.Log("Searching " + directory.FullName + " for .csa");
                            foreach (FileInfo file in buildDir.GetFiles("*.csa"))
                            {
                                Debug.Log("Found " + file.FullName);
                                StartCoroutine(LoadAssetBundle(file));
                            }
                        }
                        else {
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
            UnityEngine.Object[] objects = request.assetBundle.LoadAllAssets(typeof(GameObject));
            foreach (UnityEngine.Object prefabObj in objects)
            {
                GameObject prefab = (GameObject)prefabObj;
                UnitSpawn spawn = prefab.GetComponent<UnitSpawn>();
                if (spawn == null) {
                    AddCustomStaticProp(new CustomStaticProp_AssetBundle(file.Name, prefab.name, prefab.name, UnitSpawn.PlacementModes.Any, true, prefab));
                    Debug.Log("Added " + prefab.name + " as a static object");
                }
                else {
                    AddCustomUnit(new CustomUnit_AssetBundle(spawn.actor.team, spawn.category, prefab.name, spawn.unitName, spawn.unitDescription, UnitSpawn.PlacementModes.Any, true, prefab));
                    Debug.Log("Added " + prefab.name + " as a custom unit");
                }
            }
        }
        else {
            Debug.Log("Asset bundle was null");
        }
    }

    private void GetBaseAssetLists() {
        Debug.Log("Getting base game assets.");

        Debug.Log("Getting base game static object names.");
        baseProps = new List<string>();
        List<VTStaticObject> staticObjects = VTResources.GetAllStaticObjectPrefabs();
        foreach (VTStaticObject staticObject in staticObjects) {
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

    public void AddCustomStaticProp(CustomStaticPropBase prop) {
        if (customProps.ContainsKey(prop.objectID) == false) {
            Debug.Log("custom prop id: " + prop.objectID + " is unique, adding to custom prop list");
            customProps.Add(prop.objectID, prop);
            updatedAircraft = false;
        }
        else {
            Debug.Log("A prop with the id: " + prop.objectID + " already exists.");
        }
    }

    public List<VTStaticObject> GenerateFakeVTPrefabs()
    {
        Debug.Log("Generating fake VT Prefabs");
        List<VTStaticObject> staticObjectPrefabs = new List<VTStaticObject>();

        if (customProps != null)
        {
            foreach (KeyValuePair<string, CustomStaticPropBase> entry in customProps)
            {
                Debug.Log("Generating " + entry.Key);
                GameObject temp = entry.Value.Spawn();
                if (temp != null)
                {
                    temp.name = entry.Value.objectID;
                    VTStaticObject staticObject = temp.AddComponent<VTStaticObject>();
                    staticObject.category = entry.Value.category;
                    staticObject.alignToSurface = entry.Value.alignToSurface;
                    staticObject.objectName = entry.Value.objectName;
                    staticObject.placementMode = entry.Value.placementMode;
                    staticObject.spawnObjects = new List<VTStaticObject.SpawnObject>();
                    staticObject.editorOnly = entry.Value.hidden;

                    temp.transform.position = Vector3.down * 30000;

                    staticObjectPrefabs.Add(staticObject);
                }
                else
                {
                    Debug.Log(entry.Key + " didn't spawn anything? this is broken");
                }
            }
        }
        else
        {
            Debug.Log("the prop dictionary is null");
        }
        return staticObjectPrefabs;
    }

    public GameObject CreateTempVTPrefab(CustomStaticPropBase prop) {
        GameObject temp = prop.Spawn();
        temp.name = prop.objectID;
        VTStaticObject staticObject = temp.AddComponent<VTStaticObject>();
        staticObject.category = prop.category;
        staticObject.alignToSurface = prop.alignToSurface;
        staticObject.objectName = prop.objectName;
        staticObject.placementMode = prop.placementMode;
        staticObject.spawnObjects = new List<VTStaticObject.SpawnObject>();
        staticObject.editorOnly = prop.hidden;
        temp.transform.position = Vector3.down * 30000;
        Destroy(temp, 1);
        return temp;
    }

    public void AddCustomUnit(CustomUnitBase unit)
    {
        if (customUnits.ContainsKey(unit.unitID) == false)
        {
            Debug.Log("custom unit id: " + unit.unitID + " is unique, adding to custom unit list");
            customUnits.Add(unit.unitID, unit);
        }
        else
        {
            Debug.Log("A unit with the id: " + unit.unitID + " already exists.");
        }
    }

    public GameObject CreateTempCustomUnitPrefab(CustomUnitBase unit)
    {
        Debug.Log("Spawning custom unit: " + unit.unitName);
        GameObject temp = unit.Spawn();
        temp.transform.position = Vector3.down * 30000;
        Destroy(temp, 1);
        return temp;
    }

    public void GetFirePrefabs() {
        UnitCatalogue.UpdateCatalogue();

        Debug.Log("Trying to get fire!");
        firePrefab = UnitCatalogue.GetUnitPrefab("SRADTruck").GetComponent<VehicleFireDeath>().firePrefab;
        Debug.Log("Got fire!");

        Debug.Log("Trying to get large fire!");
        largeFirePrefab = UnitCatalogue.GetUnitPrefab("ABomberAI").GetComponent<VehicleFireDeath>().firePrefab;
        Debug.Log("Got large fire!");

        Debug.Log("Trying to get cannon SFX!");
        cannonClip = UnitCatalogue.GetUnitPrefab("alliedMBT1").GetComponent<Gun>().fireAudioClip;
        Debug.Log("Got cannon SFX!");
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
        else {
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
}