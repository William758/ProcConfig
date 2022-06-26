using System;
using UnityEngine;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;

namespace TPDespair.ProcConfig
{
	public static class OnKillLimiter
	{
		internal static int DaggerProjectileIndex = -1;
		internal static EffectIndex WilloWispEffectIndex = EffectIndex.Invalid;
		internal static EffectIndex ShatterSpleenEffectIndex = EffectIndex.Invalid;



		internal static void Init()
		{
			CharacterBody.onBodyInventoryChangedGlobal += HandleItemBehavior;

			IL.RoR2.GlobalEventManager.OnCharacterDeath += TrackGasolineHook;
			IL.RoR2.GlobalEventManager.OnCharacterDeath += TrackWilloWispHook;
			IL.RoR2.GlobalEventManager.OnCharacterDeath += TrackDaggerHook;
			IL.RoR2.GlobalEventManager.OnCharacterDeath += TrackShatterSpleenHook;

			On.RoR2.DelayBlast.Detonate += DelayedBlastHook;
			IL.RoR2.Projectile.ProjectileSingleTargetImpact.OnProjectileImpact += ProjectileImpactHook;
		}



		private static bool TrackBodyProcs(CharacterBody body)
		{
			return body.teamComponent.teamIndex == TeamIndex.Player;
		}

		private static bool BodyHasProcs(CharacterBody body)
		{
			Inventory inventory = body.inventory;
			if (inventory)
			{
				if (inventory.GetItemCount(RoR2Content.Items.IgniteOnKill) > 0) return true;
				if (inventory.GetItemCount(RoR2Content.Items.ExplodeOnDeath) > 0) return true;
				if (inventory.GetItemCount(RoR2Content.Items.Dagger) > 0) return true;
				if (inventory.GetItemCount(RoR2Content.Items.BleedOnHitAndExplode) > 0) return true;
			}

			return false;
		}



		private static void HandleItemBehavior(CharacterBody body)
		{
			if (NetworkServer.active)
			{
				body.AddItemBehavior<LimiterBehavior>((TrackBodyProcs(body) && BodyHasProcs(body)) ? 1 : 0);
			}
		}



		private static void TrackGasolineHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			int ItemCountLocIndex = -1;

			bool found = c.TryGotoNext(
				x => x.MatchLdsfld(typeof(RoR2Content.Items).GetField("IgniteOnKill")),
				x => x.MatchCallOrCallvirt<Inventory>("GetItemCount"),
				x => x.MatchStloc(out ItemCountLocIndex)
			);

			if (found)
			{
				c.Index += 3;

				c.Emit(OpCodes.Ldloc, ItemCountLocIndex);
				c.Emit(OpCodes.Ldarg, 1);
				c.EmitDelegate<Func<int, DamageReport, int>>((count, damageReport) =>
				{
					// damageReport.attackerBody has already been null checked
					CharacterBody atkBody = damageReport.attackerBody;

					if (TrackBodyProcs(atkBody))
					{
						LimiterBehavior behavior = atkBody.GetComponent<LimiterBehavior>();
						if (behavior)
						{
							DamageInfo damageInfo = damageReport.damageInfo;
							Vector3 position = damageInfo.position;

							if (behavior.ProceedWithProc(position))
							{
								behavior.AddBlocker(position);
							}
							else
							{
								//Debug.LogWarning("BlockedProc : Gasoline");
								return 0;
							}
						}
					}

					return count;
				});
				c.Emit(OpCodes.Stloc, ItemCountLocIndex);
			}
			else
			{
				Debug.LogWarning("TrackGasolineHook Failed");
			}
		}

		private static void TrackWilloWispHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			int ItemCountLocIndex = -1;

			bool found = c.TryGotoNext(
				x => x.MatchLdsfld(typeof(RoR2Content.Items).GetField("ExplodeOnDeath")),
				x => x.MatchCallOrCallvirt<Inventory>("GetItemCount"),
				x => x.MatchStloc(out ItemCountLocIndex)
			);

