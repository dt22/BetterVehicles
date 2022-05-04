using Base.AI;
using Base.AI.Defs;
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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BetterVehicles
{
    internal class Clone
    {
        public static T CreateDefFromClone<T>(T source, string guid, string name) where T : BaseDef
        {
            DefRepository Repo = GameUtl.GameComponent<DefRepository>();
            try
            {
                if (Repo.GetDef(guid) != null)
                {
                    if (!(Repo.GetDef(guid) is T tmp))
                    {
                        throw new TypeAccessException($"An item with the GUID <{guid}> has already been added to the Repo, but the type <{Repo.GetDef(guid).GetType().Name}> does not match <{typeof(T).Name}>!");
                    }
                    else
                    {
                        if (tmp != null)
                        {
                            return tmp;
                        }
                    }
                }
                T tmp2 = Repo.GetRuntimeDefs<T>(true).FirstOrDefault(rt => rt.Guid.Equals(guid));
                if (tmp2 != null)
                {
                    return tmp2;
                }
                Type type = null;
                string resultName = "";
                if (source != null)
                {
                    int start = source.name.IndexOf('[') + 1;
                    int end = source.name.IndexOf(']');
                    string toReplace = !name.Contains("[") && start > 0 && end > start ? source.name.Substring(start, end - start) : source.name;
                    resultName = source.name.Replace(toReplace, name);
                }
                else
                {
                    type = typeof(T);
                    resultName = name;
                }
                T result = (T)Repo.CreateRuntimeDef(
                    source,
                    type,
                    guid);
                result.name = resultName;
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
    public static class MyMod
    {
        public static void HomeMod(Func<string, object, object> api = null)
        {         
            HarmonyInstance.Create("your.mod.id").PatchAll();
            api?.Invoke("log verbose", "Mod Initialised.");

            DefRepository Repo = GameUtl.GameComponent<DefRepository>();
            SharedData Shared = GameUtl.GameComponent<SharedData>();

            KillNRun.Change_EP();

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
            
            WeaponDef mutogVenomHead = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(a => a.name.Equals("Mutog_HeadPoison_WeaponDef"));
            WeaponDef mutogSlashTail = Repo.GetAllDefs<WeaponDef>().FirstOrDefault(a => a.name.Equals("Mutog_Tail_Bladed_WeaponDef"));
            TacticalItemDef mutogAgileLegs = Repo.GetAllDefs<TacticalItemDef>().FirstOrDefault(a => a.name.Equals("Mutog_RearRightLeg_Agile_BodyPartDef"));

            GroundVehicleModuleDef revisedArmor = Repo.GetAllDefs<GroundVehicleModuleDef>().FirstOrDefault(a => a.name.Equals("KS_Kaos_Buggy_Revised_Armor_Plating_Hull_GroundVehicleModuleDef"));
            GroundVehicleModuleDef spikedArmor = Repo.GetAllDefs<GroundVehicleModuleDef>().FirstOrDefault(a => a.name.Equals("KS_Kaos_Buggy_Spiked_Armor_Plating_Hull_GroundVehicleModuleDef"));
            GroundVehicleModuleDef experimentalExhaust = Repo.GetAllDefs<GroundVehicleModuleDef>().FirstOrDefault(a => a.name.Equals("KS_Kaos_Buggy_Experimental_Exhaust_System_Engine_GroundVehicleModuleDef"));
            GroundVehicleModuleDef reinforcedPlating = Repo.GetAllDefs<GroundVehicleModuleDef>().FirstOrDefault(a => a.name.Equals("NJ_Armadillo_Reinforced_Plating_Hull_GroundVehicleModuleDef"));
            GroundVehicleModuleDef lightAlloy = Repo.GetAllDefs<GroundVehicleModuleDef>().FirstOrDefault(a => a.name.Equals("NJ_Armadillo_Lightweight_Alloy_Plating_Hull_GroundVehicleModuleDef"));
            GroundVehicleModuleDef superCharger = Repo.GetAllDefs<GroundVehicleModuleDef>().FirstOrDefault(a => a.name.Equals("NJ_Armadillo_Supercharger_GroundVehicleModuleDef"));
            GroundVehicleModuleDef cargoRacks = Repo.GetAllDefs<GroundVehicleModuleDef>().FirstOrDefault(a => a.name.Equals("PX_Scarab_Reinforced_Cargo_Racks_GroundVehicleModuleDef"));
            GroundVehicleModuleDef improvedChasis = Repo.GetAllDefs<GroundVehicleModuleDef>().FirstOrDefault(a => a.name.Equals("SY_Aspida_Improved_Chassis_GroundVehicleModuleDef"));

            foreach(GroundVehicleWeaponDef groundvehiclweapon in Repo.GetAllDefs<GroundVehicleWeaponDef>().Where(a => a.name.Contains("GroundVehicleWeaponDef")))
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

            foreach (GroundVehicleWeaponDef groundvehiclweapon in Repo.GetAllDefs<GroundVehicleWeaponDef>().Where(a => a.name.Contains("KS_Buggy_Minigun_")))
            {
                groundvehiclweapon.DamagePayload.AutoFireShotCount = 6;
                groundvehiclweapon.ChargesMax = 60;
            }

            fullStop.ChargesMax = 3;
            Taurus2.ChargesMax = 8;
            ArmadilloFT.ChargesMax = 10;
            ArmadilloPurgatory.ChargesMax = 6;

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

            mutogSlashTail.DamagePayload.DamageKeywords = new List<DamageKeywordPair>
            {
                mutogSlashTail.DamagePayload.DamageKeywords[0],
                mutogSlashTail.DamagePayload.DamageKeywords[1],
                new DamageKeywordPair{DamageKeywordDef = Shared.SharedDamageKeywords.PiercingKeyword, Value = 20 },
            };

            mutogVenomHead.DamagePayload.DamageKeywords[1].Value = 80;
            //mutogVenomHead.UseAimIK = true;
            BodyPartAspectDef MAL = (BodyPartAspectDef)mutogAgileLegs.BodyPartAspectDef;
            MAL.Speed = 11;

            FrenzyStatusDef frenzyStatusDef = Repo.GetAllDefs<FrenzyStatusDef>().FirstOrDefault((FrenzyStatusDef w) => w.name == "Mutog_Enraged_StatusDef");
            frenzyStatusDef.SpeedCoefficient = 1f;
            HealthConditionStatusDef healthConditionStatusDef = Repo.GetAllDefs<HealthConditionStatusDef>().FirstOrDefault((HealthConditionStatusDef w) => w.name == "Mutog_HealthCondition_StatusDef");
            healthConditionStatusDef.Thresholds[0].HealthRatioThreshold = 0.3f;

            int usesPerTurn = 1;
            float actionPointCost = 0.5f;
            int num = 3;
            int num2 = 10;
            bool targetFirstDamagedBodypart = false;
            foreach (ItemDef itemDef in from ad in Repo.GetAllDefs<ItemDef>()
                                        where ad.name.Contains("Leg_Regenerating") && ad.name.Contains("BodyPartDef")
                                        select ad)
            {
                itemDef.Armor = 25f;
                itemDef.HitPoints = 160f;
            }
            TacticalActorDef tacticalActorDef = Repo.GetAllDefs<TacticalActorDef>().FirstOrDefault((TacticalActorDef ta) => ta.name.Equals("Mutog_ActorDef"));
            ApplyStatusAbilityDef item = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault((ApplyStatusAbilityDef ad) => ad.name.Equals("Unbreakable_AbilityDef"));
            ApplyStatusAbilityDef applyStatusAbilityDef = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault((ApplyStatusAbilityDef w) => w.name == "Mutog_Regeneration_AbilityDef");
            AbilityDef item2 = Repo.GetAllDefs<AbilityDef>().FirstOrDefault((AbilityDef w) => w.name == "GroundStomp_AbilityDef");
            AbilityDef item3 = Repo.GetAllDefs<AbilityDef>().FirstOrDefault((AbilityDef w) => w.name == "Mutog_Ram_AbilityDef");
            RamAbilityDef ramAbilityDef = Repo.GetAllDefs<RamAbilityDef>().FirstOrDefault((RamAbilityDef w) => w.name == "Mutog_Ram_AbilityDef");
            ItemDef itemDef2 = Repo.GetAllDefs<ItemDef>().FirstOrDefault((ItemDef w) => w.name.Contains("Mutog_RearRightLeg_Regenerating_BodyPartDef"));
            GooDamageMultiplierAbilityDef item4 = Repo.GetAllDefs<GooDamageMultiplierAbilityDef>().FirstOrDefault((GooDamageMultiplierAbilityDef w) => w.name == "GooImmunity_AbilityDef");
            DamageMultiplierAbilityDef item5 = Repo.GetAllDefs<DamageMultiplierAbilityDef>().FirstOrDefault((DamageMultiplierAbilityDef w) => w.name.Equals("AcidResistant_DamageMultiplierAbilityDef"));
            HealthChangeStatusDef healthChangeStatusDef = Repo.GetAllDefs<HealthChangeStatusDef>().FirstOrDefault((HealthChangeStatusDef w) => w.name.Equals("E_Status [Mutog_Regeneration_AbilityDef]"));
            tacticalActorDef.ShouldStopMovingOnActorSpotted = false;
            ramAbilityDef.UsesPerTurn = usesPerTurn;
            ramAbilityDef.ActionPointCost = actionPointCost;
            ramAbilityDef.WillPointCost = (float)num;
            ramAbilityDef.RamForce = 600;
            ramAbilityDef.ViewElementDef.DisplayName1 = new LocalizedTextBind("FURIOUS CHARGE", false);
            ramAbilityDef.ViewElementDef.Description = new LocalizedTextBind("Charge forward ramming everything in the way for the cost of AP with distance", false);
            healthChangeStatusDef.HealthChangeAmount = (float)num2;
            healthChangeStatusDef.TargetFirstDamagedBodypart = targetFirstDamagedBodypart;
            applyStatusAbilityDef.ViewElementDef.Description = new LocalizedTextBind("Each leg restores 10 Hit Points to all injured body parts.", false);
            List<AbilityDef> list = itemDef2.Abilities.ToList<AbilityDef>();
            list.Add(item3);
            list.Add(item2);
            list.Add(item5);
            list.Add(item4);
            list.Add(item);
            itemDef2.Abilities = list.ToArray();
        }
        public static void MainMod(Func<string, object, object> api)
        {
            HarmonyInstance.Create("your.mod.id").PatchAll();
            api("log verbose", "Mod Initialised.");
        }
    }
}
