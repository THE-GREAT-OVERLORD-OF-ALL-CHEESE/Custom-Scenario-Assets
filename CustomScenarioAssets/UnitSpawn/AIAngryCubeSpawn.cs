using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class AIAngryCubeSpawn : AIUnitSpawn {
    public Material mat;

    [UnitSpawnAttributeRange("Initial Angryness", 0f, 100f, UnitSpawnAttributeRange.RangeTypes.Float)]
    public float angryness;

    private void Start() {
        mat = GetComponentInChildren<MeshRenderer>().material;
        SetAngryness(angryness);
    }

    [VTEvent("Be Angry", "Become Angry")]
    public void BeAngry() {
        Debug.Log("I am angry.");
        angryness = 100;
    }

    private void SetAngryness(float angryness) {
        angryness = Mathf.Clamp(angryness, 0, 100);

        if (mat != null) {
            mat.color = Color.Lerp(Color.white, Color.red, angryness/100f);
        }
    }
}
