using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CSA.CustomMonoBehaviours
{
    public class CSA_AI_SlowUpdate : MonoBehaviour
    {
		[Range(0, 15)]
		public float updateInterval = 1;
		private float timer;

		private void Start()
		{
			timer = UnityEngine.Random.Range(0, updateInterval);
		}

		private void Update()
		{
			timer += Time.deltaTime;

			if (timer > updateInterval)
			{
				timer = 0;
				SlowUpdate(updateInterval);
			}
		}

		protected virtual void SlowUpdate(float deltaTime)
		{

		}
	}
}