			if (found)
			{
				c.Index += 3;

				c.Emit(OpCodes.Ldloc, ItemCountLocIndex);
				c.Emit(OpCodes.Ldarg, 1);
				c.EmitDelegate<Func<int, DamageReport, int>>((count, damageReport) =>
				{
					// damageReport.attackerBody has already been null checked
					CharacterBody atkBody = damageReport.attackerBody;

					if (TrackBodyProcs(atkBody))
					{
						LimiterBehavior behavior = atkBody.GetComponent<LimiterBehavior>();
						if (behavior)
						{
							DamageInfo damageInfo = damageReport.damageInfo;
							Vector3 position = damageInfo.position;

							if (behavior.ProceedWithProc(position))
							{
								behavior.AddBlocker(position);
							}
							else
							{
								//Debug.LogWarning("BlockedProc : WilloWisp");
								return 0;
							}
						}
					}

					return count;
				});
				c.Emit(OpCodes.Stloc, ItemCountLocIndex);
			}
			else
			{
				Debug.LogWarning("TrackWilloWispHook Failed");
			}
		}

		private static void TrackDaggerHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			int ItemCountLocIndex = -1;

			bool found = c.TryGotoNext(
				x => x.MatchLdsfld(typeof(RoR2Content.Items).GetField("Dagger")),
				x => x.MatchCallOrCallvirt<Inventory>("GetItemCount"),
				x => x.MatchStloc(out ItemCountLocIndex)
			);

			if (found)
			{
				c.Index += 3;

				c.Emit(OpCodes.Ldloc, ItemCountLocIndex);
				c.Emit(OpCodes.Ldarg, 1);
				c.EmitDelegate<Func<int, DamageReport, int>>((count, damageReport) =>
				{
					// damageReport.attackerBody has already been null checked
					CharacterBody atkBody = damageReport.attackerBody;

					if (TrackBodyProcs(atkBody))
					{
						LimiterBehavior behavior = atkBody.GetComponent<LimiterBehavior>();
						if (behavior)
						{
							DamageInfo damageInfo = damageReport.damageInfo;
							Vector3 position = damageInfo.position;

							if (behavior.ProceedWithProc(position))
							{
								behavior.AddBlocker(position);
							}
							else
							{
								//Debug.LogWarning("BlockedProc : Dagger");
								return 0;
							}
						}
					}

					return count;
				});
				c.Emit(OpCodes.Stloc, ItemCountLocIndex);
			}
			else
			{
				Debug.LogWarning("TrackDaggerHook Failed");
			}
		}

		private static void TrackShatterSpleenHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			int ItemCountLocIndex = -1;

			bool found = c.TryGotoNext(
				x => x.MatchLdsfld(typeof(RoR2Content.Items).GetField("BleedOnHitAndExplode")),
				x => x.MatchCallOrCallvirt<Inventory>("GetItemCount"),
				x => x.MatchStloc(out ItemCountLocIndex)
			);

			if (found)
			{
				c.Index += 3;

				c.Emit(OpCodes.Ldloc, ItemCountLocIndex);
				c.Emit(OpCodes.Ldarg, 1);
				c.EmitDelegate<Func<int, DamageReport, int>>((count, damageReport) =>
				{
					CharacterBody vicBody = damageReport.victimBody;
					if (!vicBody || !(vicBody.HasBuff(RoR2Content.Buffs.Bleeding) || vicBody.HasBuff(RoR2Content.Buffs.SuperBleed))) return 0;

					// damageReport.attackerBody has already been null checked
					CharacterBody atkBody = damageReport.attackerBody;

					if (TrackBodyProcs(atkBody))
					{
						LimiterBehavior behavior = atkBody.GetComponent<LimiterBehavior>();
						if (behavior)
						{
							DamageInfo damageInfo = damageReport.damageInfo;
							Vector3 position = damageInfo.position;

							if (behavior.ProceedWithProc(position))
							{
								behavior.AddBlocker(position);
							}
							else
							{
								//Debug.LogWarning("BlockedProc : ShatterSpleen");
								return 0;
							}
						}
					}

					return count;
				});
				c.Emit(OpCodes.Stloc, ItemCountLocIndex);
			}
			else
			{
				Debug.LogWarning("TrackShatterSpleenHook Failed");
			}
		}



		private static void DelayedBlastHook(On.RoR2.DelayBlast.orig_Detonate orig, DelayBlast self)
		{
			EffectIndex effectIndex = EffectCatalog.FindEffectIndexFromPrefab(self.explosionEffect);

			if (effectIndex == WilloWispEffectIndex || effectIndex == ShatterSpleenEffectIndex)
			{
				//Debug.LogWarning("DelayedBlast Detonate");

				GameObject atkObject = self.attacker;
				if (atkObject)
				{
					CharacterBody atkBody = atkObject.GetComponent<CharacterBody>();
					if (atkBody && TrackBodyProcs(atkBody))
					{
						LimiterBehavior behavior = atkBody.GetComponent<LimiterBehavior>();
						if (behavior)
						{
							Vector3 position = self.transform.position;

							behavior.AddBlocker(position, false);
						}
					}
				}
			}

			orig(self);
		}

		private static void ProjectileImpactHook(ILContext il)
		{
			ILCursor c = new ILCursor(il);

			bool found = c.TryGotoNext(
				x => x.MatchCallOrCallvirt<DamageInfo>("ModifyDamageInfo")
			);

			if (found)
			{
				c.Emit(OpCodes.Ldarg, 0);
				c.Emit(OpCodes.Ldarg, 1);
				c.EmitDelegate<Action<ProjectileSingleTargetImpact, ProjectileImpactInfo>>((projectileImpact, impactInfo) =>
				{
					ProjectileController controller = projectileImpact.projectileController;
					if (controller)
					{
						if (controller.catalogIndex == DaggerProjectileIndex)
						{
							//Debug.LogWarning("Projectile Impact");

							GameObject atkObject = controller.owner;
							if (atkObject)
							{
								CharacterBody atkBody = atkObject.GetComponent<CharacterBody>();
								if (atkBody && TrackBodyProcs(atkBody))
								{
									LimiterBehavior behavior = atkBody.GetComponent<LimiterBehavior>();
									if (behavior)
									{
										Vector3 position = impactInfo.estimatedPointOfImpact;

										behavior.AddBlocker(position, false);
									}
								}
							}
						}
					}
				});
			}
			else
			{
				Debug.LogWarning("ProjectileImpactHook Failed");
			}
		}
	}
}
