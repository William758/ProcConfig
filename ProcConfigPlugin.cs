using System;
using System.Linq;
using UnityEngine;
using BepInEx;
using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;

using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete

namespace TPDespair.ProcConfig
{
	[BepInPlugin(ModGuid, ModName, ModVer)]

	public class ProcConfigPlugin : BaseUnityPlugin
	{
		public const string ModVer = "1.2.0";
		public const string ModName = "ProcConfig";
		public const string ModGuid = "com.TPDespair.ProcConfig";



		private static bool lateSetupAttempted = false;



		public static GameObject DaggerPrefab;
		public static GameObject MissilePrefab;
		public static GameObject FireworkPrefab;
		public static GameObject BleedExplodePrefab;
		public static GameObject WillowispPrefab;
		public static GameObject DiskPrefab;
		public static GameObject MeatballPrefab;
		public static GameObject ShurikenPrefab;
		public static GameObject VoidFlamePrefab;
		public static GameObject LunarSunPrefab;



		public static ConfigEntry<bool> EnableProcModuleCfg { get; set; }

		public static ConfigEntry<float> DaggerProcCfg { get; set; }
		public static ConfigEntry<float> DaggerDurationCfg { get; set; }
		public static ConfigEntry<float> MissileProcCfg { get; set; }
		public static ConfigEntry<float> FireworkProcCfg { get; set; }
		public static ConfigEntry<float> SpleenProcCfg { get; set; }
		public static ConfigEntry<float> WillowProcCfg { get; set; }
		public static ConfigEntry<float> DiscBeamProcCfg { get; set; }
		public static ConfigEntry<float> DiscExplosionProcCfg { get; set; }
		public static ConfigEntry<float> MeatballProcCfg { get; set; }
		public static ConfigEntry<float> ShurikenProcCfg { get; set; }
		public static ConfigEntry<float> VoidFlameProcCfg { get; set; }
		public static ConfigEntry<float> LunarSunProcCfg { get; set; }

		public static ConfigEntry<float> LightningStrikeProcCfg { get; set; }
		public static ConfigEntry<float> HookProcCfg { get; set; }
		public static ConfigEntry<float> UkuleleProcCfg { get; set; }
		public static ConfigEntry<float> PolyluteProcCfg { get; set; }
		public static ConfigEntry<float> TeslaProcCfg { get; set; }
		public static ConfigEntry<float> RazorProcCfg { get; set; }
		public static ConfigEntry<float> DiscipleProcCfg { get; set; }
		public static ConfigEntry<float> NkuhanaProcCfg { get; set; }
		public static ConfigEntry<float> VoidMissileProcCfg { get; set; }

		public static ConfigEntry<float> IcicleProcCfg { get; set; }
		public static ConfigEntry<float> GasolineProcCfg { get; set; }

		public static ConfigEntry<bool> EnableOnKillLimiterCfg { get; set; }
		public static ConfigEntry<float> OnKillLimiterDistanceCfg { get; set; }
		public static ConfigEntry<float> OnKillLimiterDurationCfg { get; set; }



		public void Awake()
		{
			RoR2Application.isModded = true;
			NetworkModCompatibilityHelper.networkModList = NetworkModCompatibilityHelper.networkModList.Append(ModGuid + ":" + ModVer);

			SetupConfig(Config);

			OnLogBookControllerReady();
			OnMainMenuEnter();

			if (EnableOnKillLimiterCfg.Value)
			{
				OnKillLimiter.Init();
			}
		}
		/*
		public void Update()
		{
			DebugDrops();
		}
		//*/



