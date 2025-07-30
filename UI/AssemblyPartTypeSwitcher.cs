using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class AssemblyPartTypeSwitcher : Singleton<AssemblyPartTypeSwitcher>
    {
        [SerializeField] TextMeshProUGUI partTypeText;

        private void OnEnable()
        {
            MenuChangeService.OnMenuChanged += MenuManager_OnMenuChanged;
        }

        private void OnDisable()
        {
            MenuChangeService.OnMenuChanged -= MenuManager_OnMenuChanged;
        }

        // Sets up part type UI when entering assembly menu
        private void MenuManager_OnMenuChanged(object sender, EventArgs e)
        {
            if (MenuChangeService.Instance.curMenu == MenuType.Assemble)
            {
                UnitAssemblyService.Instance.Setup();
                partTypeText.text = GetPartTypeLabel(UnitAssemblyService.Instance.curPartType);
            }
        }

        // Switches to the next part type and updates label
        public void NextPartType()
        {
            UnitAssemblyService.Instance.NextPartType();
            partTypeText.text = GetPartTypeLabel(UnitAssemblyService.Instance.curPartType);
        }

        // Switches to the previous part type and updates label
        public void PreviousPartType()
        {
            UnitAssemblyService.Instance.PreviousPartType();
            partTypeText.text = GetPartTypeLabel(UnitAssemblyService.Instance.curPartType);
        }

        // Returns a user-friendly label for the given part type
        private string GetPartTypeLabel(PartType partType)
        {
            switch (partType)
            {
                case PartType.Base:
                    return "Base";
                case PartType.Torso:
                    return "Torso";
                case PartType.WeaponPrimary:
                    return "Primary Weapon";
                case PartType.WeaponSecondary:
                    return "Secondary Weapon";
                default:
                    return partType.ToString();
            }
        }
    }
}