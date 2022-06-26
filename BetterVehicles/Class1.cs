using Base.AI;
using Base.AI.Defs;
using Base.Assets;
using Base.Core;
using Base.Defs;
using Base.Entities.Abilities;
using Base.Entities.Effects;
using Base.Entities.Statuses;
using Base.Levels;
using Base.UI;
using Base.Utils.Maths;
using Code.PhoenixPoint.Tactical.Entities.Equipments;
using Harmony;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities;
using PhoenixPoint.Common.Entities.Characters;
using PhoenixPoint.Common.Entities.Equipments;
using PhoenixPoint.Common.Entities.GameTags;
using PhoenixPoint.Common.Entities.GameTagsTypes;
using PhoenixPoint.Common.Entities.Items;
using PhoenixPoint.Common.UI;
using PhoenixPoint.Geoscape.Entities.DifficultySystem;
using PhoenixPoint.Geoscape.Entities.Research;
using PhoenixPoint.Geoscape.Events.Eventus;
using PhoenixPoint.Tactical;
using PhoenixPoint.Tactical.AI;
using PhoenixPoint.Tactical.AI.Actions;
using PhoenixPoint.Tactical.AI.Considerations;
using PhoenixPoint.Tactical.AI.TargetGenerators;
using PhoenixPoint.Tactical.Entities;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.Animations;
using PhoenixPoint.Tactical.Entities.DamageKeywords;
using PhoenixPoint.Tactical.Entities.Effects;
using PhoenixPoint.Tactical.Entities.Effects.DamageTypes;
using PhoenixPoint.Tactical.Entities.Equipments;
using PhoenixPoint.Tactical.Entities.Statuses;
using PhoenixPoint.Tactical.Entities.Weapons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using PhoenixPoint.Geoscape.View.ViewModules;
using I2.Loc;

