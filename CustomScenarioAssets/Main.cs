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

    public override void ModLoaded()
    {
        HarmonyInstance harmony = HarmonyInstance.Create("cheese.customScenarioAssets");
        harmony.PatchAll(Assembly.GetExecutingAssembly());

        VTOLAPI.SceneLoaded += SceneLoaded;
        base.ModLoaded();

        instance = this;

        VTMapEdResources.LoadAll();

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
        StartCoroutine(LoadAssetBundles());
    }

    private IEnumerator LoadAssetBundles()
    {
        string address = Directory.GetCurrentDirectory() + @"\VTOLVR_ModLoader\mods\";
        Debug.Log("Checking for: " + address);

        if (Directory.Exists(address))
        {
            Debug.Log(address + " exists!");
            DirectoryInfo info = new DirectoryInfo(address);
            foreach (DirectoryInfo directory in info.GetDirectories())
            {
                Debug.Log("Searching " + address + directory.Name + " for .csa");
                foreach (FileInfo file in directory.GetFiles("*.csa")) {
                    AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(file.FullName);
                    yield return request;

                    UnityEngine.Object[] objects = request.assetBundle.LoadAllAssets(typeof(GameObject));
                    foreach (UnityEngine.Object prefabObj in objects) {
                        GameObject prefab = (GameObject)prefabObj;
                        AddCustomStaticProp(new CustomStaticProp_AssetBundle(file.Name, prefab.name, prefab.name, UnitSpawn.PlacementModes.Any, true, prefab));
                        Debug.Log("Added " + prefab.name);
                    }
                }
            }
        }
        else
        {
            Debug.Log(address + " doesn't exist.");
        }
    }

    private void SceneLoaded(VTOLScenes scene)
    {

    }

    public void AddCustomStaticProp(CustomStaticPropBase prop) {
        if (customProps.ContainsKey(prop.objectID) == false) {
            customProps.Add(prop.objectID, prop);
        }
        else {
            Debug.Log("A prop with this name already exists.");
        }
    }

    public List<VTStaticObject> GenerateFakeVTPrefabs()
    {
        List<VTStaticObject> staticObjectPrefabs = new List<VTStaticObject>();
        foreach (KeyValuePair<string, CustomStaticPropBase> entry in customProps)
        {
            GameObject temp = entry.Value.Spawn();
            temp.name = entry.Value.objectID;
            VTStaticObject staticObject = temp.AddComponent<VTStaticObject>();
            staticObject.category = entry.Value.category;
            staticObject.alignToSurface = entry.Value.alignToSurface;
            staticObject.objectName = entry.Value.objectName;
            staticObject.placementMode = entry.Value.placementMode;
            staticObject.spawnObjects = new List<VTStaticObject.SpawnObject>();

            temp.transform.position = Vector3.down * 30000;

            staticObjectPrefabs.Add(staticObject);
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
        temp.transform.position = Vector3.down * 30000;
        Destroy(temp, 1);
        return temp;
    }
}