		private static void SetupConfig(ConfigFile Config)
		{
			EnableProcModuleCfg = Config.Bind(
				"0-Proc - Enable", "enableProcModule", true,
				"Enable Proc Module."
			);

			DaggerProcCfg = Config.Bind(
				"1-Proc - Prefabs", "daggerProc", 0f,
				"Procoeff of Ceremonial Dagger. Vanilla is 1"
			);
			DaggerDurationCfg = Config.Bind(
				"1-Proc - Prefabs", "daggerDuration", 10f,
				"Duration that Ceremonial Daggers linger before expiring. Vanilla is 30"
			);
			MissileProcCfg = Config.Bind(
				"1-Proc - Prefabs", "missileProc", 0.25f,
				"Procoeff of ATG and Disposable Missile Launcher. Vanilla is 1"
			);
			FireworkProcCfg = Config.Bind(
				"1-Proc - Prefabs", "fireworkProc", 0.1f,
				"Procoeff of Bundle of Fireworks. Vanilla is 0.2"
			);
			SpleenProcCfg = Config.Bind(
				"1-Proc - Prefabs", "spleenProc", 0f,
				"Procoeff of Shatterspleen. Vanilla is 1"
			);
			WillowProcCfg = Config.Bind(
				"1-Proc - Prefabs", "willowProc", 0f,
				"Procoeff of Will-o'-the-wisp. Vanilla is 1"
			);
			DiscBeamProcCfg = Config.Bind(
				"1-Proc - Prefabs", "discBeamProc", 0f,
				"Procoeff of Resonance Disc beam. Vanilla is 1"
			);
			DiscExplosionProcCfg = Config.Bind(
				"1-Proc - Prefabs", "discExplosionProc", 0f,
				"Procoeff of Resonance Disc explosion. Vanilla is 0"
			);
			MeatballProcCfg = Config.Bind(
				"1-Proc - Prefabs", "meatballProc", 0.1f,
				"Procoeff of Molten Perforator. Vanilla is 0.7"
			);
			ShurikenProcCfg = Config.Bind(
				"1-Proc - Prefabs", "shurikenProc", 0.25f,
				"Procoeff of Shuriken. Vanilla is 1"
			);
			VoidFlameProcCfg = Config.Bind(
				"1-Proc - Prefabs", "voidFlameProc", 0.1f,
				"Procoeff of Voidsent Flame. Vanilla is 1"
			);
			LunarSunProcCfg = Config.Bind(
				"1-Proc - Prefabs", "lunarSunProc", 1f,
				"Procoeff of Egocentrism. Vanilla is 1"
			);

			LightningStrikeProcCfg = Config.Bind(
				"2-Proc - Orbs", "lightningStrikeProc", 0.25f,
				"Procoeff of Charged Perforator. Vanilla is 1"
			);
			HookProcCfg = Config.Bind(
				"2-Proc - Orbs", "hookProc", 0.2f,
				"Procoeff of Sentient Meat Hook. Vanilla is 0.33"
			);
			UkuleleProcCfg = Config.Bind(
				"2-Proc - Orbs", "ukuleleProc", 0.2f,
				"Procoeff of Ukulele. Vanilla is 0.2"
			);
			PolyluteProcCfg = Config.Bind(
				"2-Proc - Orbs", "polyluteProc", 0.2f,
				"Procoeff of Polylute. Vanilla is 0.2"
			);
			TeslaProcCfg = Config.Bind(
				"2-Proc - Orbs", "teslaProc", 0.2f,
				"Procoeff of Unstable Tesla Coil. Vanilla is 0.3"
			);
			RazorProcCfg = Config.Bind(
				"2-Proc - Orbs", "razorProc", 0.1f,
				"Procoeff of Razorwire. Vanilla is 0.5"
			);
			DiscipleProcCfg = Config.Bind(
				"2-Proc - Orbs", "discipleProc", 0.1f,
				"Procoeff of Little Disciple. Vanilla is 1"
			);
			NkuhanaProcCfg = Config.Bind(
				"2-Proc - Orbs", "nkuhanaProc", 0.1f,
				"Procoeff of N'kuhana's Opinion. Vanilla is 0.2"
			);
			VoidMissileProcCfg = Config.Bind(
				"2-Proc - Orbs", "voidMissileProc", 0.2f,
				"Procoeff of Plasma Shrimp. Vanilla is 0.2"
			);

			IcicleProcCfg = Config.Bind(
				"3-Proc - Other", "icicleProc", 0.1f,
				"Procoeff of Frost Relic. Vanilla is 0.2"
			);
			GasolineProcCfg = Config.Bind(
				"3-Proc - Other", "gasolineProc", 0f,
				"Procoeff of Gasoline. Vanilla is 0"
			);

			EnableOnKillLimiterCfg = Config.Bind(
				"4-Proc - OnKillLimiter", "enableOnKillLimiter", true,
				"On kill effects will create a restricted area in which further on kill effects cannot proc until it expires. Restricted items : Gasoline, Will-o'-the-wisp, Ceremonial Dagger, and Shatterspleen."
			);
			OnKillLimiterDistanceCfg = Config.Bind(
				"4-Proc - OnKillLimiter", "onKillLimiterDistance", 15f,
				"Distance restriction to prevent further on kill procs from player."
			);
			OnKillLimiterDurationCfg = Config.Bind(
				"4-Proc - OnKillLimiter", "onKillLimiterDuration", 0.025f,
				"Duration restriction to prevent further on kill procs from player."
			);
		}



