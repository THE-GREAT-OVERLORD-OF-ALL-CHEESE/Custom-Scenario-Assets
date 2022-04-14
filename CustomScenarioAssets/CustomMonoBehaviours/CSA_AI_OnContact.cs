using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace CSA.CustomMonoBehaviours
{
	public class CSA_AI_OnContact : CSA_AI_SlowUpdate
	{
		public VisualTargetFinder detector;
		public Radar radar;

		public UnityEvent onContact;
		public UnityEvent onLostContact;

		public float contactPersistance = 10;

		private bool lastContact;
		private float contactTimer;

		protected override void SlowUpdate(float deltaTime)
		{
			bool contact = false;
			if (detector != null && detector.attackingTarget != null)
			{
				contact = true;
			}
			if (radar != null && radar.detectedUnits.Count > 0)
			{
				contact = true;
			}

			if (contact)
			{
				contactTimer = contactPersistance;
			}
			else
			{
				contactTimer -= deltaTime;
			}

			if (contact != lastContact)
			{
				if (contact) {
					onContact.Invoke();
					lastContact = contact;
				}
				else
				{
					if (contactTimer <= 0)
					{
						onLostContact.Invoke();
						lastContact = contact;
					}
				}
			}
		}
	}
}
