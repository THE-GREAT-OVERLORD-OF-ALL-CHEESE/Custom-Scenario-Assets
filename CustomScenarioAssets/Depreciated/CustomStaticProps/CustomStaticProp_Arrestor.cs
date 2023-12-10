using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CustomStaticProp_CarrierArrestor : CustomStaticPropBase
{
    public CustomStaticProp_CarrierArrestor(string category, string id, string name, UnitSpawn.PlacementModes placementMode, bool alignToSurface, bool hidden = false) : base(category, id, name, placementMode, alignToSurface, hidden)
    {

    }

    public override GameObject Spawn()
    {
        GameObject root = new GameObject();
        Rigidbody rb = root.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        Debug.Log("Trying to get wire!");
        GameObject catPrefab = UnitCatalogue.GetUnitPrefab("AlliedCarrier").GetComponentInChildren<CarrierCable>().gameObject;
        Debug.Log("Got wire!");
        GameObject catapult = GameObject.Instantiate(catPrefab);
        catapult.transform.parent = root.transform;
        catapult.transform.localPosition = new Vector3(0,0.15f,0);
        catapult.transform.localRotation = Quaternion.identity;

        return root;
    }
}
