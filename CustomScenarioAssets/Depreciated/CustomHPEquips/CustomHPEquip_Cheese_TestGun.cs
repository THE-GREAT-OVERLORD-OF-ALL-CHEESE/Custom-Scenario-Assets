using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CustomHPEquip_Cheese_TestGun : CustomHPEquipBase
{
    public CustomHPEquip_Cheese_TestGun(string equipPath, string equipID, string allowedHardpoints) : base(equipPath, equipID, allowedHardpoints)
    {

    }

    public override GameObject Spawn()
    {
        GameObject root = new GameObject();
        root.name = equipID;
        root.SetActive(false);

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

        Gun gun = root.AddComponent<Gun>();
        gunEquip.gun = gun;
        gun.gunMass = 0;
        gun.rpm = 3900;
        gun.bulletInfo = new Bullet.BulletInfo();
        gun.bulletInfo.speed = 1000;
        gun.bulletInfo.tracerWidth = 0.12f;
        gun.bulletInfo.rayWidth = -1;
        gun.bulletInfo.dispersion = 0.5f;
        gun.bulletInfo.damage = 100;
        gun.bulletInfo.color = Color.red;
        gun.bulletInfo.maxLifetime = 8;
        gun.bulletInfo.lifetimeVariance = 0;
        gun.bulletInfo.projectileMass = 0;
        gun.bulletInfo.totalMass = 0;
        gun.fireTransforms = new Transform[] { root.transform };
        gun.maxAmmo = 10000;
        gun.currentAmmo = 10000;
        gun.fireAudioSource = root.AddComponent<AudioSource>();
        //gun.fireAudioClip = CustomScenarioAssets.instance.m230Clip;
        gun.loopingAudio = true;
        //gun.fireStopClip = CustomScenarioAssets.instance.m230EndClip;

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.parent = root.transform;
        GameObject.Destroy(cube.GetComponent<BoxCollider>());

        root.SetActive(true);

        return root;
    }
}
