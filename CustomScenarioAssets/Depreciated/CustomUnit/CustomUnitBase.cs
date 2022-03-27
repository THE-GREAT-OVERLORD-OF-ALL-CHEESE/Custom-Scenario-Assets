using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CustomUnitBase
{
    public Teams team = Teams.Allied;
    public string category = "Test Catagory";
    public string unitID = "cheese_testunit";
    public string unitName = "Test Unit";
    public string description = "This is an example unit to test adding custom units!";
    public UnitSpawn.PlacementModes placementMode = UnitSpawn.PlacementModes.Any;
    public bool alignToSurface = true;

    public CustomUnitBase(Teams team, string category, string id, string name, string description, UnitSpawn.PlacementModes placementMode, bool alignToSurface)
    {
        this.team = team;
        this.category = category;
        unitID = id;
        unitName = name;
        this.description = description;
        this.placementMode = placementMode;
        this.alignToSurface = alignToSurface;
    }

    public virtual GameObject Spawn()
    {
        GameObject root = new GameObject();
        root.name = unitID;

        root.SetActive(false);

        Debug.Log("Adding unit spawn");
        AIUnitSpawn spawn = root.AddComponent<AIUnitSpawn>();
        spawn.category = category;
        spawn.unitName = unitName;
        spawn.unitDescription = description;
        spawn.placementMode = placementMode;

        spawn.alignToGround = alignToSurface;
        spawn.createFloatingOriginTransform = true;
        spawn.heightFromSurface = 0.5f;

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
        actor.iconType = actor.iconType = team == Teams.Allied ? UnitIconManager.MapIconTypes.FriendlyGround : UnitIconManager.MapIconTypes.EnemyGround;

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