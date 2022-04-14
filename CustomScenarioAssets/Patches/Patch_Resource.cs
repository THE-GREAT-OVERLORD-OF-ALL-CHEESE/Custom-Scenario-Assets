using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;

//this is patching a function in Unity, I dont like this, but im not sure what else to do.
/*
[HarmonyPatch(typeof(Resources), "Load")]
class Patch_Resources_Load
{
    [HarmonyPrefix]
    static bool Prefix(ref UnityEngine.Object __result, string path)
    {
        HPEquippable equip;
        if (CustomScenarioAssets.instance.customEquips.TryGetValue(path, out equip))
        {
            __result = equip.gameObject;
            return false;
        }
        return true;
	}
}
*/

[HarmonyPatch(typeof(WeaponManager), "EquipWeapons")]
class Patch_WeaponManager_EquipWeapons
{
	[HarmonyPrefix]
	static bool Prefix(WeaponManager __instance, Loadout loadout)
	{
		Traverse traverse = Traverse.Create(__instance);

		float maxAntiAirRange = 0f;
		float maxAntiRadRange = 0f;
		float maxAGMRange = 0f;

		HPEquippable[] equips = (HPEquippable[])traverse.Field("equips").GetValue();

		List<string> uniqueWeapons = new List<string>();

		MassUpdater component = __instance.vesselRB.GetComponent<MassUpdater>();
		for (int i = 0; i < equips.Length; i++)
		{
			if (equips[i] != null)
			{
				foreach (IMassObject o in equips[i].GetComponentsInChildren<IMassObject>())
				{
					component.RemoveMassObject(o);
				}
				equips[i].OnUnequip();
				__instance.InvokeUnequipEvent(i);
				UnityEngine.Object.Destroy(equips[i].gameObject);
				equips[i] = null;
			}
		}
		string[] hpLoadout = loadout.hpLoadout;
		int num = 0;
		while (num < __instance.hardpointTransforms.Length && num < hpLoadout.Length)
		{
			if (!string.IsNullOrEmpty(hpLoadout[num]))
			{
				UnityEngine.Object @object = null;
				HPEquippable equip;
				if (CustomScenarioAssets.instance.customEquips.TryGetValue(__instance.resourcePath + "/" + hpLoadout[num], out equip))
				{
					@object = equip.gameObject;
					Debug.Log($"Equiping custom equip: {__instance.resourcePath} / {hpLoadout[num]}");
				}
				else
				{
					@object = Resources.Load(__instance.resourcePath + "/" + hpLoadout[num]);
					Debug.Log($"Equiping stock equip: {__instance.resourcePath} / {hpLoadout[num]}");
				}

				if (@object)
				{
					GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(@object, __instance.hardpointTransforms[num]);
					gameObject.name = hpLoadout[num];
					gameObject.transform.localRotation = Quaternion.identity;
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.transform.localScale = Vector3.one;
					HPEquippable component2 = gameObject.GetComponent<HPEquippable>();
					component2.SetWeaponManager(__instance);
					equips[num] = component2;
					component2.wasPurchased = true;
					component2.hardpointIdx = num;
					component2.Equip();
					/*
					if (__instance.OnWeaponEquipped != null)
					{
						__instance.OnWeaponEquipped(component2);
					}
					if (__instance.OnWeaponEquippedHPIdx != null)
					{
						__instance.OnWeaponEquippedHPIdx(num);
					}
					*/
					if (component2.jettisonable)
					{
						Rigidbody component3 = component2.GetComponent<Rigidbody>();
						if (component3)
						{
							component3.interpolation = RigidbodyInterpolation.None;
						}
					}
					if (component2.armable)
					{
						component2.armed = true;
						if (!uniqueWeapons.Contains(component2.shortName))
						{
							uniqueWeapons.Add(component2.shortName);
						}
					}
					gameObject.SetActive(true);
					foreach (Component component4 in component2.gameObject.GetComponentsInChildren<Component>())
					{
						if (component4 is IParentRBDependent)
						{
							((IParentRBDependent)component4).SetParentRigidbody(__instance.vesselRB);
						}
						if (component4 is IRequiresLockingRadar)
						{
							((IRequiresLockingRadar)component4).SetLockingRadar(__instance.lockingRadar);
						}
						if (component4 is IRequiresOpticalTargeter)
						{
							((IRequiresOpticalTargeter)component4).SetOpticalTargeter(__instance.opticalTargeter);
						}
					}
					if (component2 is HPEquipIRML || component2 is HPEquipRadarML)
					{
						if (component2.dlz)
						{
							DynamicLaunchZone.LaunchParams dynamicLaunchParams = component2.dlz.GetDynamicLaunchParams(__instance.transform.forward * 343f, __instance.transform.position + __instance.transform.forward * 10000f, Vector3.zero);
							maxAntiAirRange = Mathf.Max(dynamicLaunchParams.maxLaunchRange, maxAntiAirRange);
						}
					}
					else if (component2 is HPEquipARML)
					{
						if (component2.dlz)
						{
							DynamicLaunchZone.LaunchParams dynamicLaunchParams2 = component2.dlz.GetDynamicLaunchParams(__instance.transform.forward * 343f, __instance.transform.position + __instance.transform.forward * 10000f, Vector3.zero);
							maxAntiRadRange = Mathf.Max(dynamicLaunchParams2.maxLaunchRange, maxAntiRadRange);
						}
					}
					else if (component2 is HPEquipOpticalML && component2.dlz)
					{
						DynamicLaunchZone.LaunchParams dynamicLaunchParams3 = component2.dlz.GetDynamicLaunchParams(__instance.transform.forward * 280f, __instance.transform.position + __instance.transform.forward * 10000f, Vector3.zero);
						maxAGMRange = Mathf.Max(dynamicLaunchParams3.maxLaunchRange, maxAGMRange);
					}
					__instance.ReportWeaponArming(component2);
					__instance.ReportEquipJettisonMark(component2);
				}
				else
				{
					Debug.Log( $"Equip: {__instance.resourcePath} / {hpLoadout[num]} not found..." );
				}
			}
			num++;
		}
		if (__instance.vesselRB)
		{
			__instance.vesselRB.ResetInertiaTensor();
		}
		if (loadout.cmLoadout != null)
		{
			CountermeasureManager componentInChildren = __instance.GetComponentInChildren<CountermeasureManager>();
			if (componentInChildren)
			{
				int num2 = 0;
				while (num2 < componentInChildren.countermeasures.Count && num2 < loadout.cmLoadout.Length)
				{
					componentInChildren.countermeasures[num2].count = Mathf.Clamp(loadout.cmLoadout[num2], 0, componentInChildren.countermeasures[num2].maxCount);
					componentInChildren.countermeasures[num2].UpdateCountText();
					num2++;
				}
			}
		}
		//__instance.weaponIdx = 0;
		traverse.Field("weaponIdx").SetValue(0);
		__instance.ToggleMasterArmed();
		__instance.ToggleMasterArmed();
		if (__instance.OnWeaponChanged != null)
		{
			__instance.OnWeaponChanged.Invoke();
		}
		component.UpdateMassObjects();
		//__instance.rcsAddDirty = true;
		traverse.Field("rcsAddDirty").SetValue(true);



		traverse.Field("maxAntiAirRange").SetValue(maxAntiAirRange);
		traverse.Field("maxAntiRadRange").SetValue(maxAntiRadRange);
		traverse.Field("maxAGMRange").SetValue(maxAGMRange);

		traverse.Field("equips").SetValue(equips);
		traverse.Field("uniqueWeapons").SetValue(uniqueWeapons);

		return false;
	}
}