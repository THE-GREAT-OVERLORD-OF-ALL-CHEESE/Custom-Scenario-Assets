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
        if (CustomScenarioAssets.instance.updatedAircraft)
        {
            return;
        }
        CustomScenarioAssets.instance.updatedAircraft = true;

        Debug.Log("Adding custom units.");

        foreach (KeyValuePair<string, CustomUnitBase> item in CustomScenarioAssets.instance.customUnits)
        {
            CustomUnitBase unit = item.Value;

            UnitCatalogue.Unit catalogueUnit = new UnitCatalogue.Unit();
            catalogueUnit.prefabName = unit.unitID;
            catalogueUnit.name = unit.unitName;
            catalogueUnit.description = unit.description;
            catalogueUnit.teamIdx = (int)unit.team;
            catalogueUnit.isPlayerSpawn = false;
            catalogueUnit.hideFromEditor = false;

            UnitCatalogue.UnitTeam team = UnitCatalogue.catalogue[unit.team];

            UnitCatalogue.UnitCategory category;
            if (team.categories.TryGetValue(unit.category, out category) == false) {
                category = new UnitCatalogue.UnitCategory();
                category.name = unit.category;
                team.categories.Add(unit.category, category);
                team.keys.Add(unit.category);
            }

            catalogueUnit.categoryIdx = team.keys.IndexOf(unit.category);
            catalogueUnit.unitIdx = category.keys.Count - 1;


            if (category.units.ContainsKey(unit.unitID) == false)
            {
                category.units.Add(unit.unitID, catalogueUnit);
            }
            if (CustomScenarioAssets.instance.unitCatalogUnits.ContainsKey(unit.unitID) == false)
            {
                CustomScenarioAssets.instance.unitCatalogUnits.Add(unit.unitID, catalogueUnit);
            }
        }

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
        CustomUnitBase unit;
        if (CustomScenarioAssets.instance.customUnits.TryGetValue(unitID, out unit))
        {
            __result = CustomScenarioAssets.instance.CreateTempCustomUnitPrefab(unit);
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
        __result = null;
        return true;
    }
}