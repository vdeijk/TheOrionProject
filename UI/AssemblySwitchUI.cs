using System.Collections.Generic;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Game;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class AssemblySwitchUI : UnitSwitchController
    {
        private void Awake()
        {
            duration = durationData.CameraBlendDuration; // Set switch duration from game settings
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
                SwitchUp(UnitCategoryService.Instance.Data.Allies);
                SetPartPreview();
                VCamAssemblyController.Instance.SetTarget();
            }
        }

        // Switches to the previous unit in the allies list and updates preview/camera
        public void SwitchUnitsDown()
        {
            if (canSwitch)
            {
                SwitchDown(UnitCategoryService.Instance.Data.Allies);
                SetPartPreview();
                VCamAssemblyController.Instance.SetTarget();
            }
        }

        // Sets the part preview to the first available part of the current type
        private void SetPartPreview()
        {
            var availableParts = PartsManager.Instance.allParts;
            var curPartType = AssemblyService.Instance.Data.CurPartType;
            AssemblyService.Instance.SelectPart(availableParts[curPartType][0]);
        }
    }
}