		private static void OnLogBookControllerReady()
		{
			On.RoR2.UI.LogBook.LogBookController.Init += (orig) =>
			{
				try
				{
					if (!lateSetupAttempted)
					{
						lateSetupAttempted = true;

						SetProcCoefficients();
						GatherIndexes();
					}
				}
				catch (Exception ex)
				{
					Debug.LogError(ex);
				}

				orig();
			};
		}

		private static void OnMainMenuEnter()
		{
			On.RoR2.UI.MainMenu.BaseMainMenuScreen.OnEnter += (orig, self, controller) =>
			{
				orig(self, controller);

				try
				{
					if (!lateSetupAttempted)
					{
						lateSetupAttempted = true;

						SetProcCoefficients();
						GatherIndexes();
					}
				}
				catch (Exception ex)
				{
					Debug.LogError(ex);
				}
			};
		}



		private static void SetProcCoefficients()
		{
			if (EnableProcModuleCfg.Value)
			{
				SetDaggerValues();
				SetMissileProc();
				SetFireworkProc();
				SetSpleenDelayedProc();
				SetWillowispDelayedProc();
				SetDiscProc();
				SetMeatballProc();
				SetShurikenProc();
				SetVoidFlameDelayedProc();
				SetLunarSunProc();

				SimpleLightningStrikeOrbProc();
				BounceOrbProc();
				LightningOrbProc();
				DevilOrbProc();
				VoidLightningOrbProc();
				MissileVoidOrbProc();

				IcicleProc();
				GasolineProc();
			}
		}

		private static void GatherIndexes()
		{
			if (EnableOnKillLimiterCfg.Value)
			{
				if (!DaggerPrefab)
				{
					DaggerPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/DaggerProjectile");
				}

				OnKillLimiter.DaggerProjectileIndex = ProjectileCatalog.GetProjectileIndex(DaggerPrefab);
				Debug.LogWarning("Dagger ProjectileIndex : " + OnKillLimiter.DaggerProjectileIndex);

				if (!WillowispPrefab)
				{
					WillowispPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/networkedobjects/WilloWispDelay");
				}

				DelayBlast wispBlast = WillowispPrefab.GetComponent<DelayBlast>();
				OnKillLimiter.WilloWispEffectIndex = EffectCatalog.FindEffectIndexFromPrefab(wispBlast.explosionEffect);
				Debug.LogWarning("WilloWisp EffectIndex : " + OnKillLimiter.WilloWispEffectIndex);

				if (!BleedExplodePrefab)
				{
					BleedExplodePrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/networkedobjects/BleedOnHitAndExplodeDelay");
				}

				DelayBlast bleedBlast = BleedExplodePrefab.GetComponent<DelayBlast>();
				OnKillLimiter.ShatterSpleenEffectIndex = EffectCatalog.FindEffectIndexFromPrefab(bleedBlast.explosionEffect);
				Debug.LogWarning("ShatterSpleen EffectIndex : " + OnKillLimiter.ShatterSpleenEffectIndex);
			}
		}



		private static void SetDaggerValues()
		{
			try
			{
				DaggerPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/DaggerProjectile");
				DaggerPrefab.GetComponent<ProjectileController>().procCoefficient = DaggerProcCfg.Value;
				//Debug.LogWarning("Dagger lifetime : " + DaggerPrefab.GetComponent<ProjectileSimple>().lifetime);
				DaggerPrefab.GetComponent<ProjectileSimple>().lifetime = DaggerDurationCfg.Value;
			}
			catch (Exception ex)
			{
				Debug.Log("Failed to set dagger proc values.");
				Debug.LogError(ex);
			}
		}

		private static void SetMissileProc()
		{
			try
			{
				MissilePrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/MissileProjectile");
				MissilePrefab.GetComponent<ProjectileController>().procCoefficient = MissileProcCfg.Value;
			}
			catch (Exception ex)
			{
				Debug.Log("Failed to set missile proc coefficient.");
				Debug.LogError(ex);
			}
		}

		private static void SetFireworkProc()
		{
			try
			{
				FireworkPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/FireworkProjectile");
				FireworkPrefab.GetComponent<ProjectileController>().procCoefficient = FireworkProcCfg.Value;
			}
			catch (Exception ex)
			{
				Debug.Log("Failed to set firework proc coefficient.");
				Debug.LogError(ex);
			}
		}

