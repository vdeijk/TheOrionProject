using System;
using TMPro;
using UnityEngine;

namespace TurnBasedStrategy
{
    public abstract class UIAssemblyPart : MonoBehaviour
    {
        protected string symbol = " <b><color=#00F1D5>>></color></b> ";

        private void OnEnable()
        {
            UnitAssemblyService.OnPartSelected += UnitAssemblyService_OnPartSelected;
        }

        private void OnDisable()
        {
            UnitAssemblyService.OnPartSelected -= UnitAssemblyService_OnPartSelected;
        }

        protected abstract void SetTexts(PartData partData);

        private void UnitAssemblyService_OnPartSelected(object sender, EventArgs e)
        {
            SetTexts(UnitAssemblyService.Instance.selectedPart);
        }

    }
}