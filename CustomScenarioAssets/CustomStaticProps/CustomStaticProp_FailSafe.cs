using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CustomStaticProp_FailSafe : CustomStaticPropBase
{
    public CustomStaticProp_FailSafe(string category, string id, string name, UnitSpawn.PlacementModes placementMode, bool alignToSurface, bool hidden = false) : base(category, id, name, placementMode, alignToSurface, hidden)
    {

    }

    public override GameObject Spawn()
    {
        GameObject root = new GameObject();
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.parent = root.transform;
        cube.transform.localScale = Vector3.one;
        cube.transform.localPosition =  0.5f * Vector3.up;
        cube.transform.localRotation = Quaternion.identity;
        cube.GetComponent<Renderer>().material.color = Color.red;
        return root;
    }
}