		private static void SetSpleenDelayedProc()
		{
			try
			{
				BleedExplodePrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/networkedobjects/BleedOnHitAndExplodeDelay");
				BleedExplodePrefab.GetComponent<DelayBlast>().procCoefficient = SpleenProcCfg.Value;
			}
			catch (Exception ex)
			{
				Debug.Log("Failed to set shatterspleen explosion proc coefficient.");
				Debug.LogError(ex);
			}
		}

		private static void SetWillowispDelayedProc()
		{
			try
			{
				WillowispPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/networkedobjects/WilloWispDelay");
				WillowispPrefab.GetComponent<DelayBlast>().procCoefficient = WillowProcCfg.Value;
			}
			catch (Exception ex)
			{
				Debug.Log("Failed to set willowisp explosion proc coefficient.");
				Debug.LogError(ex);
			}
		}

		private static void SetDiscProc()
		{
			try
			{
				DiskPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/LaserTurbineBomb");
				DiskPrefab.GetComponent<ProjectileController>().procCoefficient = DiscBeamProcCfg.Value;
				DiskPrefab.GetComponent<ProjectileImpactExplosion>().blastProcCoefficient = DiscExplosionProcCfg.Value;

				EntityStates.LaserTurbine.FireMainBeamState.mainBeamProcCoefficient = DiscBeamProcCfg.Value;
			}
			catch (Exception ex)
			{
				Debug.Log("Failed to set resonating disc proc coefficient.");
				Debug.LogError(ex);
			}
		}

		private static void SetMeatballProc()
		{
			try
			{
				MeatballPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/FireMeatBall");
				MeatballPrefab.GetComponent<ProjectileImpactExplosion>().blastProcCoefficient = MeatballProcCfg.Value;
			}
			catch (Exception ex)
			{
				Debug.Log("Failed to set molten perforator proc coefficient.");
				Debug.LogError(ex);
			}
		}

		private static void SetShurikenProc()
		{
			try
			{
				ShurikenPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/ShurikenProjectile");
				//Debug.Log("Shuriken " + ShurikenPrefab.GetComponent<ProjectileController>().procCoefficient);
				ShurikenPrefab.GetComponent<ProjectileController>().procCoefficient = ShurikenProcCfg.Value;
			}
			catch (Exception ex)
			{
				Debug.Log("Failed to set shuriken proc coefficient.");
				Debug.LogError(ex);
			}
		}

		private static void SetVoidFlameDelayedProc()
		{
			try
			{
				VoidFlamePrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/ExplodeOnDeathVoidExplosion");
				//Debug.Log("VoidFlame " + VoidFlamePrefab.GetComponent<DelayBlast>().procCoefficient);
				VoidFlamePrefab.GetComponent<DelayBlast>().procCoefficient = VoidFlameProcCfg.Value;
			}
			catch (Exception ex)
			{
				Debug.Log("Failed to set voidsent flame explosion proc coefficient.");
				Debug.LogError(ex);
			}
		}

		private static void SetLunarSunProc()
		{
			try
			{
				LunarSunPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/LunarSunProjectile");
				//Debug.Log("LunarSun " + LunarSunPrefab.GetComponent<ProjectileController>().procCoefficient);
				LunarSunPrefab.GetComponent<ProjectileController>().procCoefficient = LunarSunProcCfg.Value;
			}
			catch (Exception ex)
			{
				Debug.Log("Failed to set Egocentrism proc coefficient.");
				Debug.LogError(ex);
			}
		}



		private static void SimpleLightningStrikeOrbProc()
		{
			On.RoR2.Orbs.SimpleLightningStrikeOrb.Begin += (orig, self) =>
			{
				if (self.procCoefficient > 0f)
				{
					self.procCoefficient = LightningStrikeProcCfg.Value;
				}

				orig(self);
			};
		}

		private static void BounceOrbProc()
		{
			On.RoR2.Orbs.BounceOrb.Begin += (orig, self) =>
			{
				if (self.procCoefficient > 0f)
				{
					self.procCoefficient = HookProcCfg.Value;
				}

				orig(self);
			};
		}

		private static void LightningOrbProc()
		{
			On.RoR2.Orbs.LightningOrb.Begin += (orig, self) =>
			{
				if (self.procCoefficient > 0f)
				{
					if (self.lightningType == LightningOrb.LightningType.Ukulele) self.procCoefficient = UkuleleProcCfg.Value;
					else if (self.lightningType == LightningOrb.LightningType.Tesla) self.procCoefficient = TeslaProcCfg.Value;
					else if (self.lightningType == LightningOrb.LightningType.RazorWire) self.procCoefficient = RazorProcCfg.Value;
				}

				orig(self);
			};
		}