namespace BetterVehicles
{
    internal class ModConfig
    {
        public bool TurnOnMutogChanges = true;
        public bool FixText = true;
    }
    public static class MyMod
    {
        internal static ModConfig Config;
        internal static string LogPath;
        internal static string ModDirectory;
        internal static string ManagedDirectory;
        internal static string TexturesDirectory;
        internal static string LocalizationDirectory;
        internal static bool doNotLocalize = true;
        internal static readonly DefRepository Repo = GameUtl.GameComponent<DefRepository>();
        internal static readonly SharedData Shared = GameUtl.GameComponent<SharedData>();
        internal static readonly AssetsManager assetsManager = GameUtl.GameComponent<AssetsManager>();
        internal static List<string> ModifiedLocalizationTerms = new List<string>();
        public static void HomeMod(Func<string, object, object> api = null)
        {
            MyMod.Config = ((api("config", null) as ModConfig) ?? new ModConfig());
            HarmonyInstance.Create("BetterVehicles").PatchAll();
            api?.Invoke("log verbose", "Mod Initialised.");

            DefRepository Repo = GameUtl.GameComponent<DefRepository>();
            SharedData Shared = GameUtl.GameComponent<SharedData>();

            ModDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // Path to preset files
            ManagedDirectory = Path.Combine(ModDirectory, "Assets", "Presets");
            // Path to texture files
            TexturesDirectory = Path.Combine(ModDirectory, "Assets", "Textures");
            LocalizationDirectory = Path.Combine(ModDirectory, "Assets", "Localization");

            KillNRun.Change_EP();
            //Mutog.Change_Mutog();

            GroundVehicleWeaponDef ArmadilloFT = Repo.GetAllDefs<GroundVehicleWeaponDef>().FirstOrDefault(a => a.name.Equals("NJ_Armadillo_Mephistopheles_GroundVehicleWeaponDef"));
            GroundVehicleWeaponDef ArmadilloPurgatory = Repo.GetAllDefs<GroundVehicleWeaponDef>().FirstOrDefault(a => a.name.Equals("NJ_Armadillo_Purgatory_GroundVehicleWeaponDef"));
            GroundVehicleWeaponDef ArmadilloGaussTurret = Repo.GetAllDefs<GroundVehicleWeaponDef>().FirstOrDefault(a => a.name.Equals("NJ_Armadillo_Gauss_Turret_GroundVehicleWeaponDef"));

            GroundVehicleWeaponDef Taurus2 = Repo.GetAllDefs<GroundVehicleWeaponDef>().FirstOrDefault(a => a.name.Equals("PX_Scarab_Taurus_GroundVehicleWeaponDef"));

            WeaponDef fullStop = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(a => a.name.Equals("KS_Buggy_Fullstop_WeaponDef"));
            WeaponDef screamer = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(a => a.name.Equals("KS_Buggy_Screamer_WeaponDef"));
            WeaponDef vishnu = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(a => a.name.Equals("KS_Buggy_The_Vishnu_Gun_Cannon_WeaponDef"));
            WeaponDef fullStopMiniGun = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(a => a.name.Equals("KS_Buggy_Minigun_Fullstop_WeaponDef"));
            WeaponDef screamerMiniGun = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(a => a.name.Equals("KS_Buggy_Minigun_Screamer_WeaponDef"));
            WeaponDef vishnuMiniGun = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(a => a.name.Equals("KS_Buggy_Minigun_Vishnu_WeaponDef"));

            GroundVehicleModuleDef revisedArmor = Repo.GetAllDefs<GroundVehicleModuleDef>().FirstOrDefault(a => a.name.Equals("KS_Kaos_Buggy_Revised_Armor_Plating_Hull_GroundVehicleModuleDef"));
            GroundVehicleModuleDef spikedArmor = Repo.GetAllDefs<GroundVehicleModuleDef>().FirstOrDefault(a => a.name.Equals("KS_Kaos_Buggy_Spiked_Armor_Plating_Hull_GroundVehicleModuleDef"));
            GroundVehicleModuleDef experimentalExhaust = Repo.GetAllDefs<GroundVehicleModuleDef>().FirstOrDefault(a => a.name.Equals("KS_Kaos_Buggy_Experimental_Exhaust_System_Engine_GroundVehicleModuleDef"));
            GroundVehicleModuleDef reinforcedPlating = Repo.GetAllDefs<GroundVehicleModuleDef>().FirstOrDefault(a => a.name.Equals("NJ_Armadillo_Reinforced_Plating_Hull_GroundVehicleModuleDef"));
            GroundVehicleModuleDef lightAlloy = Repo.GetAllDefs<GroundVehicleModuleDef>().FirstOrDefault(a => a.name.Equals("NJ_Armadillo_Lightweight_Alloy_Plating_Hull_GroundVehicleModuleDef"));
            GroundVehicleModuleDef superCharger = Repo.GetAllDefs<GroundVehicleModuleDef>().FirstOrDefault(a => a.name.Equals("NJ_Armadillo_Supercharger_GroundVehicleModuleDef"));
            GroundVehicleModuleDef cargoRacks = Repo.GetAllDefs<GroundVehicleModuleDef>().FirstOrDefault(a => a.name.Equals("PX_Scarab_Reinforced_Cargo_Racks_GroundVehicleModuleDef"));
            GroundVehicleModuleDef improvedChasis = Repo.GetAllDefs<GroundVehicleModuleDef>().FirstOrDefault(a => a.name.Equals("SY_Aspida_Improved_Chassis_GroundVehicleModuleDef"));

            GroundVehicleModuleDef vishnuModule = Repo.GetAllDefs<GroundVehicleModuleDef>().FirstOrDefault(a => a.name.Equals("KS_Buggy_The_Vishnu_Gun_GroundVehicleModuleDef"));

            TacticalItemDef revisedLeftTire = Repo.GetAllDefs<TacticalItemDef>().FirstOrDefault(a => a.name.Equals("KS_Kaos_Buggy_Revised_Armor_Plating_LeftFrontTyre_BodyPartDef"));
            TacticalItemDef revisedRightTire = Repo.GetAllDefs<TacticalItemDef>().FirstOrDefault(a => a.name.Equals("KS_Kaos_Buggy_Revised_Armor_Plating_RightFrontTyre_BodyPartDef"));
            TacticalItemDef spikedLeftFrontTire = Repo.GetAllDefs<TacticalItemDef>().FirstOrDefault(a => a.name.Equals("KS_Kaos_Buggy_Spiked_Armor_LeftFrontTyre_BodyPartDef"));
            TacticalItemDef spikedLeftBackTire = Repo.GetAllDefs<TacticalItemDef>().FirstOrDefault(a => a.name.Equals("KS_Kaos_Buggy_Spiked_Armor_LeftBackTyre_BodyPartDef"));
            TacticalItemDef spikedRightFrontTire = Repo.GetAllDefs<TacticalItemDef>().FirstOrDefault(a => a.name.Equals("KS_Kaos_Buggy_Spiked_Armor_RightFrontTyre_BodyPartDef"));

            TacticalItemDef KaosBuggyTop = Repo.GetAllDefs<TacticalItemDef>().FirstOrDefault(a => a.name.Equals("KS_Kaos_Buggy_Top_BodyPartDef"));
            TacticalItemDef KaosBuggyFront = Repo.GetAllDefs<TacticalItemDef>().FirstOrDefault(a => a.name.Equals("KS_Kaos_Buggy_Front_BodyPartDef"));
            TacticalItemDef KaosBuggyLeft = Repo.GetAllDefs<TacticalItemDef>().FirstOrDefault(a => a.name.Equals("KS_Kaos_Buggy_Left_BodyPartDef"));
            TacticalItemDef KaosBuggyRight = Repo.GetAllDefs<TacticalItemDef>().FirstOrDefault(a => a.name.Equals("KS_Kaos_Buggy_Right_BodyPartDef"));
            TacticalItemDef KaosBuggyBack = Repo.GetAllDefs<TacticalItemDef>().FirstOrDefault(a => a.name.Equals("KS_Kaos_Buggy_Back_BodyPartDef"));

            TacticalItemDef KaosBuggyRearTyre = Repo.GetAllDefs<TacticalItemDef>().FirstOrDefault(a => a.name.Equals("KS_Kaos_Buggy_RearTyre_BodyPartDef"));
            TacticalItemDef KaosBuggyFrontLeftTyre = Repo.GetAllDefs<TacticalItemDef>().FirstOrDefault(a => a.name.Equals("KS_Kaos_Buggy_LeftFrontTyre_BodyPartDef"));
            TacticalItemDef KaosBuggyFrontRightTyre = Repo.GetAllDefs<TacticalItemDef>().FirstOrDefault(a => a.name.Equals("KS_Kaos_Buggy_RightFrontTyre_BodyPartDef"));

            KaosBuggyTop.HitPoints = 220;
            KaosBuggyFront.HitPoints = 280;
            KaosBuggyFront.Armor = 40;
            KaosBuggyLeft.HitPoints = 200;
            KaosBuggyLeft.Armor = 20;
            KaosBuggyRight.HitPoints = 200;
            KaosBuggyRight.Armor = 20;
            KaosBuggyBack.HitPoints = 200;
            KaosBuggyBack.Armor = 20;

            KaosBuggyRearTyre.HitPoints = 180;
            KaosBuggyRearTyre.Armor = 10;
            KaosBuggyRearTyre.BodyPartAspectDef.Speed = 13;
            KaosBuggyFrontLeftTyre.HitPoints = 180;
            KaosBuggyFrontLeftTyre.Armor = 10;
            KaosBuggyFrontRightTyre.HitPoints = 180;
            KaosBuggyFrontRightTyre.Armor = 10;

            vishnuModule.BodyPartAspectDef.Endurance = 1;
            KaosBuggyTop.BodyPartAspectDef.Endurance = 19;
            KaosBuggyFront.BodyPartAspectDef.Endurance = 20;
            KaosBuggyLeft.BodyPartAspectDef.Endurance = 18;
            KaosBuggyRight.BodyPartAspectDef.Endurance = 18;
            KaosBuggyBack.BodyPartAspectDef.Endurance = 19;

            foreach (GroundVehicleWeaponDef groundvehiclweapon in Repo.GetAllDefs<GroundVehicleWeaponDef>().Where(a => a.name.Contains("GroundVehicleWeaponDef")))
            {
                groundvehiclweapon.HitPoints *= 2;
                groundvehiclweapon.Armor *= 2;
            }

            foreach (WeaponDef groundvehiclweapon in Repo.GetAllDefs<WeaponDef>().Where(a => a.name.Contains("KS_Buggy_Minigun_") || a.name.Equals(fullStop.name) ||
             a.name.Equals(screamer.name) || a.name.Equals(vishnu.name)))
            {
                groundvehiclweapon.HitPoints *= 2;
                groundvehiclweapon.Armor *= 2;
            }

            foreach (WeaponDef groundvehiclweapon in Repo.GetAllDefs<WeaponDef>().Where(a => a.name.Contains("KS_Buggy_Minigun_")))
            {
                groundvehiclweapon.DamagePayload.AutoFireShotCount = 6;
                groundvehiclweapon.ChargesMax = 60;
            }

            fullStop.ChargesMax = 3;
            Taurus2.ChargesMax = 8;
            ArmadilloFT.ChargesMax = 10;
            ArmadilloPurgatory.ChargesMax = 6;
            revisedLeftTire.BodyPartAspectDef.Speed = 0;
            revisedRightTire.BodyPartAspectDef.Speed = 0;
            if(Config.FixText == true)
            {
                string text10 = "DOES NOT ADD ARMOR, adds +250 HP and +20 Armor to Wheels";
                revisedArmor.ViewElementDef.Description = new LocalizedTextBind(text10, true);
                spikedArmor.ViewElementDef.Description = new LocalizedTextBind(text10, true);
                MyMod.ModifiedLocalizationTerms.Add(text10);
            }        
            spikedLeftBackTire.BodyPartAspectDef.Speed = 0;
            spikedLeftFrontTire.BodyPartAspectDef.Speed = 0;
            spikedRightFrontTire.BodyPartAspectDef.Speed = 0;
            


            BodyPartAspectDef LWA = (BodyPartAspectDef)lightAlloy.BodyPartAspectDef;
            LWA.StatModifications[0].Value = 6;
            
            BodyPartAspectDef IC = (BodyPartAspectDef)improvedChasis.BodyPartAspectDef;
            IC.Speed = 4;

            BodyPartAspectDef CR = (BodyPartAspectDef)cargoRacks.BodyPartAspectDef;
            CR.StatModifications[0].Value = 9;

            BodyPartAspectDef RPBPAD = (BodyPartAspectDef)reinforcedPlating.BodyPartAspectDef;
            RPBPAD.StatModifications = new ItemStatModification[]
            {
                new ItemStatModification()
                {
                    TargetStat = StatModificationTarget.UnitsInside,
                    Modification = StatModificationType.Add,
                    Value = 1,
                },
            };
            
            ArmadilloGaussTurret.Abilities = new AbilityDef[]
            {
                ArmadilloGaussTurret.Abilities[0],
                Repo.GetAllDefs<AbilityDef>().FirstOrDefault(a => a.name.Equals("ReturnFire_AbilityDef")),
            };

            superCharger.Abilities = new AbilityDef[]
            {
                Repo.GetAllDefs<AbilityDef>().FirstOrDefault(a => a.name.Equals("GooImmunity_AbilityDef")),
            };                      
        }
        public static void MainMod(Func<string, object, object> api)
        {
            HarmonyInstance.Create("your.mod.id").PatchAll();
            api("log verbose", "Mod Initialised.");
        }
        
        [HarmonyPatch(typeof(LocalizationManager), "TryGetTranslation")]
        public static class LocalizationManager_TryGetTranslation_Patch
        {
            // Token: 0x060000CA RID: 202 RVA: 0x0000ABF2 File Offset: 0x00008DF2
            public static bool Prepare()
            {
                return BetterVehicles.MyMod.Config.FixText;
            }

            // Token: 0x060000CB RID: 203 RVA: 0x0000B0B4 File Offset: 0x000092B4
            public static void Postfix(bool __result, string Term, ref string Translation)
            {
                try
                {
                    if (!__result)
                    {
                        if (!string.IsNullOrEmpty(Term) && MyMod.ModifiedLocalizationTerms.Contains(Term))
                        {                           
                            Translation = Term;
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }
    }
}
