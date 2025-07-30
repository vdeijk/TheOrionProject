using UnityEngine;

namespace TurnBasedStrategy
{
    public class UnitSwitchUI : UnitSwitching
    {
        bool isPlaying => CameraChangeService.Instance.curCamera == CameraType.Overhead;

        private void Awake()
        {
            duration = gameDurations.materialFadeDuration;
        }

        public void SwitchUnitsUp()
        {
            if (canSwitch && isPlaying)
            {
                SwitchUp(UnitCategoryService.Instance.all);
                Vector3 targetPos = UnitSelectService.Instance.selectedUnit.unitBodyTransform.position;
                CameraSmoothingService.Instance.StartCentering(targetPos);
                GridSquareInfoController.Instance.SelectGridSquare(targetPos);
            }
        }

        public void SwitchUnitsDown()
        {
            if (canSwitch && isPlaying)
            {
                SwitchDown(UnitCategoryService.Instance.all);
                Vector3 targetPos = UnitSelectService.Instance.selectedUnit.unitBodyTransform.position;
                CameraSmoothingService.Instance.StartCentering(targetPos);
                GridSquareInfoController.Instance.SelectGridSquare(targetPos);
            }
        }
    }
}

/*
                if (UnitActionSystem.Instance.selectedAction == null) return;
                UnitActionSystem.Instance.UpdateGridPositions(UnitSelectService.Instance.selectedUnit);*/