using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using TurnBasedStrategy.Domain;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class AssemblyPartTypeUI : SingletonBaseService<AssemblyPartTypeUI>
    {
        [SerializeField] TextMeshProUGUI partTypeText;

        private void OnEnable()
        {
            MenuChangeMonobService.OnMenuChanged += MenuManager_OnMenuChanged;
        }

        private void OnDisable()
        {
            MenuChangeMonobService.OnMenuChanged -= MenuManager_OnMenuChanged;
        }

        // Sets up part type UI when entering assembly menu
        private void MenuManager_OnMenuChanged(object sender, EventArgs e)
        {
            if (MenuChangeMonobService.Instance.curMenu == MenuType.Assemble)
            {
                AssemblyService.Instance.Setup();
                partTypeText.text = GetPartTypeLabel(AssemblyService.Instance.Data.CurPartType);
            }
        }

        // Switches to the next part type and updates label
        public void NextPartType()
        {
            AssemblyService.Instance.NextPartType();
            partTypeText.text = GetPartTypeLabel(AssemblyService.Instance.Data.CurPartType);
        }

        // Switches to the previous part type and updates label
        public void PreviousPartType()
        {
            AssemblyService.Instance.PreviousPartType();
            partTypeText.text = GetPartTypeLabel(AssemblyService.Instance.Data.CurPartType);
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