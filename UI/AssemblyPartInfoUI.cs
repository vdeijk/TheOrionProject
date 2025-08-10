using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using TurnBasedStrategy.Game;
using TurnBasedStrategy.Domain;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class AssemblyPartInfo : MonoBehaviour
    {
        [SerializeField] Transform uiAssemblyBase;
        [SerializeField] Transform uiAssemblyWeapon;
        [SerializeField] Transform uiAssemblyTorso;
        [SerializeField] TextMeshProUGUI backgroundText;
        [SerializeField] List<Transform> allCards;
        [SerializeField] Transform cardParent;

        public static event EventHandler OnPartAssembled;

        private void OnEnable()
        {
            AssemblyService.OnPartSelected += UnitAssemblyService_OnPartSelected;
            AssemblyService.OnPartDeselected += UnitAssemblyService_OnPartDeselected;
        }

        private void OnDisable()
        {
            AssemblyService.OnPartSelected -= UnitAssemblyService_OnPartSelected;
            AssemblyService.OnPartDeselected -= UnitAssemblyService_OnPartDeselected;
        }

        // Updates card UI when a part is selected
        private void UnitAssemblyService_OnPartSelected(object sender, EventArgs e)
        {
            UpdateCard();
        }

        // Updates card UI when a part is deselected
        private void UnitAssemblyService_OnPartDeselected(object sender, EventArgs e)
        {
            UpdateCard();
        }

        // Handles assembling the selected part onto the unit
        public void Assemble()
        {
            var unit = UnitSelectService.Instance.Data.SelectedUnit;
            var isOnUnit = false;
            var selectedPart = AssemblyService.Instance.Data.SelectedPart;

            foreach (var kvp in unit.Data.PartsData)
            {
                if (kvp.Value == selectedPart)
                {
                    isOnUnit = true; // Prevent duplicate assembly
                }
            }

            if (unit == null || selectedPart == null || isOnUnit)
            {
                SFXMonobService.Instance.PlaySFX(SFXType.Failure);

                return;
            }

            ScrapManager.Instance.RemoveScrap(AssemblyService.Instance.Data.SelectedPart.Cost);
            AssemblyService.Instance.DestroyPartPreview();
            PartHighlightService.Instance.Destroy();
            AssemblyService.Instance.Assemble();

            OnPartAssembled?.Invoke(this, EventArgs.Empty);
        }

        // Handles recycling the selected part
        public void Recycle()
        {
            AssemblyService.Instance.Recycle();
        }

        // Updates the part info card UI based on current selection
        private void UpdateCard()
        {
            if (AssemblyService.Instance.Data.SelectedPart == null)
            {
                cardParent.gameObject.SetActive(false);
                return;
            }
            else
            {
                cardParent.gameObject.SetActive(true);
            }

            foreach (var card in allCards)
            {
                card.gameObject.SetActive(false);
            }

            switch (AssemblyService.Instance.Data.CurPartType)
            {
                case PartType.Base:
                    uiAssemblyBase.gameObject.SetActive(true);
                    break;
                case PartType.Torso:
                    uiAssemblyTorso.gameObject.SetActive(true);
                    break;
                case PartType.WeaponPrimary:
                case PartType.WeaponSecondary:
                    uiAssemblyWeapon.gameObject.SetActive(true);
                    break;
            }

            backgroundText.text = "<b>Background:</b> " + AssemblyService.Instance.Data.SelectedPart.Description;
        }
    }
}