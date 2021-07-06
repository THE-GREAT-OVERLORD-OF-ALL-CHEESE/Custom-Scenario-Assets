using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CustomStaticProp_AssetBundle : CustomStaticPropBase
{
    public GameObject prefab;

    public CustomStaticProp_AssetBundle(string category, string id, string name, UnitSpawn.PlacementModes placementMode, bool alignToSurface, GameObject prefab, bool hidden = false) : base(category, id, name, placementMode, alignToSurface, hidden)
    {
        this.prefab = prefab;
    }

    public override GameObject Spawn()
    {
        GameObject root = GameObject.Instantiate(prefab);
        return root;
    }
}
