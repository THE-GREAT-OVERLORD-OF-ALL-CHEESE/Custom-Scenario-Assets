using CSA.CustomMonoBehaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CustomUnit_ExampleAAGun : CustomUnitBase
{
    public CustomUnit_ExampleAAGun(Teams team, string category, string id, string name, string description, UnitSpawn.PlacementModes placementMode, bool alignToSurface) : base(team, category, id, name, description, placementMode, alignToSurface)
    {

    }

    public override GameObject Spawn()
    {
        GameObject root = new GameObject();
        root.name = unitID;

        root.SetActive(false);

        Debug.Log("Adding ai unit spawn");
        AIUnitSpawn spawn = root.AddComponent<AIUnitSpawn>();
        spawn.category = category;
        spawn.unitName = unitName;
        spawn.unitDescription = description;
        spawn.placementMode = placementMode;

        spawn.alignToGround = alignToSurface;
        spawn.createFloatingOriginTransform = true;
        spawn.heightFromSurface = 0;

        spawn.engageEnemies = true;

        Debug.Log("Adding health");
        Health health = root.AddComponent<Health>();
        health.startHealth = 100;
        health.maxHealth = 100;
        health.minDamage = 0;
        health.invincible = false;
        health.OnDeath = new UnityEngine.Events.UnityEvent();

        spawn.health = health;

        Debug.Log("Adding fire");
        VehicleFireDeath fire = root.AddComponent<VehicleFireDeath>();
        //fire.firePrefab = CustomScenarioAssets.instance.firePrefab;
        fire.explosionType = ExplosionManager.ExplosionTypes.Small;

        Debug.Log("Adding actor");
        Actor actor = root.AddComponent<Actor>();
        actor.team = team;
        actor.role = Actor.Roles.Ground;
        actor.iconType = team == Teams.Allied ? UnitIconManager.MapIconTypes.FriendlyGround : UnitIconManager.MapIconTypes.EnemyGround;

        GameObject turretBase = GameObject.CreatePrimitive(PrimitiveType.Cube);
        turretBase.transform.parent = root.transform;
        turretBase.transform.localPosition = new Vector3(0,0.5f,0);
        turretBase.transform.localScale = new Vector3(3, 1, 3);
        turretBase.GetComponent<MeshRenderer>().material.color = Color.grey;

        Debug.Log("Adding base hitbox");
        Hitbox hitbox = turretBase.AddComponent<Hitbox>();
        hitbox.health = health;

        Debug.Log("Adding turret");
        GameObject turretYaw = new GameObject();
        turretYaw.transform.parent = root.transform;
        turretYaw.transform.localPosition = new Vector3(0, 1.0f, 0);

        GameObject turretYawModel = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        turretYawModel.transform.parent = turretYaw.transform;
        turretYawModel.transform.localPosition = new Vector3(0, 0.5f, 0);
        turretYawModel.transform.localScale = new Vector3(2, 2, 2);
        turretYawModel.GetComponent<MeshRenderer>().material.color = team == Teams.Allied ? Color.blue : Color.red;

        Debug.Log("Adding yaw hitbox");
        Hitbox hitbox2 = turretYawModel.AddComponent<Hitbox>();
        hitbox2.health = health;

        GameObject turretPitch = new GameObject();
        turretPitch.transform.parent = turretYaw.transform;
        turretPitch.transform.localPosition = new Vector3(0, 0.5f, 0);

        GameObject muzzleRef = new GameObject();
        muzzleRef.transform.parent = turretPitch.transform;
        muzzleRef.transform.localPosition = new Vector3(0, 0, 2.5f);

        ModuleTurret turret = root.AddComponent<ModuleTurret>();
        turret.yawTransform = turretYaw.transform;
        turret.pitchTransform = turretPitch.transform;
        turret.referenceTransform = muzzleRef.transform;

        turret.yawRange = 360;
        turret.minPitch = -10;
        turret.maxPitch = 80;

        turret.yawSpeedDPS = 90;
        turret.pitchSpeedDPS = 90;

        Debug.Log("Adding gun");
        GameObject muzzle1 = new GameObject();
        muzzle1.transform.parent = turretPitch.transform;
        muzzle1.transform.localPosition = new Vector3(-0.5f, 0, 2.5f);

        GameObject muzzle2 = new GameObject();
        muzzle2.transform.parent = turretPitch.transform;
        muzzle2.transform.localPosition = new Vector3(0.5f, 0, 2.5f);

        GameObject barrel1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        barrel1.transform.parent = turretPitch.transform;
        barrel1.transform.localPosition = new Vector3(-0.5f, 0, 1.2f);
        barrel1.transform.localScale = new Vector3(0.1f, 0.1f, 3);
        barrel1.GetComponent<MeshRenderer>().material.color = Color.black;
        GameObject.Destroy(barrel1.GetComponent<BoxCollider>());

        GameObject barrel2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        barrel2.transform.parent = turretPitch.transform;
        barrel2.transform.localPosition = new Vector3(0.5f, 0, 1.2f);
        barrel2.transform.localScale = new Vector3(0.1f, 0.1f, 3);
        barrel2.GetComponent<MeshRenderer>().material.color = Color.black;
        GameObject.Destroy(barrel2.GetComponent<BoxCollider>());

        GameObject ejectTf = new GameObject();
        ejectTf.transform.parent = turretPitch.transform;
        ejectTf.transform.localPosition = new Vector3(0, 0, 1.1f);
        ejectTf.transform.localEulerAngles = new Vector3(90, 0, 0);

        Gun gun = turretPitch.AddComponent<Gun>();
        gun.rpm = 300;
        gun.bulletInfo = new Bullet.BulletInfo();
        gun.bulletInfo.speed = 1000;
        gun.bulletInfo.tracerWidth = 0.25f;
        gun.bulletInfo.rayWidth = 0;
        gun.bulletInfo.dispersion = 0.1f;
        gun.bulletInfo.damage = 15;
        gun.bulletInfo.detonationRange = 10;
        gun.bulletInfo.color = Color.red;
        gun.bulletInfo.maxLifetime = 6;
        gun.bulletInfo.lifetimeVariance = 0.2f;
        gun.bulletInfo.projectileMass = 0;
        gun.bulletInfo.totalMass = 0;

        gun.fireTransforms = new Transform[] {muzzle1.transform, muzzle2.transform};
        gun.currentAmmo = 1000;
        gun.maxAmmo = 1000;
        //gun.ejectTransform = ejectTf.transform;

        gun.fireAudioSource = turretPitch.AddComponent<AudioSource>();
        gun.fireAudioSource.minDistance = 80;
        gun.fireAudioSource.minDistance = 4500;
        gun.fireAudioSource.spatialBlend = 1;
        //gun.fireAudioClip = CustomScenarioAssets.instance.cannonClip;

        Debug.Log("Adding AI");
        GameObject finderTf = new GameObject();
        finderTf.transform.parent = turretYaw.transform;
        finderTf.transform.localPosition = new Vector3(0, 3, 0);

        VisualTargetFinder targetFinder = finderTf.AddComponent<VisualTargetFinder>();
        targetFinder.visionRadius = 5000;
        targetFinder.minEngageRadius = 50;
        targetFinder.targetScanInterval = 5;
        targetFinder.targetsToFind.ground = true;
        targetFinder.targetsToFind.groundArmor = true;
        targetFinder.targetsToFind.air = true;
        targetFinder.targetsToFind.ship = false;
        targetFinder.targetsToFind.missile = true;

        GunTurretAI ai = root.AddComponent<GunTurretAI>();
        ai.gun = gun;
        ai.turret = turret;
        ai.targetFinder = targetFinder;

        ai.maxFiringRange = 4000;
        ai.burstTime = 3;
        ai.minBurstTime = 2;
        ai.fireInterval = 2;

        ai.inaccuracyFactor = 1;
        ai.inaccuracyRate = 4;

        ai.leadSweep = 3;
        ai.leadSweepRate = 5;

        ai.engageEnemies = true;

        CSA_AI_AirburstHelper airburst = root.AddComponent<CSA_AI_AirburstHelper>();

        root.SetActive(true);

        return root;
    }
}