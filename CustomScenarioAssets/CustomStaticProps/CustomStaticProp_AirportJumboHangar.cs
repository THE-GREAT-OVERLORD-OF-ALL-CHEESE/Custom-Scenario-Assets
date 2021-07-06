using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CustomStaticProp_AirportJumboHangar : CustomStaticPropBase
{
    public CustomStaticProp_AirportJumboHangar(string category, string id, string name, UnitSpawn.PlacementModes placementMode, bool alignToSurface, bool hidden = false) : base(category, id, name, placementMode, alignToSurface, hidden)
    {

    }

    public override GameObject Spawn()
    {
        GameObject root = new GameObject();
        Rigidbody rb = root.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        Debug.Log("Trying to get hangar!");
        GameObject towerPrefab = VTMapEdResources.GetPrefab("airbase1").transform.Find("jumboHangar").gameObject;
        Debug.Log("Got hangar!");
        GameObject tower = GameObject.Instantiate(towerPrefab);
        tower.transform.parent = root.transform;
        tower.transform.localPosition = new Vector3(0,0,0);
        tower.transform.localRotation = Quaternion.Euler(-90, 0, -90);

        return root;
    }
}
