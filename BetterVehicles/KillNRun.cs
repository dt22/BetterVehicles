using Base.Cameras.ExecutionNodes;
using Base.Core;
using Base.Defs;
using Base.Entities.Abilities;
using Base.Entities.Effects;
using Base.Entities.Statuses;
using Base.UI;
using Harmony;
using PhoenixPoint.Common.Core;
using PhoenixPoint.Common.Entities;
using PhoenixPoint.Common.Entities.Equipments;
using PhoenixPoint.Common.Entities.GameTags;
using PhoenixPoint.Common.Entities.GameTagsTypes;
using PhoenixPoint.Common.UI;
using PhoenixPoint.Tactical.Cameras.Filters;
using PhoenixPoint.Tactical.Entities.Abilities;
using PhoenixPoint.Tactical.Entities.Animations;
using PhoenixPoint.Tactical.Entities.Statuses;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace BetterVehicles
{
    internal class KillNRun
    {
       public static void Change_EP()
        {
            DefRepository Repo = GameUtl.GameComponent<DefRepository>();
            GroundVehicleModuleDef experimentalExhaust = Repo.GetAllDefs<GroundVehicleModuleDef>().FirstOrDefault(a => a.name.Equals("KS_Kaos_Buggy_Experimental_Exhaust_System_Engine_GroundVehicleModuleDef"));
            string skillName = "KillAndRunVehicle_AbilityDef";        

            // Source to clone from for main ability: Inspire
            ApplyStatusAbilityDef inspireAbility = Repo.GetAllDefs<ApplyStatusAbilityDef>().FirstOrDefault(a => a.name.Equals("Inspire_AbilityDef"));

            // Create Neccessary RuntimeDefs
            ApplyStatusAbilityDef killAndRunAbility = Clone.CreateDefFromClone(
                inspireAbility,
                "af77ed60-254d-4be6-adf8-91ca972d1e39",
                skillName);
            AbilityCharacterProgressionDef progression = Clone.CreateDefFromClone(
                inspireAbility.CharacterProgressionData,
                "52bdb6ed-7544-4d2a-af4f-eb199ab68fb0",
                skillName);
            TacticalAbilityViewElementDef viewElement = Clone.CreateDefFromClone(
                inspireAbility.ViewElementDef,
                "861dd580-f206-4e21-98c3-c846a6071f03",
                skillName);
            OnActorDeathEffectStatusDef onActorDeathEffectStatus = Clone.CreateDefFromClone(
                inspireAbility.StatusDef as OnActorDeathEffectStatusDef,
                "1f5d9d57-a777-43a2-8026-1755a66fd4b2",
                "E_KillListenerStatus [" + skillName + "]");
            RepositionAbilityDef dashAbility = Clone.CreateDefFromClone( // Create an own Dash ability from standard Dash
                Repo.GetAllDefs<RepositionAbilityDef>().FirstOrDefault(r => r.name.Equals("Dash_AbilityDef")),
                "6a35bee7-3201-4333-b0e3-00ffdc0fd025",
                "KillAndRun_Dash_AbilityDef");
            TacticalTargetingDataDef dashTargetingData = Clone.CreateDefFromClone( // ... and clone its targeting data
                Repo.GetAllDefs<TacticalTargetingDataDef>().FirstOrDefault(t => t.name.Equals("E_TargetingData [Dash_AbilityDef]")),
                "503e2edc-4c31-4762-b8ca-fd1a7f60af8a",
                "KillAndRun_Dash_AbilityDef");
            StatusRemoverEffectDef statusRemoverEffect = Clone.CreateDefFromClone( // Borrow effect from Manual Control
                Repo.GetAllDefs<StatusRemoverEffectDef>().FirstOrDefault(a => a.name.Equals("E_RemoveStandBy [ManualControlStatus]")),
                "60275a1e-6caf-48c1-b24c-cc9e33a103e2",
                "E_StatusRemoverEffect [" + skillName + "]");
            AddAbilityStatusDef addAbiltyStatus = Clone.CreateDefFromClone( // Borrow status from Deplay Beacon (final mission)
                Repo.GetAllDefs<AddAbilityStatusDef>().FirstOrDefault(a => a.name.Equals("E_AddAbilityStatus [DeployBeacon_StatusDef]")),
                "519423f6-b41a-4409-a48f-b5113efe61ac",
                skillName);
            MultiStatusDef multiStatus = Clone.CreateDefFromClone( // Borrow multi status from Rapid Clearance
                Repo.GetAllDefs<MultiStatusDef>().FirstOrDefault(m => m.name.Equals("E_MultiStatus [RapidClearance_AbilityDef]")),
                "d8af0b40-94f9-4c2a-a841-796469998d86",
                skillName);
            FirstMatchExecutionDef cameraAbility = Clone.CreateDefFromClone(
                Repo.GetAllDefs<FirstMatchExecutionDef>().FirstOrDefault(bd => bd.name.Equals("E_DashCameraAbility [NoDieCamerasTacticalCameraDirectorDef]")),
                "20f5659c-890a-4f29-9968-07ea67b04c6b",
                "E_KnR_Dash_CameraAbility [NoDieCamerasTacticalCameraDirectorDef]");
            cameraAbility.FilterDef = Clone.CreateDefFromClone(
                Repo.GetAllDefs<TacCameraAbilityFilterDef>().FirstOrDefault(c => c.name.Equals("E_DashAbilityFilter [NoDieCamerasTacticalCameraDirectorDef]")),
                "64ba51e9-c67b-4e5e-ad61-315e7f796ffa",
                "E_KnR_Dash_CameraAbilityFilter [NoDieCamerasTacticalCameraDirectorDef]");
            (cameraAbility.FilterDef as TacCameraAbilityFilterDef).TacticalAbilityDef = dashAbility;

            // Add new KnR Dash ability to animation action handler for dash (same animation)
            foreach (TacActorSimpleAbilityAnimActionDef def in Repo.GetAllDefs<TacActorSimpleAbilityAnimActionDef>().Where(b => b.name.Contains("Dash")))
            {
                if (!def.AbilityDefs.Contains(dashAbility))
                {
                    def.AbilityDefs = def.AbilityDefs.Append(dashAbility).ToArray();
                }
            }

            // Set fields
            killAndRunAbility.CharacterProgressionData = progression;
            killAndRunAbility.ViewElementDef = viewElement;
            killAndRunAbility.SkillTags = new SkillTagDef[0];
            killAndRunAbility.StatusDef = multiStatus;
            killAndRunAbility.StatusApplicationTrigger = StatusApplicationTrigger.StartTurn;

            viewElement.DisplayName1 = new LocalizedTextBind("KILL'N'RUN", false);
            viewElement.Description = new LocalizedTextBind("Once per turn, take a free move after killing an enemy.", false);
            
           
            viewElement.ShowInStatusScreen = true;
            viewElement.HideFromPassives = true;

            dashAbility.TargetingDataDef = dashTargetingData;
            dashAbility.TargetingDataDef.Origin.Range = 14.0f;

            dashAbility.ViewElementDef = Clone.CreateDefFromClone(
                inspireAbility.ViewElementDef,
                "2e5aaf9d-21b3-4857-8e98-6df883654506",
                "KillAndRun_Dash_AbilityDef");
            dashAbility.ViewElementDef.DisplayName1 = viewElement.DisplayName1;
            dashAbility.ViewElementDef.Description = viewElement.Description;
            
            dashAbility.ViewElementDef.ShowInStatusScreen = false;
            dashAbility.ViewElementDef.HideFromPassives = true;
            dashAbility.ViewElementDef.ShouldFlash = true;

            dashAbility.SuppressAutoStandBy = true;
            dashAbility.DisablingStatuses = new StatusDef[] { onActorDeathEffectStatus };
            dashAbility.UsesPerTurn = 1;
            dashAbility.ActionPointCost = 0.0f;
            dashAbility.WillPointCost = 0.0f;
            dashAbility.SamePositionIsValidTarget = true;
            dashAbility.AmountOfMovementToUseAsRange = -1.0f;

            multiStatus.Statuses = new StatusDef[] { onActorDeathEffectStatus, addAbiltyStatus };

            onActorDeathEffectStatus.EffectName = "KnR_KillTriggerListener";
            onActorDeathEffectStatus.Visuals = viewElement;
            onActorDeathEffectStatus.VisibleOnPassiveBar = true;
            onActorDeathEffectStatus.DurationTurns = 0;
            onActorDeathEffectStatus.EffectDef = statusRemoverEffect;

            statusRemoverEffect.StatusToRemove = "KnR_KillTriggerListener";

            addAbiltyStatus.DurationTurns = 0;
            addAbiltyStatus.SingleInstance = true;
            addAbiltyStatus.AbilityDef = dashAbility;

            experimentalExhaust.Abilities = new AbilityDef[]
            {
                Repo.GetAllDefs<AbilityDef>().FirstOrDefault(a => a.name.Equals("KillAndRunVehicle_AbilityDef")),
            };
        }
        // Harmony Patch to diplay the KnR Dash ability after kill has been achieved
        [HarmonyPatch(typeof(TacticalAbility), "get_ShouldDisplay")]
        internal static class BC_TacticalAbility_get_ShouldDisplay_Patch
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051")]
            private static void Postfix(TacticalAbility __instance, ref bool __result)
            {
                // Check if instance is KnR ability
                if (__instance.TacticalAbilityDef.name.Equals("KillAndRun_Dash_AbilityDef"))
                {
                    //  Set return value __result = true when ability is not disabled => show
                    __result = __instance.GetDisabledState() == AbilityDisabledState.NotDisabled;
                }
            }
        }
    }
}
