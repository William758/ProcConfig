using System.Collections.Generic;
using UnityEngine;
using RoR2;

namespace TPDespair.ProcConfig
{
	public class LimiterBehavior : CharacterBody.ItemBehavior
	{
		private class Blocker
		{
			internal Vector3 position;
			internal float timer;
			internal float sqrRad;
			internal bool cavity = true;

			public Blocker(Vector3 p, float t, float r)
			{
				position = p;
				timer = t;
				sqrRad = r * r;
			}
		}

		private readonly List<Blocker> Blockers = new List<Blocker>();

		private bool AnyCavity = false;

		public void Awake()
		{
			enabled = false;
		}

		public void OnDisable()
		{
			Blockers.Clear();
		}

		public void FixedUpdate()
		{
			float deltaTime = Time.fixedDeltaTime;

			for (int i = 0; i < Blockers.Count; i++)
			{
				Blocker blocker = Blockers[i];

				blocker.timer -= deltaTime;
				if (blocker.timer <= 0f)
				{
					Blockers.RemoveAt(i);
					i--;
				}

				if (blocker.cavity) blocker.cavity = false;
			}

			AnyCavity = false;
		}



		public bool ProceedWithProc(Vector3 position)
		{
			if (Blockers.Count == 0) return true;

			bool blockedByFilled = false;

			for (int i = 0; i < Blockers.Count; i++)
			{
				Blocker blocker = Blockers[i];

				float sqrDist = (position - blocker.position).sqrMagnitude;

				if (AnyCavity)
				{
					if (blocker.cavity && sqrDist <= 1f) return true;
					if (sqrDist <= blocker.sqrRad) blockedByFilled = true;
				}
				else
				{
					if (sqrDist <= blocker.sqrRad) return false;
				}
			}

			if (blockedByFilled) return false;

			return true;
		}

		public void AddBlocker(Vector3 position, bool cavity = true)
		{
			if (!BlockerOverlap(position, cavity))
			{
				float duration = ProcConfigPlugin.OnKillLimiterDurationCfg.Value;
				float radius = ProcConfigPlugin.OnKillLimiterDistanceCfg.Value;

				Blocker blocker = new Blocker(position, duration, radius);

				if (cavity) AnyCavity = true;
				else blocker.cavity = false;

				Blockers.Add(blocker);
			}
		}

		private bool BlockerOverlap(Vector3 position, bool cavity)
		{
			if (Blockers.Count == 0) return false;

			for (int i = 0; i < Blockers.Count; i++)
			{
				Blocker blocker = Blockers[i];

				if (blocker.cavity == cavity && (position - blocker.position).sqrMagnitude <= 1f) return true;
			}

			return false;
		}
	}
}
