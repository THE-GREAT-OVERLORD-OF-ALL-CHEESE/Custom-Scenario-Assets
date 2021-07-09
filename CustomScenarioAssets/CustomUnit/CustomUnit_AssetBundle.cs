using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CustomUnit_AssetBundle : CustomUnitBase
{
    public GameObject prefab;

    public CustomUnit_AssetBundle(Teams team, string category, string id, string name, string description, UnitSpawn.PlacementModes placementMode, bool alignToSurface, GameObject prefab) : base(team, category, id, name, description, placementMode, alignToSurface)
    {
        this.prefab = prefab;
    }

    public override GameObject Spawn()
    {
        GameObject root = GameObject.Instantiate(prefab);
        return root;
    }
}