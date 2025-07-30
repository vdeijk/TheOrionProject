using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TurnBasedStrategy
{
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
            UnitAssemblyService.OnPartSelected += UnitAssemblyService_OnPartSelected;
            UnitAssemblyService.OnPartDeselected += UnitAssemblyService_OnPartDeselected;
        }

        private void OnDisable()
        {
            UnitAssemblyService.OnPartSelected -= UnitAssemblyService_OnPartSelected;
            UnitAssemblyService.OnPartDeselected -= UnitAssemblyService_OnPartDeselected;
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
            var unit = UnitSelectService.Instance.selectedUnit;
            var isOnUnit = false;
            var selectedPart = UnitAssemblyService.Instance.selectedPart;

            foreach (var kvp in unit.partsData)
            {
                if (kvp.Value == selectedPart)
                {
                    isOnUnit = true; // Prevent duplicate assembly
                }
            }

            if (unit == null || selectedPart == null || isOnUnit)
            {
                SFXController.Instance.PlaySFX(SFXType.Failure);

                return;
            }

            ScrapManager.Instance.RemoveScrap(UnitAssemblyService.Instance.selectedPart.Cost);
            PartPreviewService.Instance.RemovePreview();
            HighlightService.Instance.RemoveHighlight();
            UnitAssemblyService.Instance.Assemble();

            OnPartAssembled?.Invoke(this, EventArgs.Empty);
        }

        // Handles recycling the selected part
        public void Recycle()
        {
            UnitAssemblyService.Instance.Recycle();
        }

        // Updates the part info card UI based on current selection
        private void UpdateCard()
        {
            if (UnitAssemblyService.Instance.selectedPart == null)
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

            switch (UnitAssemblyService.Instance.curPartType)
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

            backgroundText.text = "<b>Background:</b> " + UnitAssemblyService.Instance.selectedPart.Description;
        }
    }
}