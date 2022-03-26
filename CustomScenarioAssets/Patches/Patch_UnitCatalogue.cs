using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;

[HarmonyPatch(typeof(UnitCatalogue), "UpdateCatalogue")]
class Patch_UnitCatalogue_UpdateCatalogue
{
    [HarmonyPostfix]
    static void Postfix()
    {
        if (CustomScenarioAssets.instance.updatedAircraft || CustomScenarioAssets.instance.customUnits == null)
        {
            return;
        }
        CustomScenarioAssets.instance.updatedAircraft = true;

        Debug.Log("Adding custom units to unit catalog.");

        foreach (KeyValuePair<string, UnitSpawn> item in CustomScenarioAssets.instance.customUnits)
        {
            UnitSpawn unit = item.Value;

            Debug.Log("finding unit team");
            Teams team = unit.gameObject.GetComponentInChildren<Actor>().team;
            Debug.Log("found unit team");

            UnitCatalogue.Unit catalogueUnit = new UnitCatalogue.Unit();
            catalogueUnit.prefabName = unit.name;
            catalogueUnit.name = unit.unitName;
            catalogueUnit.description = unit.unitDescription;
            catalogueUnit.teamIdx = (int)team;
            catalogueUnit.isPlayerSpawn = false;
            catalogueUnit.hideFromEditor = false;
            catalogueUnit.resourcePath = unit.name;

            UnitCatalogue.UnitTeam unitTeam = UnitCatalogue.catalogue[team];

            UnitCatalogue.UnitCategory category;
            if (unitTeam.categories.TryGetValue(unit.category, out category) == false) {
                category = new UnitCatalogue.UnitCategory();
                category.name = unit.category;
                unitTeam.categories.Add(unit.category, category);
                unitTeam.keys.Add(unit.category);
            }

            catalogueUnit.categoryIdx = unitTeam.keys.IndexOf(unit.category);
            catalogueUnit.unitIdx = category.keys.Count - 1;


            Debug.Log("updating lists and dictionaries");
            if (category.units.ContainsKey(unit.name) == false)
            {
                category.units.Add(unit.name, catalogueUnit);
            }
            if (CustomScenarioAssets.instance.unitCatalogUnits.ContainsKey(unit.name) == false)
            {
                CustomScenarioAssets.instance.unitCatalogUnits.Add(unit.name, catalogueUnit);
            }
        }
        Debug.Log("setting up allied and enemy catagory options");

        UnitCatalogue.categoryOptions = new Dictionary<Teams, string[]>();
        string[] alliedUnits = new string[UnitCatalogue.catalogue[Teams.Allied].categories.Count];
        int aCount = 0;
        foreach (string alliedUnit in UnitCatalogue.catalogue[Teams.Allied].categories.Keys)
        {
            alliedUnits[aCount] = alliedUnit;
            aCount++;
        }
        UnitCatalogue.categoryOptions.Add(Teams.Allied, alliedUnits);
        string[] enemyUnits = new string[UnitCatalogue.catalogue[Teams.Enemy].categories.Count];
        int eCount = 0;
        foreach (string enemyUnit in UnitCatalogue.catalogue[Teams.Enemy].categories.Keys)
        {
            enemyUnits[eCount] = enemyUnit;
            eCount++;
        }
        UnitCatalogue.categoryOptions.Add(Teams.Enemy, enemyUnits);
    }
}

[HarmonyPatch(typeof(UnitCatalogue), "GetUnitPrefab")]
class Patch_UnitCatalogue_GetUnitPrefab
{
    [HarmonyPrefix]
    static bool Prefix(out GameObject __result, string unitID)
    {
        UnitSpawn unit;
        if (CustomScenarioAssets.instance.customUnits.TryGetValue(unitID, out unit))
        {
            __result = unit.gameObject;
            return false;
        }
        __result = null;
        return true;
    }
}

[HarmonyPatch(typeof(UnitCatalogue), "GetUnit", new Type[] { typeof(string) })]
class Patch_UnitCatalogue_GetUnit
{
    [HarmonyPrefix]
    static bool Prefix(ref UnitCatalogue.Unit __result, string unitID)
    {
        UnitCatalogue.Unit unit;
        if (CustomScenarioAssets.instance.unitCatalogUnits.TryGetValue(unitID, out unit))
        {
            __result = unit;
            return false;
        }
        if (CustomScenarioAssets.instance.baseUnits.Contains(unitID))
        {
            __result = null;
            return true;
        }
        else {
            CustomScenarioAssets.instance.ReportMissingUnit(unitID);
            __result = null;
            return true;
        }
    }
}