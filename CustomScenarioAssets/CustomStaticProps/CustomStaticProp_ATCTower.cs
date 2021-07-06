using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CustomStaticProp_ATCTower : CustomStaticPropBase
{
    public CustomStaticProp_ATCTower(string category, string id, string name, UnitSpawn.PlacementModes placementMode, bool alignToSurface, bool hidden = false) : base(category, id, name, placementMode, alignToSurface, hidden)
    {

    }

    public override GameObject Spawn()
    {
        GameObject root = new GameObject();
        Rigidbody rb = root.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        Debug.Log("Trying to get tower!");
        GameObject towerPrefab = VTMapEdResources.GetPrefab("airbase1").transform.Find("atc").gameObject;
        Debug.Log("Got tower!");
        GameObject tower = GameObject.Instantiate(towerPrefab);
        tower.transform.parent = root.transform;
        tower.transform.localPosition = new Vector3(0,0,0);
        tower.transform.localRotation = Quaternion.Euler(-90, 0, 0);

        return root;
    }
}
