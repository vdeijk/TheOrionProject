using System;
using TMPro;
using UnityEngine;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public abstract class SalvageDisplayUI : MonoBehaviour
    {
        private void OnEnable()
        {
            SalvageService.OnCurSalvageTypeChanged += UnitSalvageService_OnCurSalvageTypeChanged;
        }

        private void OnDisable()
        {
            SalvageService.OnCurSalvageTypeChanged -= UnitSalvageService_OnCurSalvageTypeChanged;
        }

        private void UnitSalvageService_OnCurSalvageTypeChanged(object sender, EventArgs e)
        {
            Setup();
        }

        protected abstract void Setup();
    }
}