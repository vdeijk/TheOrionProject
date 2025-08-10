using TMPro;
using UnityEngine;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class AssemblyPartButtonUI : BaseButtonUI
    {
        private PartSO partData;

        [field: SerializeField] public TextMeshProUGUI partNameText { get; private set; }
        [field: SerializeField] public TextMeshProUGUI partCostText { get; private set; }
        [field: SerializeField] public TextMeshProUGUI partNumberText { get; private set; }

        // Returns the part data associated with this button
        public PartSO GetPartData() => partData;

        // Initializes the button with part data
        public void Initialize(PartSO partData)
        {
            this.partData = partData;
        }

        // Updates button visuals based on current selection
        public void UpdateSelectedVisual()
        {
            PartSO currentSelectedPart = AssemblyService.Instance.Data.SelectedPart;
            isSelected = currentSelectedPart != null && currentSelectedPart == partData;
            button.interactable = true;
            SetButtonState(GetButtonState(), true);
        }

        // Handles button click to select the part in the assembly system
        protected override void OnButtonClicked()
        {
            if (partData != null)
            {
                AssemblyService.Instance.SelectPart(partData);
            }
        }
    }
}