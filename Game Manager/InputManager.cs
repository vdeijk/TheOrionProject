using UnityEngine;
using UnityEngine.EventSystems;

namespace TurnBasedStrategy
{
    public class InputManager : Singleton<InputManager>
    {
        public Vector3 cameraMoveInputs { get; private set; }
        public float cameraZoomInput { get; private set; }
        public bool leftMouseButtonInput { get; private set; }
        public bool rightMouseButtonInput { get; private set; }
        public bool cameraCenteringInput { get; private set; }
        public bool pauseInput { get; private set; }
        public bool areControlsEnabled { get; private set; } = false;

        // Central update loop for input handling and game control
        private void Update()
        {
            ObtainUserInputs();
            HandlePauseInput();

            if (areControlsEnabled)
            {
                HandleCameraInput();
                HandleMouseInput();
            }
        }

        public void ToggleControls(bool areControlsEnabled)
        {
            this.areControlsEnabled = areControlsEnabled;
        }

        // Handles pause input and menu transitions
        private void HandlePauseInput()
        {
            if (!pauseInput) return;

            switch (MenuChangeService.Instance.curMenu)
            {
                case MenuType.Assemble:
                    StartCoroutine(ControlModeManager.Instance.EnterPrepMode(false));
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
                        MenuChangeService.Instance.ToPauseMenu();
                        SFXController.Instance.PlaySFX(SFXType.ExecuteAction);
                    }
                    else
                    {
                        MenuChangeService.Instance.ToPrevMenu();
                    }
                    break;
            }
        }

        // Handles camera movement and zoom based on current camera mode
        private void HandleCameraInput()
        {
            switch (CameraChangeService.Instance.curCamera)
            {
                case CameraType.Overhead:
                    bool noMenu = MenuChangeService.Instance.curMenu == MenuType.None;
                    if (noMenu)
                    {
                        CameraControlsService.Instance.UpdateZoom(cameraZoomInput);
                        CameraControlsService.Instance.UpdateCentering(cameraCenteringInput);
                        CameraControlsService.Instance.UpdatePosition(cameraMoveInputs);
                    }
                    break;
                case CameraType.Map:
                    CameraControlsService.Instance.UpdateZoom(cameraZoomInput);
                    break;
            }
        }

        // Handles mouse input only in overhead camera mode
        private void HandleMouseInput()
        {
            if (CameraChangeService.Instance.curCamera == CameraType.Overhead)
            {
                HandleLeftMouseInput();
                HandleRightMouseInput();
            }
        }

        // Collects all relevant user inputs for this frame
        private void ObtainUserInputs()
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
                bool isOverWorldSpaceUI = MenuPointerService.Instance.IsPointerOverWorldSpaceUI();

                if (isOverUI) return; // Ignore clicks over UI
                if (!PhaseManager.Instance.isPlayerPhase) return; // Only allow in player phase
                if (UnitActionSystem.Instance.TryHandleSelectedAction()) return;
                if (UnitSelectService.Instance.TryHandleUnitSelection()) return;
                if (GridSquareInfoController.Instance.TryHandleGridSquareSelection()) return;
            }
        }

        // Handles right mouse button input for unit and grid selection
        private void HandleRightMouseInput()
        {
            UnitSelectService.Instance.HandleInputMouseRight(rightMouseButtonInput);
            GridSquareInfoController.Instance.HandleInputMouseRight(rightMouseButtonInput);
        }
    }
}