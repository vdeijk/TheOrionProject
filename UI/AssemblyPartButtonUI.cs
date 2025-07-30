using TMPro;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class AssemblyPartButtonUI : BaseButtonUI
    {
        private PartData partData;

        [field: SerializeField] public TextMeshProUGUI partNameText { get; private set; }
        [field: SerializeField] public TextMeshProUGUI partCostText { get; private set; }
        [field: SerializeField] public TextMeshProUGUI partNumberText { get; private set; }

        // Returns the part data associated with this button
        public PartData GetPartData() => partData;

        // Initializes the button with part data
        public void Initialize(PartData partData)
        {
            this.partData = partData;
        }

        // Updates button visuals based on current selection
        public void UpdateSelectedVisual()
        {
            PartData currentSelectedPart = UnitAssemblyService.Instance.selectedPart;
            isSelected = currentSelectedPart != null && currentSelectedPart == partData;
            button.interactable = true;
            SetButtonState(GetButtonState(), true);
        }

        // Handles button click to select the part in the assembly system
        protected override void OnButtonClicked()
        {
            if (partData != null)
            {
                UnitAssemblyService.Instance.SelectPart(partData);
            }
        }
    }
}