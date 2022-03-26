using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using UnityEngine;

[HarmonyPatch(typeof(VTEditorSpawnRenderer), "Start")]
class Patch_VTEditorSpawnRenderer_Start
{
    [HarmonyPrefix]
    static bool Prefix(VTEditorSpawnRenderer __instance)
    {
		Debug.Log("VTEditorSpawnRenderer is spawning a mock unit: " + __instance.GetComponent<UnitSpawner>().unitID);

		if (CustomScenarioAssets.instance.customUnits.ContainsKey(__instance.GetComponent<UnitSpawner>().unitID) == false) {
			Debug.Log("This is a stock unit, the game is in charge of this one");
			return true;
		}

		Debug.Log("This is a custom unit, go go gadget spawn it in.");

		Traverse traverse = Traverse.Create(__instance);

		UnitSpawner spawner = __instance.GetComponent<UnitSpawner>();
		traverse.Field("spawner").SetValue(spawner);
		Material mat = new Material(Shader.Find("Particles/MF-Alpha Blended"));
		traverse.Field("mat").SetValue(mat);
		Color colour = ((spawner.team == Teams.Allied) ? Color.green : Color.red);
		colour.a = 0.06f;
		traverse.Field("unitColor").SetValue(colour);
		mat.SetColor("_TintColor", colour);
		UnitCatalogue.Unit unit = UnitCatalogue.GetUnit(spawner.unitID);
		traverse.Field("unit").SetValue(unit);

		Debug.Log("Setup colour.");

		bool flag = false;
		GameObject gameObject = GameObject.Instantiate(CustomScenarioAssets.instance.customUnits[spawner.unitID].gameObject);
		MeshFilter[] componentsInChildren = gameObject.GetComponentsInChildren<MeshFilter>();
		Vector3 pos = -gameObject.transform.position;
		Quaternion q = Quaternion.Inverse(gameObject.transform.rotation);
		Matrix4x4 lhs = Matrix4x4.TRS(pos, q, Vector3.one);
		List<Mesh> list = new List<Mesh>();
		List<Matrix4x4> list2 = new List<Matrix4x4>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].GetComponent<MeshRenderer>().enabled && componentsInChildren[i].sharedMesh != null && !componentsInChildren[i].gameObject.name.ToLower().Contains("lod"))
			{
				flag = true;
				list.Add(componentsInChildren[i].sharedMesh);
				Matrix4x4 item = lhs * componentsInChildren[i].transform.localToWorldMatrix;
				list2.Add(item);
			}
		}
		List<Mesh> bakedMeshes = new List<Mesh>();
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
		{
			flag = true;
			Mesh mesh = new Mesh();
			skinnedMeshRenderer.BakeMesh(mesh);
			list.Add(mesh);
			bakedMeshes.Add(mesh);
			Matrix4x4 item2 = lhs * skinnedMeshRenderer.transform.localToWorldMatrix;
			list2.Add(item2);
		}
		traverse.Field("bakedMeshes").SetValue(bakedMeshes);
		traverse.Field("meshes").SetValue(list.ToArray());
		traverse.Field("matrices").SetValue(list2.ToArray());

		Debug.Log("Setup meshes.");

		GameObject gameObject2 = new GameObject("Sprite");
		gameObject2.transform.parent = __instance.transform;
		gameObject2.transform.localPosition = Vector3.zero;
		gameObject2.transform.localScale = Vector3.one;
		SpriteRenderer sprite = gameObject2.AddComponent<SpriteRenderer>();
		traverse.Field("sprite").SetValue(sprite);
		VTScenarioEditor.EditorSprite edSprite;
		if (!string.IsNullOrEmpty(unit.editorSprite))
		{
			edSprite = __instance.editor.GetSprite(unit.editorSprite);
		}
		else
		{
			edSprite = __instance.editor.defaultSprite;
		}
		traverse.Field("edSprite").SetValue(edSprite);
		sprite.sprite = edSprite.sprite;
		sprite.color = edSprite.color;
		sprite.sharedMaterial = __instance.editor.spriteMaterial;
		IconScaleTest iconScaleTest = gameObject2.AddComponent<IconScaleTest>();
		iconScaleTest.maxDistance = __instance.editor.spriteMaxDist;
		iconScaleTest.applyScale = true;
		iconScaleTest.directional = false;
		iconScaleTest.faceCamera = true;
		iconScaleTest.scale = edSprite.size * __instance.editor.globalSpriteScale;
		iconScaleTest.cameraUp = true;
		iconScaleTest.updateRoutine = true;
		iconScaleTest.enabled = false;
		iconScaleTest.enabled = true;

		Debug.Log("Setup sprites.");

		GameObject gameObject3 = new GameObject("Label");
		TextMesh textMesh = gameObject3.AddComponent<TextMesh>();
		textMesh.text = spawner.GetUIDisplayName();
		gameObject3.transform.parent = gameObject2.transform;
		gameObject3.transform.localPosition = new Vector3(0f, 0.062f / iconScaleTest.scale, 0f);
		gameObject3.transform.localRotation = Quaternion.identity;
		textMesh.fontSize = __instance.editor.iconLabelFontSize;
		gameObject3.transform.localScale = 0.035f / iconScaleTest.scale * Vector3.one;
		textMesh.anchor = TextAnchor.LowerCenter;
		textMesh.color = sprite.color;
		traverse.Field("nameText").SetValue(textMesh);
		//__instance.editor.OnScenarioObjectsChanged += __instance.Editor_OnScenarioObjectsChanged;
		traverse.Method("SetupMouseDowns").GetValue();
		if (!flag)
		{
			__instance.enabled = false;
		}

		Debug.Log("Setup text.");

		return false;
    }
}