using System.Collections.Generic;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Domain;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class UnitSwitchUI : UnitSwitchController
    {
        bool isPlaying => CameraChangeMonobService.Instance.curCamera == Data.CameraType.Overhead;

        private void Awake()
        {
            duration = durationData.MaterialFadeDuration;
        }

        public void SwitchUnitsUp()
        {
            if (canSwitch && isPlaying)
            {
                SwitchUp(UnitCategoryService.Instance.Data.All);
                Vector3 targetPos = UnitSelectService.Instance.Data.SelectedUnit.Data.UnitBodyTransform.position;
                CameraSmoothingMonobService.Instance.StartCentering(targetPos);
            }
        }

        public void SwitchUnitsDown()
        {
            if (canSwitch && isPlaying)
            {
                SwitchDown(UnitCategoryService.Instance.Data.All);
                Vector3 targetPos = UnitSelectService.Instance.Data.SelectedUnit.Data.UnitBodyTransform.position;
                CameraSmoothingMonobService.Instance.StartCentering(targetPos);
            }
        }
    }
}
