using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CustomUnit_AngryCube : CustomUnitBase
{
    public CustomUnit_AngryCube(Teams team, string category, string id, string name, string description, UnitSpawn.PlacementModes placementMode, bool alignToSurface) : base(team, category, id, name, description, placementMode, alignToSurface)
    {

    }

    public override GameObject Spawn()
    {
        GameObject root = new GameObject();
        root.name = unitID;

        root.SetActive(false);

        Debug.Log("Adding angry cube unit spawn");
        AIAngryCubeSpawn spawn = root.AddComponent<AIAngryCubeSpawn>();
        spawn.category = category;
        spawn.unitName = unitName;
        spawn.unitDescription = description;
        spawn.placementMode = placementMode;

        spawn.alignToGround = alignToSurface;
        spawn.createFloatingOriginTransform = true;
        spawn.heightFromSurface = 0.5f;

        spawn.engageEnemies = true;

        Debug.Log("Adding health");
        Health health = root.AddComponent<Health>();
        health.startHealth = 100;
        health.maxHealth = 100;
        health.minDamage = 0;
        health.invincible = false;
        health.OnDeath = new UnityEngine.Events.UnityEvent();

        spawn.health = health;

        Debug.Log("Adding actor");
        Actor actor = root.AddComponent<Actor>();
        actor.team = team;
        actor.role = Actor.Roles.Ground;
        actor.iconType = team == Teams.Allied ? UnitIconManager.MapIconTypes.FriendlyAir : UnitIconManager.MapIconTypes.EnemyAir;

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.parent = root.transform;

        Debug.Log("Adding hitbox");
        Hitbox hitbox = cube.AddComponent<Hitbox>();
        hitbox.health = health;

        Debug.Log("Adding fire");
        VehicleFireDeath fire = root.AddComponent<VehicleFireDeath>();
        //fire.firePrefab = CustomScenarioAssets.instance.firePrefab;
        fire.explosionType = ExplosionManager.ExplosionTypes.Small;

        root.SetActive(true);

        return root;
    }
}