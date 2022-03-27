using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CSA.CustomMonoBehaviours
{
    public class CSA_AI_AutoFlaps : MonoBehaviour
    {
		private void Update()
		{
			if (useAirSpeed)
			{
				autoPilot.SetFlaps(speedFlapsCurve.Evaluate(flightInfo.airspeed));
				return;
			}
			if (useEngineTilt)
			{
				autoPilot.SetFlaps(engineTiltCurve.Evaluate(tiltController.currentTilt / tiltController.maxTilt));
			}
		}

		public FlightInfo flightInfo;
		public TiltController tiltController;
		public AutoPilot autoPilot;

		public bool useAirSpeed;
		public AnimationCurve speedFlapsCurve;
		public bool useEngineTilt;
		public AnimationCurve engineTiltCurve;
	}
}
