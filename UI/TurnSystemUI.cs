using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Game;
using TurnBasedStrategy.Domain;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class TurnSystemUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI turnPhaseText;
        [SerializeField] CanvasGroup[] canvasGroups;

        private DurationData durationData => DurationData.Instance;

        private void Awake()
        {
            foreach (CanvasGroup canvasGroup in canvasGroups)
            {
                canvasGroup.alpha = 0;
                canvasGroup.gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            PhaseManager.OnPhaseChanged += ControlModeManager_OnPhaseChanged;
        }

        private void OnDisable()
        {
            PhaseManager.OnPhaseChanged -= ControlModeManager_OnPhaseChanged;
        }

        private void ControlModeManager_OnPhaseChanged(object sender, EventArgs e)
        {
            List<UnitSingleController> allies = (List<UnitSingleController>)UnitCategoryService.Instance.Data.Allies;
            if (allies.Count > 0)
            {
                StopAllCoroutines();
                UpdateHideableUI();
                Enable();
                StartCoroutine(WaitToDisable());
            }
        }

        private void Enable()
        {
            foreach (CanvasGroup canvasGroup in canvasGroups)
            {
                StartCoroutine(MenuFadeMonobService.Instance.Fade(canvasGroup, 1));
            }
        }

        IEnumerator WaitToDisable()
        {
            yield return new WaitForSeconds(durationData.TurnUIVisibility);

            foreach (CanvasGroup canvasGroup in canvasGroups)
            {
                StartCoroutine(MenuFadeMonobService.Instance.Fade(canvasGroup, 0));
            }
        }

        private void UpdateHideableUI()
        {
            if (PhaseManager.Instance.isPlayerPhase)
            {
                turnPhaseText.text = "Your turn";
            }
            else
            {
                turnPhaseText.text = "Enemy turn";
            }
        }
    }
}
