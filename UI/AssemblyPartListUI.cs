using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Game;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class AssemblyPartListUI : MonoBehaviour
    {
        [SerializeField] Transform partButtonContainer;
        [SerializeField] Transform partButtonPrefab;

        private List<AssemblyPartButtonUI> partButtons = new List<AssemblyPartButtonUI>();
        private PartType? currentPartType = null;

        public Dictionary<PartType, List<PartSO>> playerParts => PartsManager.Instance.playerParts;

        private void OnEnable()
        {
            // Subscribe to part and selection events for dynamic UI updates
            AssemblyService.OnPartSelected += UnitAssemblyService_OnPartSelected;
            AssemblyService.OnPartDeselected += UnitAssemblyService_OnPartDeselected;
            PartsManager.OnPartAdded += PartsManager_OnPartAdded;
            PartsManager.OnPartRemoved += PartsManager_OnPartRemoved;
        }

        private void OnDisable()
        {
            AssemblyService.OnPartSelected -= UnitAssemblyService_OnPartSelected;
            AssemblyService.OnPartDeselected -= UnitAssemblyService_OnPartDeselected; 
            PartsManager.OnPartAdded -= PartsManager_OnPartAdded;
            PartsManager.OnPartRemoved -= PartsManager_OnPartRemoved;
        }

        // Rebuilds the part list when a part is added
        private void PartsManager_OnPartAdded(object sender, EventArgs e)
        {
            StartCoroutine(RebuildListNextFrame());
        }

        // Rebuilds the part list when a part is removed
        private void PartsManager_OnPartRemoved(object sender, EventArgs e)
        {
            StartCoroutine(RebuildListNextFrame());
        }

        // Updates part list or selection visuals when a part is deselected
        private void UnitAssemblyService_OnPartDeselected(object sender, EventArgs e)
        {
            PartType newPartType = AssemblyService.Instance.Data.CurPartType;
            if (newPartType != currentPartType)
            {
                currentPartType = newPartType;
                StartCoroutine(RebuildListNextFrame());
            }
            else
            {
                StartCoroutine(UpdateSelectionsNextFrame());
            }
        }

        // Updates part list or selection visuals when a part is selected
        private void UnitAssemblyService_OnPartSelected(object sender, EventArgs e)
        {
            PartType newPartType = AssemblyService.Instance.Data.CurPartType;

            if (newPartType != currentPartType)
            {
                currentPartType = newPartType;
                StartCoroutine(RebuildListNextFrame());
            }
            else
            {
                StartCoroutine(UpdateSelectionsNextFrame());
            }
        }

        // Waits one frame before rebuilding the part list (ensures UI is up-to-date)
        private IEnumerator RebuildListNextFrame()
        {
            yield return null;

            DestroyPartList();
            CreatePartList();
        }

        // Waits one frame before updating selection visuals
        private IEnumerator UpdateSelectionsNextFrame()
        {
            yield return null;

            foreach (AssemblyPartButtonUI partButtonUI in partButtons)
            {
                if (partButtonUI != null)
                {
                    partButtonUI.UpdateSelectedVisual();
                }
            }
        }

        // Destroys all part buttons in the UI
        private void DestroyPartList()
        {
            foreach (AssemblyPartButtonUI partButton in partButtons)
            {
                if (partButton != null)
                {
                    Destroy(partButton.gameObject);
                }
            }

            partButtons.Clear();
        }

        // Creates part buttons for the current part type, grouping by part and count
        private void CreatePartList()
        {
            PartType curPartType = AssemblyService.Instance.Data.CurPartType;
            List<PartSO> parts = playerParts[curPartType];

            var groupedParts = parts
                .GroupBy(p => p)
                .Select(g => new { Part = g.Key, Count = g.Count() });

            foreach (var group in groupedParts)
            {
                Transform newPart = Instantiate(partButtonPrefab, partButtonContainer);
                AssemblyPartButtonUI partButton = newPart.GetComponent<AssemblyPartButtonUI>();

                partButtons.Add(partButton);

                partButton.Initialize(group.Part);
                partButton.partNameText.text = group.Part.Name;
                partButton.partCostText.text = group.Part.Cost.ToString();
                partButton.partNumberText.text = $"x{group.Count}";
            }

            // Force layout rebuild for immediate UI update
            if (partButtonContainer is RectTransform rectTransform)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            }

            StartCoroutine(UpdateSelectionsNextFrame());
        }
    }
}