		private static void DevilOrbProc()
		{
			On.RoR2.Orbs.DevilOrb.Begin += (orig, self) =>
			{
				if (self.procCoefficient > 0f)
				{
					if (self.effectType == DevilOrb.EffectType.Skull) self.procCoefficient = NkuhanaProcCfg.Value;
					else if (self.effectType == DevilOrb.EffectType.Wisp) self.procCoefficient = DiscipleProcCfg.Value;
				}

				orig(self);
			};
		}

		private static void VoidLightningOrbProc()
		{
			On.RoR2.Orbs.VoidLightningOrb.Begin += (orig, self) =>
			{
				if (self.procCoefficient > 0f)
				{
					self.procCoefficient = PolyluteProcCfg.Value;
				}

				orig(self);
			};
		}

		private static void MissileVoidOrbProc()
		{
			On.RoR2.Orbs.MissileVoidOrb.Begin += (orig, self) =>
			{
				if (self.procCoefficient > 0f)
				{
					self.procCoefficient = VoidMissileProcCfg.Value;
				}

				orig(self);
			};
		}



		private static void IcicleProc()
		{
			On.RoR2.IcicleAuraController.Awake += (orig, self) =>
			{
				orig(self);

				self.icicleProcCoefficientPerTick = IcicleProcCfg.Value;
			};
		}

		private static void GasolineProc()
		{
			IL.RoR2.GlobalEventManager.ProcIgniteOnKill += (il) =>
			{
				ILCursor c = new ILCursor(il);

				bool found = c.TryGotoNext(
					x => x.MatchLdcR4(0f),
					x => x.MatchStfld<BlastAttack>("procCoefficient")
				);

				if (found)
				{
					c.Index += 1;

					c.Emit(OpCodes.Pop);
					c.EmitDelegate<Func<float>>(() =>
					{
						return GasolineProcCfg.Value;
					});
				}
				else
				{
					Debug.LogWarning("GasolineProcHook Failed");
				}
			};
		}



		/*
		private static void DebugDrops()
		{
			if (Input.GetKeyDown(KeyCode.F2))
			{
				var transform = PlayerCharacterMasterController.instances[0].master.GetBodyObject().transform;

				CreateDroplet(RoR2Content.Items.ShieldOnly, transform.position + new Vector3(0f, 0f, 0f));
				CreateDroplet(RoR2Content.Equipment.BurnNearby, transform.position + new Vector3(0f, 0f, 0f));



				CreateDroplet(RoR2Content.Items.IgniteOnKill, transform.position + new Vector3(-5f, 5f, 5f));
				CreateDroplet(RoR2Content.Items.CritGlasses, transform.position + new Vector3(-10f, 10f, 10f));

				CreateDroplet(DLC1Content.Items.MushroomVoid, transform.position + new Vector3(0f, 5f, 7.5f));
				CreateDroplet(DLC1Content.Items.MissileVoid, transform.position + new Vector3(0f, 10f, 15f));

				CreateDroplet(RoR2Content.Items.ExplodeOnDeath, transform.position + new Vector3(5f, 5f, 5f));
				CreateDroplet(RoR2Content.Items.SprintArmor, transform.position + new Vector3(10f, 10f, 10f));

				CreateDroplet(RoR2Content.Items.Dagger, transform.position + new Vector3(-5f, 5f, -5f));
				CreateDroplet(RoR2Content.Items.BarrierOnOverHeal, transform.position + new Vector3(-10f, 10f, -10f));

				CreateDroplet(RoR2Content.Items.BleedOnHitAndExplode, transform.position + new Vector3(0f, 5f, -7.5f));
				CreateDroplet(RoR2Content.Items.Knurl, transform.position + new Vector3(0f, 10f, -15f));

				CreateDroplet(RoR2Content.Items.Pearl, transform.position + new Vector3(5f, 5f, -5f));
				CreateDroplet(RoR2Content.Items.ShinyPearl, transform.position + new Vector3(10f, 10f, -10f));
			}
		}

		private static void CreateDroplet(EquipmentDef def, Vector3 pos)
		{
			if (!def) return;

			PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(def.equipmentIndex), pos, Vector3.zero);
		}

		private static void CreateDroplet(ItemDef def, Vector3 pos)
		{
			if (!def) return;

			PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(def.itemIndex), pos, Vector3.zero);
		}
		//*/
	}
}
