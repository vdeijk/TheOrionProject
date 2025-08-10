using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.UI;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.Game
{
    [DefaultExecutionOrder(200)]
    public class ControlModeManager
    {
        private static ControlModeManager _instance;
        private GameController controller;

        public GameControlType gameControlType { get; private set; } = GameControlType.Outside;
        public bool isPreparing { get; private set; }
        public static ControlModeManager Instance => _instance ??= new ControlModeManager();
        private DurationData durationData => DurationData.Instance;

        public static event EventHandler OnGameModeChanged;

        public void Init(GameController gameController)
        {
            controller = gameController;
        }

        // Handles transition to salvage mode, including camera and UI changes
        public void EnterSalvageMode(UnitSingleController unit)
        {
            CameraChangeMonobService.Instance.ChangeCamera(Data.CameraType.Salvage);
            SalvageService.Instance.Setup();
            CamerasSalvageService.Instance.SetSalvagableUnit(unit);
            MenuChangeMonobService.Instance.ToSalvageMenu();
            TimeScaleManager.Instance.SetTimeScaleToZero();

            SFXMonobService.Instance.PlaySFX(SFXType.StartPhase);

            PopupFadingUI.Instance.FadeIn(TutorialType.Salvage);
        }

        // Switches to action mode for combat
        public void EnterActionMode()
        {
            TimeScaleManager.Instance.SetTimeScaleNormal();
            CameraChangeMonobService.Instance.ChangeCamera(Data.CameraType.Action);
        }

        // Switches to repair mode for unit maintenance
        public void EnterRepairMode()
        {
            SelectModeManager.Instance.UnitSelected(UnitCategoryService.Instance.Data.Allies[0]);

            VCamAssemblyController.Instance.SetTarget();
            CameraChangeMonobService.Instance.ChangeCamera(Data.CameraType.Assembly);
            MenuChangeMonobService.Instance.ToRepairMenu();

            PopupFadingUI.Instance.FadeIn(TutorialType.Repair);
        }

        // Switches to preview mode for tactical overview
        public void EnterPreviewMode()
        {
            SelectModeManager.Instance.DefaultActionSelected(UnitCategoryService.Instance.Data.Allies[0]);
            InputManager.Instance.ToggleControls(true);
            CameraChangeMonobService.Instance.ChangeCamera(Data.CameraType.Overhead);
            MenuChangeMonobService.Instance.ToNone();

            Vector3 targetPosition = UnitSelectService.Instance.Data.SelectedUnit.Data.UnitBodyTransform.position;
            CameraSmoothingMonobService.Instance.StartCentering(targetPosition);

            PopupFadingUI.Instance.FadeIn(TutorialType.Preview);
        }

        // Handles briefing mode, including new campaign setup
        public void EnterBriefingMode(bool newRun)
        {
            UnitSelectService.Instance.DeselectUnit();
            ActionCoordinatorService.Instance.DeselectAction();
            UnitSelectService.Instance.DeselectGridSquare();

            if (newRun)
            {
                GridUnitService.Instance.RemoveAlliedUnits();
                UnitCategoryService.Instance.RemoveAlliedUnits();
                SaveDataManager.Instance.StartNewCampaign();
                SFXMonobService.Instance.PlaySFX(SFXType.StartPhase);
            }
            else
            {
                SaveDataManager.Instance.SaveGameData();
            }

            MenuChangeMonobService.Instance.ToIntroMenu();
            CameraChangeMonobService.Instance.ChangeCamera(Data.CameraType.Drift);
        }

        // Switches to assembly mode for unit customization
        public void EnterAssemblyMode()
        {
            SelectModeManager.Instance.UnitSelected(UnitCategoryService.Instance.Data.Allies[0]);
            VCamAssemblyController.Instance.SetTarget();
            CameraChangeMonobService.Instance.ChangeCamera(Data.CameraType.Assembly);
            MenuChangeMonobService.Instance.ToAssembleMenu();

            PopupFadingUI.Instance.FadeIn(TutorialType.Assemble);
        }

        // Handles transition to mission mode, including phase and camera setup
        public void EnterMissionMode()
        {
            TimeScaleManager.Instance.SetTimeScaleNormal();

            gameControlType = GameControlType.Mission;
            CameraChangeMonobService.Instance.ChangeCamera(Data.CameraType.Overhead);
            List<UnitSingleController> allies = (List<UnitSingleController>)UnitCategoryService.Instance.Data.Allies;
            UnitSingleController unit = UnitSelectService.Instance.Data.SelectedUnit;
            if (unit == null && allies.Count > 0)
            {
                Vector2Int gridPos = GridUtilityService.Instance.GetGridPosition(MouseWorldService.Instance.GetPosition());
                UnitSelectService.Instance.SelectGridSquare(gridPos);
                UnitSelectService.Instance.SelectUnit(allies[0]);
                ActionCoordinatorService.Instance.SelectMoveAction();
                CameraSmoothingMonobService.Instance.StartCentering(allies[0].transform.position);
            }

            if (PhaseManager.Instance.isPlayerPhase)
            {
                TimeScaleManager.Instance.SetTimeScaleNormal();
                InputManager.Instance.ToggleControls(true);
            }
            else
            {
                TimeScaleManager.Instance.SetTimeScaleFast();
                InputManager.Instance.ToggleControls(false);
            }
            MenuChangeMonobService.Instance.ToNone();

            OnGameModeChanged?.Invoke(this, EventArgs.Empty);
        }

        // Sets up menu mode and spawns units if needed
        public void EnterMenuMode()
        {
            PhaseManager.Instance.ResetPhase();
            //MissionSetupManager.Instance.GenerateSettings();
            TimeScaleManager.Instance.SetTimeScaleNormal();
            UnitSelectService.Instance.DeselectUnit();
            ActionCoordinatorService.Instance.DeselectAction();
            UnitSelectService.Instance.DeselectGridSquare();
            if (UnitCategoryService.Instance.Data.Enemies.Count <= 0) SpawnEnemyService.Instance.SpawnEnemies();
            CameraChangeMonobService.Instance.ChangeCamera(Data.CameraType.Drift);
            MenuChangeMonobService.Instance.ToMainMenu();
            UnitMovementService.Instance.TeleportToSpawnPoint(UnitCategoryService.Instance.Data.Allies);
            UnitMaterialService.Instance.SetAllMeshes(UnitCategoryService.Instance.Data.Allies);
        }

        // Handles victory state transition using coroutine for timing
        public IEnumerator EnterVictoryMode()
        {
            gameControlType = GameControlType.Outside;
            SFXMonobService.Instance.PlaySFX(SFXType.StartPhase);
            TimeScaleManager.Instance.SetTimeScaleNormal();
            CameraChangeMonobService.Instance.ChangeCamera(Data.CameraType.Victory);

            ScrapManager.Instance.SetScrapReward();
            LevelManager.Instance.SetLevel(LevelManager.Instance.level + 1);
            SaveDataManager.Instance.SaveGameData();

            yield return new WaitForSeconds(durationData.CameraBlendDuration);

            MenuChangeMonobService.Instance.ToVictoryMenu();

            OnGameModeChanged?.Invoke(this, EventArgs.Empty);
        }

        // Handles defeat state transition using coroutine for timing
        public IEnumerator EnterDefeatMode()
        {
            gameControlType = GameControlType.Outside;
            SFXMonobService.Instance.PlaySFX(SFXType.StartPhase);
            TimeScaleManager.Instance.SetTimeScaleNormal();
            CameraChangeMonobService.Instance.ChangeCamera(Data.CameraType.Overhead);

            yield return new WaitForSeconds(durationData.CameraBlendDuration);

            MenuChangeMonobService.Instance.ToDefeatMenu();

            OnGameModeChanged?.Invoke(this, EventArgs.Empty);
        }

        // Prepares the game for the next phase, optionally healing and refilling units
        public IEnumerator EnterPrepMode(bool isFullStep)
        {
            isPreparing = true;

            CameraChangeMonobService.Instance.ChangeCamera(Data.CameraType.Overhead);

            if (isFullStep)
            {
                MenuChangeMonobService.Instance.ToNone();
                CameraSmoothingMonobService.Instance.StartCentering(controller.defaultPosTransform.position);

                yield return new WaitForSeconds(0.8f);

                UnitHealthService.Instance.HealAllAlies();
                SpawnAllyService.Instance.Refill();

                yield return new WaitForSeconds(durationData.UnitSpawnDuration + .3f);
            }

            isPreparing = false;

            Vector2Int gridPos = GridUtilityService.Instance.GetGridPosition(MouseWorldService.Instance.GetPosition());
            UnitSelectService.Instance.SelectGridSquare(gridPos);
            UnitSelectService.Instance.SelectUnit(UnitCategoryService.Instance.Data.Allies[0]);
            ActionCoordinatorService.Instance.SelectMoveAction();
            MenuChangeMonobService.Instance.ToPreparationMenu();

            PopupFadingUI.Instance.FadeIn(TutorialType.Preparation);
        }
    }
}

