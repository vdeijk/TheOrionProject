using System;
using TMPro;
using TurnBasedStrategy;
using UnityEngine;

public abstract class UISalvageDisplay : MonoBehaviour
{
    private void OnEnable()
    {
        UnitSalvageService.OnCurSalvageTypeChanged += UnitSalvageService_OnCurSalvageTypeChanged;
    }

    private void OnDisable()
    {
        UnitSalvageService.OnCurSalvageTypeChanged -= UnitSalvageService_OnCurSalvageTypeChanged;
    }

    private void UnitSalvageService_OnCurSalvageTypeChanged(object sender, EventArgs e)
    {
        Setup();
    }

    protected abstract void Setup();
}
