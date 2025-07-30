using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class ControlModeManager : Singleton<ControlModeManager>
    {
        [SerializeField] GameDurations gameDurations;
        [SerializeField] TutorialData tutorialData;
        [SerializeField] Transform defaultPosTransform;
        [SerializeField] MissionData missionData;

        public GameControlType gameControlType { get; private set; } = GameControlType.Outside;
        public bool isPreparing { get; private set; }

        public static event EventHandler OnGameModeChanged;

        private void Start()
        {
            EnterMenuMode(); // Start in menu mode
        }

        // Handles transition to salvage mode, including camera and UI changes
        public void EnterSalvageMode(Unit unit)
        {
            CameraChangeService.Instance.ChangeCamera(CameraType.Salvage);
            UnitSalvageService.Instance.Setup();
            SalvageCamera.Instance.SetSalvagableUnit(unit);
            MenuChangeService.Instance.ToSalvageMenu();
            TimeScaleManager.Instance.SetTimeScaleToZero();

            SFXController.Instance.PlaySFX(SFXType.StartPhase);

            PopupFading.Instance.FadeIn(TutorialType.Salvage);
        }

        // Switches to action mode for combat
        public void EnterActionMode()
        {
            TimeScaleManager.Instance.SetTimeScaleNormal();
            CameraChangeService.Instance.ChangeCamera(CameraType.Action);
        }

        // Switches to repair mode for unit maintenance
        public void EnterRepairMode()
        {
            SelectUnit();
            CameraAssembly.Instance.SetTarget();
            CameraChangeService.Instance.ChangeCamera(CameraType.Assembly);
            MenuChangeService.Instance.ToRepairMenu();

            PopupFading.Instance.FadeIn(TutorialType.Repair);
        }

        // Switches to preview mode for tactical overview
        public void EnterPreviewMode()
        {
            SelectUnit();
            InputManager.Instance.ToggleControls(true);
            CameraChangeService.Instance.ChangeCamera(CameraType.Overhead);
            MenuChangeService.Instance.ToNone();

            Vector3 targetPosition = UnitSelectService.Instance.selectedUnit.unitBodyTransform.position;
            CameraSmoothingService.Instance.StartCentering(targetPosition);

            PopupFading.Instance.FadeIn(TutorialType.Preview);
        }

        // Handles briefing mode, including new campaign setup
        public void EnterBriefingMode(bool newRun)
        {
            UnitSelectService.Instance.DeselectUnit();
            GridSquareInfoController.Instance.DeselectGridSquare();

            if (newRun)
            {
                LevelGrid.Instance.RemoveAlliedUnits();
                SaveDataManager.Instance.StartNewCampaign();
                SFXController.Instance.PlaySFX(SFXType.StartPhase);
            }
            else
            {
                SaveDataManager.Instance.SaveGameData();
            }

            MenuChangeService.Instance.ToIntroMenu();
            CameraChangeService.Instance.ChangeCamera(CameraType.Drift);
        }

        // Switches to assembly mode for unit customization
        public void EnterAssemblyMode()
        {
            SelectUnit();
            CameraAssembly.Instance.SetTarget();
            CameraChangeService.Instance.ChangeCamera(CameraType.Assembly);
            MenuChangeService.Instance.ToAssembleMenu();

            PopupFading.Instance.FadeIn(TutorialType.Assemble);
        }

        // Handles transition to mission mode, including phase and camera setup
        public void EnterMissionMode()
        {
            TimeScaleManager.Instance.SetTimeScaleNormal();

            gameControlType = GameControlType.Mission;
            CameraChangeService.Instance.ChangeCamera(CameraType.Overhead);
            List<Unit> allies = UnitCategoryService.Instance.allies;
            Unit unit = UnitSelectService.Instance.selectedUnit;
            if (unit == null && allies.Count > 0)
            {
                UnitSelectService.Instance.SelectUnit(allies[0]);
                CameraSmoothingService.Instance.StartCentering(allies[0].transform.position);
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
            MenuChangeService.Instance.ToNone();

            OnGameModeChanged?.Invoke(this, EventArgs.Empty);
        }

        // Sets up menu mode and spawns units if needed
        public void EnterMenuMode()
        {
            PhaseManager.Instance.ResetPhase();
            MissionSetupManager.Instance.GenerateSettings();
            TimeScaleManager.Instance.SetTimeScaleNormal();
            UnitSelectService.Instance.DeselectUnit();
            GridSquareInfoController.Instance.DeselectGridSquare();
            if (UnitCategoryService.Instance.enemies.Count <= 0) UnitSpawnService.Instance.SpawnEnemies();
            CameraChangeService.Instance.ChangeCamera(CameraType.Drift);
            MenuChangeService.Instance.ToMainMenu();
            UnitSpawnService.Instance.TeleportToSpawnPoint(UnitCategoryService.Instance.allies);
            UnitMaterialService.Instance.SetAllMeshes(UnitCategoryService.Instance.allies);
        }

        // Handles victory state transition using coroutine for timing
        public IEnumerator EnterVictoryMode()
        {
            gameControlType = GameControlType.Outside;
            SFXController.Instance.PlaySFX(SFXType.StartPhase);
            TimeScaleManager.Instance.SetTimeScaleNormal();
            CameraChangeService.Instance.ChangeCamera(CameraType.Victory);

            ScrapManager.Instance.SetScrapReward();
            LevelManager.Instance.SetLevel(LevelManager.Instance.level + 1);
            SaveDataManager.Instance.SaveGameData();

            yield return new WaitForSeconds(gameDurations.cameraBlendDuration);

            MenuChangeService.Instance.ToVictoryMenu();

            OnGameModeChanged?.Invoke(this, EventArgs.Empty);
        }

        // Handles defeat state transition using coroutine for timing
        public IEnumerator EnterDefeatMode()
        {
            gameControlType = GameControlType.Outside;
            SFXController.Instance.PlaySFX(SFXType.StartPhase);
            TimeScaleManager.Instance.SetTimeScaleNormal();
            CameraChangeService.Instance.ChangeCamera(CameraType.Overhead);

            yield return new WaitForSeconds(gameDurations.cameraBlendDuration);

            MenuChangeService.Instance.ToDefeatMenu();

            OnGameModeChanged?.Invoke(this, EventArgs.Empty);
        }

        // Prepares the game for the next phase, optionally healing and refilling units
        public IEnumerator EnterPrepMode(bool isFullStep)
        {
            isPreparing = true;

            CameraChangeService.Instance.ChangeCamera(CameraType.Overhead);

            if (isFullStep)
            {
                MenuChangeService.Instance.ToNone();
                CameraSmoothingService.Instance.StartCentering(defaultPosTransform.position);

                yield return new WaitForSeconds(0.8f);

                UnitHealthService.Instance.HealAllAlies();
                UnitSpawnService.Instance.RefillAllies();

                yield return new WaitForSeconds(gameDurations.unitSpawnDuration + .3f);
            }

            isPreparing = false;

            UnitSelectService.Instance.SelectUnit(UnitCategoryService.Instance.allies[0]);
            Vector3 pos = UnitCategoryService.Instance.allies[0].transform.position;
            GridSquareInfoController.Instance.SelectGridSquare(pos);
            MenuChangeService.Instance.ToPreparationMenu();

            PopupFading.Instance.FadeIn(TutorialType.Preparation);
        }

        // Ensures a valid player unit is selected
        private void SelectUnit()
        {
            Unit unit = UnitSelectService.Instance.selectedUnit;
            if (unit == null || unit.unitEntityTransform.GetComponent<UnitFaction>().IS_ENEMY)
            {
                UnitSelectService.Instance.SelectUnit(UnitCategoryService.Instance.allies[0]);
            }
        }
    }
}

