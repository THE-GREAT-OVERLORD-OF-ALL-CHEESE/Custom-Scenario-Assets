using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CustomHPEquipBase
{
    public string equipPath = "HPEquips/VTOL";
    public string equipID = "cheese_test_gun";
    public string allowedHardpoints = "0,1,2,3,4,5,6";

    public CustomHPEquipBase(string equipPath, string equipID, string allowedHardpoints)
    {
        this.equipPath = equipPath;
        this.equipID = equipID;
        this.allowedHardpoints = allowedHardpoints;
    }

    public virtual GameObject Spawn()
    {
        GameObject root = new GameObject();
        root.name = equipID;

        HPEquipGun gunEquip = root.AddComponent<HPEquipGun>();
        gunEquip.fullName = "Cheese's Test Gun";
        gunEquip.shortName = "KEK9000";
        gunEquip.localize = false;
        gunEquip.unitCost = 0;
        gunEquip.description = "A weapon made to test adding custom weapons using CSA";
        gunEquip.subLabel = "CANNON";
        gunEquip.jettisonable = true;
        gunEquip.armable = true;
        gunEquip.reticleIndex = 0;
        gunEquip.allowedHardpoints = allowedHardpoints;
        gunEquip.baseRadarCrossSection = 0.25f;

        gunEquip.perAmmoCost = 0;
        gunEquip.shakeMagnitude = 0.15f;

        Gun gun = root.AddComponent<Gun>();//the gun cant be setup from here, this base class cannot be used in game
        gunEquip.gun = gun;
        
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.parent = root.transform;
        
        return root;
    }
}
