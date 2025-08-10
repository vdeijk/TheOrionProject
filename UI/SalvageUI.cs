using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class SalvageUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI attemptsLeftText;
        [SerializeField] TextMeshProUGUI scrapGainText;
        [SerializeField] TextMeshProUGUI salvageTypeText;
        [SerializeField] Transform uiWeaponsTransform;
        [SerializeField] Transform uiTorsosTransform;
        [SerializeField] Transform uiBaseTransform;
        [SerializeField] Transform rightSectionTransform;
        [SerializeField] TextMeshProUGUI inInventoryText;
        [SerializeField] TextMeshProUGUI salvageChanceText;

        private bool canClick = true;
        private string COLOR_POSITIVE = "#00F1D5";
        private string COLOR_WARNING = "#e10000";
        private Dictionary<PartType, Transform> partTypeToUI;

        private PartType curSalvageType => SalvageService.Instance.Data.curSalvageType;
        private DurationData durationData => DurationData.Instance;

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
            SalvageService.OnCurSalvageTypeChanged += UnitSalvageService_OnCurSalvageTypeChanged;
        }

        private void OnDisable()
        {
            SalvageService.OnCurSalvageTypeChanged -= UnitSalvageService_OnCurSalvageTypeChanged;
        }

        public void OnBackButtonClicked()
        {
            if (!PreventMultiClicks()) return;

            SalvageService.Instance.PreviousPart();

            Setup();
        }

        public void OnNextButtonClicked()
        {
            if (!PreventMultiClicks()) return;

            SalvageService.Instance.NextPart();

            Setup();
        }

        public void OnSalvageButtonClicked()
        {
            if (!PreventMultiClicks()) return;

            SalvageService.Instance.Salvage();

            Setup();
        }

        public void OnRecycleButtonClicked()
        {
            if (!PreventMultiClicks()) return;

            SalvageService.Instance.Recycle();
        }

        private void UnitSalvageService_OnCurSalvageTypeChanged(object sender, EventArgs e)
        {
            Setup();
        }

        private void Setup()
        {
            UnitSingleController selectedTarget = UnitSelectService.Instance.Data.SelectedTarget;
            TorsoSO torsoData = (TorsoSO)selectedTarget.Data.PartsData[PartType.Torso];

            salvageTypeText.text = selectedTarget.Data.PartsData[curSalvageType].Name;

            inInventoryText.text = "<b>In Inventory:</b> " + SalvageService.Instance.Data.InInventory[curSalvageType].ToString();
            salvageChanceText.text = "<b>Scavenge Chance:</b> " + (20 + torsoData.SalvageBoost).ToString() +"%";

            float salvageChance = 20f;
            string chanceColor = salvageChance > 20f ? COLOR_POSITIVE : COLOR_WARNING;

            int attemptsLeft = SalvageService.Instance.Data.RemainingAttempts;
            string attemptColor = attemptsLeft > 1 ? COLOR_POSITIVE : COLOR_WARNING;
            attemptsLeftText.text = $"<b>Attempts Left:</b> <color={attemptColor}>{attemptsLeft}</color>";

            PartSO partData = SalvageService.Instance.Data.SalvageableParts[curSalvageType];
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
            if (!canClick || SalvageService.Instance.Data.RemainingAttempts <= 0) return false;

            canClick = false;

            StopAllCoroutines();
            StartCoroutine(ResetCanClick());

            return true;
        }

        private IEnumerator ResetCanClick()
        {
            yield return new WaitForSecondsRealtime(durationData.CameraBlendDuration);

            canClick = true;
        }
    }
}