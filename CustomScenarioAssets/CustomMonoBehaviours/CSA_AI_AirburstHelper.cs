using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CSA.CustomMonoBehaviours
{
    public class CSA_AI_AirburstHelper : MonoBehaviour
    {
        public GunTurretAI ai;
        public MinMax fuseTimer = new MinMax(1, 6);

        private void Start()
        {
            ai = GetComponent<GunTurretAI>();
        }

        private void FixedUpdate()
        {
            Actor tgt = ai.targetFinder.attackingTarget;
            if (tgt != null)
            {
                Vector3 dif = tgt.position - ai.gun.fireTransforms[0].position;
                Vector3 vel = tgt.velocity - ai.gun.actor.velocity;
                ai.gun.bulletInfo.maxLifetime = Mathf.Clamp(VectorUtils.CalculateLeadTime(dif, vel, ai.gun.bulletInfo.speed), fuseTimer.min, fuseTimer.max);
            }
            else
            {
                ai.gun.bulletInfo.maxLifetime = fuseTimer.max;
            }
        }
    }
}