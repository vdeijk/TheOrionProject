using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class TurnSystemUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI turnPhaseText;
        [SerializeField] CanvasGroup[] canvasGroups;
        [SerializeField] GameDurations gameDurations;

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
            List<Unit> allies = UnitCategoryService.Instance.allies;
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
                StartCoroutine(MenuFadeService.Instance.Fade(canvasGroup, 1));
            }
        }

        IEnumerator WaitToDisable()
        {
            yield return new WaitForSeconds(gameDurations.turnUIVisibility);

            foreach (CanvasGroup canvasGroup in canvasGroups)
            {
                StartCoroutine(MenuFadeService.Instance.Fade(canvasGroup, 0));
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
