using System;
using TMPro;
using UnityEngine;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public abstract class AssemblyPartUI : MonoBehaviour
    {
        protected string symbol = " <b><color=#00F1D5>>></color></b> ";

        private void OnEnable()
        {
            AssemblyService.OnPartSelected += UnitAssemblyService_OnPartSelected;
        }

        private void OnDisable()
        {
            AssemblyService.OnPartSelected -= UnitAssemblyService_OnPartSelected;
        }

        protected abstract void SetTexts(PartSO partData);

        private void UnitAssemblyService_OnPartSelected(object sender, EventArgs e)
        {
            SetTexts(AssemblyService.Instance.Data.SelectedPart);
        }

    }
}