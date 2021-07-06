using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CustomStaticProp_CarrierCatapult : CustomStaticPropBase
{
    public CustomStaticProp_CarrierCatapult(string category, string id, string name, UnitSpawn.PlacementModes placementMode, bool alignToSurface, bool hidden = false) : base(category, id, name, placementMode, alignToSurface, hidden)
    {

    }

    public override GameObject Spawn()
    {
        GameObject root = new GameObject();
        Rigidbody rb = root.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        Debug.Log("Trying to get catapult!");
        GameObject catPrefab = UnitCatalogue.GetUnitPrefab("AlliedCarrier").GetComponentInChildren<CarrierCatapult>().gameObject;
        Debug.Log("Got catapult!");
        GameObject catapult = GameObject.Instantiate(catPrefab);
        catapult.transform.parent = root.transform;
        catapult.transform.localPosition = new Vector3(0,0.15f,0);
        catapult.transform.localRotation = Quaternion.identity;

        catapult.GetComponent<CarrierCatapult>().parentRb = rb;

        Debug.Log("Trying to get deflector!");
        GameObject deflectorPrefab = UnitCatalogue.GetUnitPrefab("AlliedCarrier").GetComponentInChildren<TelescopePiston>().gameObject;
        Debug.Log("Got deflector!");
        GameObject deflector = GameObject.Instantiate(deflectorPrefab);
        deflector.transform.parent = root.transform;
        deflector.transform.localPosition = new Vector3(0, 0, -8);
        deflector.transform.localRotation = Quaternion.Euler(-90, 0, -90);

        catapult.GetComponent<CarrierCatapult>().deflectorRotator.transforms[0].transform = deflector.transform;

        CarrierCatapultManager catManager = root.AddComponent<CarrierCatapultManager>();
        catManager.catapults = new List<CarrierCatapult>();
        catManager.catapults.Add(catapult.GetComponent<CarrierCatapult>());

        return root;
    }
}
