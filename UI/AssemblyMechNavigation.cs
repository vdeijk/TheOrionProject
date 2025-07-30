using System.Collections.Generic;

namespace TurnBasedStrategy
{
    public class AssemblyMechNavigation : UnitSwitching
    {
        private void Awake()
        {
            duration = gameDurations.cameraBlendDuration; // Set switch duration from game settings
        }

        // Exits assembly mode and enters prep mode
        public void ToggleOff()
        {
            StartCoroutine(ControlModeManager.Instance.EnterPrepMode(false));
        }

        // Switches to the next unit in the allies list and updates preview/camera
        public void SwitchUnitsUp()
        {
            if (canSwitch)
            {
                SwitchUp(UnitCategoryService.Instance.allies);
                SetPartPreview();
                CameraAssembly.Instance.SetTarget();
            }
        }

        // Switches to the previous unit in the allies list and updates preview/camera
        public void SwitchUnitsDown()
        {
            if (canSwitch)
            {
                SwitchDown(UnitCategoryService.Instance.allies);
                SetPartPreview();
                CameraAssembly.Instance.SetTarget();
            }
        }

        // Sets the part preview to the first available part of the current type
        private void SetPartPreview()
        {
            var availableParts = PartsManager.Instance.allParts;
            var curPartType = UnitAssemblyService.Instance.curPartType;
            UnitAssemblyService.Instance.SelectPart(availableParts[curPartType][0]);
        }
    }
}
