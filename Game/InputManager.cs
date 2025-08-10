using UnityEngine;
using UnityEngine.EventSystems;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.Game
{
    [DefaultExecutionOrder(200)]
    public class InputManager
    {
        private static InputManager _instance;
        private GameController gameController;

        public bool areControlsEnabled { get; private set; } = false;

        public Vector3 cameraMoveInputs { get; private set; }
        public float cameraZoomInput { get; private set; }
        public bool leftMouseButtonInput { get; private set; }
        public bool rightMouseButtonInput { get; private set; }
        public bool cameraCenteringInput { get; private set; }
        public bool pauseInput { get; private set; }

        public static InputManager Instance => _instance ??= new InputManager();

        public void ToggleControls(bool areControlsEnabled)
        {
            this.areControlsEnabled = areControlsEnabled;
        }

        // Handles pause input and menu transitions
        public void HandlePauseInput()
        {
            if (!pauseInput) return;

            switch (MenuChangeMonobService.Instance.curMenu)
            {
                case MenuType.Assemble:
                    gameController.StartCoroutine(ControlModeManager.Instance.EnterPrepMode(false));
                    break;

                case MenuType.Preparation:
                    ControlModeManager.Instance.EnterPreviewMode();
                    break;

                case MenuType.Pause:
                    ControlModeManager.Instance.EnterMissionMode();
                    break;

                case MenuType.None:
                    ToggleControls(false);
                    bool inMission = ControlModeManager.Instance.gameControlType == GameControlType.Mission;
                    if (inMission)
                    {
                        MenuChangeMonobService.Instance.ToPauseMenu();
                        SFXMonobService.Instance.PlaySFX(SFXType.ExecuteAction);
                    }
                    else
                    {
                        MenuChangeMonobService.Instance.ToPrevMenu();
                    }
                    break;
            }
        }

        // Handles camera movement and zoom based on current camera mode
        public void HandleCameraInput()
        {
            switch (CameraChangeMonobService.Instance.curCamera)
            {
                case Data.CameraType.Overhead:
                    bool noMenu = MenuChangeMonobService.Instance.curMenu == MenuType.None;
                    if (noMenu)
                    {
                        CameraControlsMonobService.Instance.UpdateZoom(cameraZoomInput);
                        CameraControlsMonobService.Instance.UpdateCentering(cameraCenteringInput);
                        CameraControlsMonobService.Instance.UpdatePosition(cameraMoveInputs);
                    }
                    break;
                case Data.CameraType.Map:
                    CameraControlsMonobService.Instance.UpdateZoom(cameraZoomInput);
                    break;
            }
        }

        // Handles mouse input only in overhead camera mode
        public void HandleMouseInput()
        {
            if (CameraChangeMonobService.Instance.curCamera == Data.CameraType.Overhead)
            {
                HandleLeftMouseInput();
                HandleRightMouseInput();
            }
        }

        // Collects all relevant user inputs for this frame
        public void ObtainUserInputs()
        {
            cameraMoveInputs = ObtainCameraInputsMove();
            cameraZoomInput = Input.mouseScrollDelta.y;
            pauseInput = Input.GetKeyDown(KeyCode.Escape);
            leftMouseButtonInput = Input.GetMouseButtonDown(0);
            rightMouseButtonInput = Input.GetMouseButtonDown(1);
            cameraCenteringInput = Input.GetKeyDown(KeyCode.C);
        }

        // Returns camera movement direction based on WASD keys
        private Vector3 ObtainCameraInputsMove()
        {
            Vector3 inputMoveDir = new Vector3(0, 0, 0);

            if (Input.GetKey(KeyCode.W))
            {
                inputMoveDir.z = 1;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                inputMoveDir.z = -1;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                inputMoveDir.x = -1;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                inputMoveDir.x = 1;
            }

            return inputMoveDir;
        }

        // Handles left mouse button input, prioritizing UI and game phase checks
        private void HandleLeftMouseInput()
        {
            if (leftMouseButtonInput)
            {
                bool isOverUI = EventSystem.current.IsPointerOverGameObject();
                bool isOverWorldSpaceUI = MenuPointerMonobService.Instance.IsPointerOverWorldSpaceUI();

                if (isOverUI) return; // Ignore clicks over UI
                if (!PhaseManager.Instance.isPlayerPhase) return; // Only allow in player phase
                if (ActionMoveService.Instance.TryStartAction()) return;
                if (ActionShootService.Instance.TryStartAction()) return;
                if (ActionPassService.Instance.TryStartAction()) return;

                UnitSelectService.Instance.TryHandleUnitSelection();
            }
        }

        // Handles right mouse button input for unit and grid selection
        private void HandleRightMouseInput()
        {
            UnitSelectService.Instance.HandleInputMouseRight(rightMouseButtonInput);
        }
    }
}