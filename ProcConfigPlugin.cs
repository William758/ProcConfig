using System;
using System.Linq;
using UnityEngine;
using BepInEx;
using BepInEx.Configuration;
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

	public class ZetTweaksPlugin : BaseUnityPlugin
	{
		public const string ModVer = "1.0.0";
		public const string ModName = "ProcConfig";
		public const string ModGuid = "com.TPDespair.ProcConfig";



		public static GameObject DaggerPrefab;
		public static GameObject MissilePrefab;
		public static GameObject FireworkPrefab;
		public static GameObject BleedExplodePrefab;
		public static GameObject WillowispPrefab;
		public static GameObject DiskPrefab;
		public static GameObject MeatballPrefab;



		public static ConfigEntry<bool> EnableModuleCfg { get; set; }
		public static ConfigEntry<float> DaggerProcCfg { get; set; }
		public static ConfigEntry<float> MissileProcCfg { get; set; }
		public static ConfigEntry<float> FireworkProcCfg { get; set; }
		public static ConfigEntry<float> SpleenProcCfg { get; set; }
		public static ConfigEntry<float> WillowProcCfg { get; set; }
		public static ConfigEntry<float> DiscBeamProcCfg { get; set; }
		public static ConfigEntry<float> DiscExplosionProcCfg { get; set; }
		public static ConfigEntry<float> MeatballProcCfg { get; set; }
		public static ConfigEntry<float> LightningStrikeProcCfg { get; set; }
		public static ConfigEntry<float> HookProcCfg { get; set; }
		public static ConfigEntry<float> UkuleleProcCfg { get; set; }
		public static ConfigEntry<float> TeslaProcCfg { get; set; }
		public static ConfigEntry<float> RazorProcCfg { get; set; }
		public static ConfigEntry<float> DiscipleProcCfg { get; set; }
		public static ConfigEntry<float> NkuhanaProcCfg { get; set; }
		public static ConfigEntry<float> IcicleProcCfg { get; set; }



		public void Awake()
		{
			RoR2Application.isModded = true;
			NetworkModCompatibilityHelper.networkModList = NetworkModCompatibilityHelper.networkModList.Append(ModGuid + ":" + ModVer);

			SetupConfig(Config);
			OnLogBookControllerReady();
		}



		private static void SetupConfig(ConfigFile Config)
		{
			EnableModuleCfg = Config.Bind(
				"0-Proc - Enable", "enableProcModule", true,
				"Enable Proc Module."
			);

			DaggerProcCfg = Config.Bind(
				"1-Proc - Prefabs", "daggerProc", 0f,
				"Procoeff of Ceremonial Dagger. Vanilla is 1"
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

			IcicleProcCfg = Config.Bind(
				"3-Proc - Other", "icicleProc", 0.1f,
				"Procoeff of Frost Relic. Vanilla is 0.2"
			);
		}



		private static void OnLogBookControllerReady()
		{
			On.RoR2.UI.LogBook.LogBookController.Init += (orig) =>
			{
				SetProcCoefficients();

				orig();
			};
		}

		private static void SetProcCoefficients()
		{
			if (EnableModuleCfg.Value)
			{
				SetDaggerProc();
				SetMissileProc();
				SetFireworkProc();
				SetSpleenDelayedProc();
				SetWillowispDelayedProc();
				SetDiscProc();
				SetMeatballProc();

				SimpleLightningStrikeOrbProc();
				BounceOrbProc();
				LightningOrbProc();
				DevilOrbProc();

				IcicleProc();
			}
		}



		private static void SetDaggerProc()
		{
			try
			{
				DaggerPrefab = Resources.Load<GameObject>("prefabs/projectiles/DaggerProjectile");
				DaggerPrefab.GetComponent<ProjectileController>().procCoefficient = DaggerProcCfg.Value;
			}
			catch (Exception ex)
			{
				Debug.Log("Failed to set dagger proc coefficient.");
				Debug.LogError(ex);
			}
		}

		private static void SetMissileProc()
		{
			try
			{
				MissilePrefab = Resources.Load<GameObject>("prefabs/projectiles/MissileProjectile");
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
				FireworkPrefab = Resources.Load<GameObject>("prefabs/projectiles/FireworkProjectile");
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
				BleedExplodePrefab = Resources.Load<GameObject>("prefabs/networkedobjects/BleedOnHitAndExplodeDelay");
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
				WillowispPrefab = Resources.Load<GameObject>("prefabs/networkedobjects/WilloWispDelay");
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
				DiskPrefab = Resources.Load<GameObject>("prefabs/projectiles/LaserTurbineBomb");
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
				MeatballPrefab = Resources.Load<GameObject>("Prefabs/Projectiles/FireMeatBall");
				MeatballPrefab.GetComponent<ProjectileImpactExplosion>().blastProcCoefficient = MeatballProcCfg.Value;
			}
			catch (Exception ex)
			{
				Debug.Log("Failed to set molten perforator proc coefficient.");
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



		private static void IcicleProc()
		{
			On.RoR2.IcicleAuraController.Awake += (orig, self) =>
			{
				orig(self);

				self.icicleProcCoefficientPerTick = IcicleProcCfg.Value;
			};
		}
	}
}