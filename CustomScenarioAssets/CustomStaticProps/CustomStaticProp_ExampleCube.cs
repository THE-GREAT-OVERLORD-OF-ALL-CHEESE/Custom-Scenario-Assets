using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CustomStaticProp_ExampleCube : CustomStaticPropBase
{
    public float scale = 1;

    public CustomStaticProp_ExampleCube(string category, string id, string name, UnitSpawn.PlacementModes placementMode, bool alignToSurface, float scale, bool hidden = false) : base(category, id, name, placementMode, alignToSurface, hidden)
    {
        this.scale = scale;
    }

    public override GameObject Spawn()
    {
        GameObject root = new GameObject();
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.parent = root.transform;
        cube.transform.localScale = scale * Vector3.one;
        cube.transform.localPosition = scale * 0.5f * Vector3.up;
        cube.transform.localRotation = Quaternion.identity;
        return root;
    }
}
