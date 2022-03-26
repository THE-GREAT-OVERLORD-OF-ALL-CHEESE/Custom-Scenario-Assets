using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CustomStaticPropBase
{
    public string category = "Cheeses Example Props";
    public string objectID = "cheese_1m_cube";
    public string objectName = "1m Cube";
    public UnitSpawn.PlacementModes placementMode = UnitSpawn.PlacementModes.Any;
    public bool alignToSurface = true;
    public bool hidden = false;

    public CustomStaticPropBase(string category, string id, string name, UnitSpawn.PlacementModes placementMode, bool alignToSurface, bool hidden = false)
    {
        this.category = category;
        objectID = id;
        objectName = name;
        this.placementMode = placementMode;
        this.alignToSurface = alignToSurface;
        this.hidden = hidden;
    }

    public virtual GameObject Spawn()
    {
        return GameObject.CreatePrimitive(PrimitiveType.Cube);
    }
}
