using UnityEngine;

public static class CSAUtilities
{
    public static void ConvertToGunTurret(GameObject turretRoot, TurretInfo info)
    {
        Debug.Log("Converting " + turretRoot.name + " to a gun turret!");

        if (turretRoot == null)
        {
            Debug.Log("Turret root is null, aborting");
            return;
        }
        if (info == null)
        {
            Debug.Log("Turret info is null, aborting");
            return;
        }

        Debug.Log("Finding transforms");

        Transform root = turretRoot.transform;
        Transform turretYaw = root.Find("Yaw");
        if (turretYaw == null)
        {
            Debug.Log("Couldn't find Yaw, aborting");
            return;
        }
        Transform turretPitch = turretYaw.Find("Pitch");
        if (turretPitch == null)
        {
            Debug.Log("Couldn't find Pitch, aborting");
            return;
        }
        Transform muzzleRef = turretPitch.Find("MuzzleRef");
        if (muzzleRef == null)
        {
            Debug.Log("Couldn't find MuzzleRef, aborting");
            return;
        }
        Transform muzzleParent = turretPitch.Find("MuzzleParent");
        if (muzzleParent == null)
        {
            Debug.Log("Couldn't find MuzzleParent, aborting");
            return;
        }

        Debug.Log("Adding turret");
        ModuleTurret turret = root.gameObject.AddComponent<ModuleTurret>();
        turret.yawTransform = turretYaw;
        turret.pitchTransform = turretPitch;
        turret.referenceTransform = muzzleRef;

        turret.yawRange = info.yawRange;
        turret.minPitch = info.minPitch;
        turret.maxPitch = info.maxPitch;

        turret.yawSpeedDPS = info.yawSpeed;
        turret.pitchSpeedDPS = info.pitchSpeed;

        Debug.Log("Adding gun");
        Gun gun = root.gameObject.AddComponent<Gun>();
        gun.rpm = info.gunRPM;
        gun.bulletInfo = info.bulletInfo;

        Transform[] muzzles = new Transform[muzzleParent.childCount];
        for (int i = 0; i < muzzleParent.childCount; i++)
        {
            muzzles[i] = muzzleParent.GetChild(i);
        }
        gun.fireTransforms = muzzles;
        gun.currentAmmo = info.currentAmmo;
        gun.maxAmmo = info.maxAmmo;

        gun.fireAudioSource = root.gameObject.GetComponent<AudioSource>();

        Debug.Log("Adding AI");
        GameObject finderTf = root.Find("VisualTargetFinder").gameObject;

        VisualTargetFinder targetFinder = finderTf.AddComponent<VisualTargetFinder>();
        targetFinder.visionRadius = info.visionRadius;
        targetFinder.minEngageRadius = info.minEngageRadius;
        targetFinder.targetScanInterval = info.targetScanInterval;
        targetFinder.targetsToFind = info.targetsToFind;

        GunTurretAI ai = root.gameObject.AddComponent<GunTurretAI>();
        ai.gun = gun;
        ai.turret = turret;
        ai.targetFinder = targetFinder;

        ai.maxFiringRange = info.maxFiringRange;
        ai.burstTime = info.maxBurstTime;
        ai.minBurstTime = info.minBurstTime;
        ai.fireInterval = info.fireInterval;

        ai.inaccuracyFactor = info.inaccuracyFactor;
        ai.inaccuracyRate = info.inaccuracyRate;

        ai.leadSweep = info.leadSweep;
        ai.leadSweepRate = info.leadSweepRate;

        ai.engageEnemies = true;

        if (info.airburst)
        {
            AI_AirburstHelper airburst = root.gameObject.AddComponent<AI_AirburstHelper>();
        }
    }
}
