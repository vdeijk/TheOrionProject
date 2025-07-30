using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class UISalvage : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI attemptsLeftText;
        [SerializeField] TextMeshProUGUI scrapGainText;
        [SerializeField] TextMeshProUGUI salvageTypeText;
        [SerializeField] Transform uiWeaponsTransform;
        [SerializeField] Transform uiTorsosTransform;
        [SerializeField] Transform uiBaseTransform;
        [SerializeField] Transform rightSectionTransform;
        [SerializeField] GameDurations gameDurations;
        [SerializeField] TextMeshProUGUI inInventoryText;
        [SerializeField] TextMeshProUGUI salvageChanceText;

        private bool canClick = true;
        private string COLOR_POSITIVE = "#00F1D5";
        private string COLOR_WARNING = "#e10000";
        private Dictionary<PartType, Transform> partTypeToUI;

        private PartType curSalvageType => UnitSalvageService.Instance.curSalvageType;

        private void Awake()
        {
            partTypeToUI = new Dictionary<PartType, Transform>
            {
                { PartType.Base, uiBaseTransform },
                { PartType.Torso, uiTorsosTransform },
                { PartType.WeaponPrimary, uiWeaponsTransform },
                { PartType.WeaponSecondary, uiWeaponsTransform }
            };
        }

        private void OnEnable()
        {
            UnitSalvageService.OnCurSalvageTypeChanged += UnitSalvageService_OnCurSalvageTypeChanged;
        }

        private void OnDisable()
        {
            UnitSalvageService.OnCurSalvageTypeChanged -= UnitSalvageService_OnCurSalvageTypeChanged;
        }

        public void OnBackButtonClicked()
        {
            if (!PreventMultiClicks()) return;

            UnitSalvageService.Instance.PreviousPart();

            Setup();
        }

        public void OnNextButtonClicked()
        {
            if (!PreventMultiClicks()) return;

            UnitSalvageService.Instance.NextPart();

            Setup();
        }

        public void OnSalvageButtonClicked()
        {
            if (!PreventMultiClicks()) return;

            UnitSalvageService.Instance.Salvage();

            Setup();
        }

        public void OnRecycleButtonClicked()
        {
            if (!PreventMultiClicks()) return;

            UnitSalvageService.Instance.Recycle();
        }

        private void UnitSalvageService_OnCurSalvageTypeChanged(object sender, EventArgs e)
        {
            Setup();
        }

        private void Setup()
        {
            Unit selectedTarget = UnitSelectService.Instance.selectedTarget;
            TorsoData torsoData = (TorsoData)selectedTarget.partsData[PartType.Torso];

            salvageTypeText.text = selectedTarget.partsData[curSalvageType].Name;

            inInventoryText.text = "<b>In Inventory:</b> " + UnitSalvageService.Instance.inInventory[curSalvageType].ToString();
            salvageChanceText.text = "<b>Scavenge Chance:</b> " + (20 + torsoData.SalvageBoost).ToString() +"%";

            float salvageChance = 20f;
            string chanceColor = salvageChance > 20f ? COLOR_POSITIVE : COLOR_WARNING;

            int attemptsLeft = UnitSalvageService.Instance.remainingAttempts;
            string attemptColor = attemptsLeft > 1 ? COLOR_POSITIVE : COLOR_WARNING;
            attemptsLeftText.text = $"<b>Attempts Left:</b> <color={attemptColor}>{attemptsLeft}</color>";

            PartData partData = UnitSalvageService.Instance.salvageableParts[curSalvageType];
            scrapGainText.text = "<b>Scrap Gain:</b> " + (partData.Cost / 2);

            uiBaseTransform.gameObject.SetActive(false);
            uiTorsosTransform.gameObject.SetActive(false);
            uiWeaponsTransform.gameObject.SetActive(false);

            if (partTypeToUI.TryGetValue(curSalvageType, out var activeUI))
            {
                activeUI.gameObject.SetActive(true);
            }
        }

        private bool PreventMultiClicks()
        {
            if (!canClick || UnitSalvageService.Instance.remainingAttempts <= 0) return false;

            canClick = false;

            StopAllCoroutines();
            StartCoroutine(ResetCanClick());

            return true;
        }

        private IEnumerator ResetCanClick()
        {
            yield return new WaitForSecondsRealtime(gameDurations.cameraBlendDuration);

            canClick = true;
        }
    }
}