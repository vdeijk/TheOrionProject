using System;
using System.Collections;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class BriefingFading : MenuFading
    {
        [SerializeField] BriefingContent briefingContent;
        [SerializeField] GameDurations gameDurations;

        protected override void MenuChangeService_OnMenuChange(object sender, EventArgs e)
        {
            if (MenuChangeService.Instance.curMenu == menuType)
            {
                MenuFadeService.Instance.Fade(true, canvasGroupsMenu);

                if (canvasGroupsElements == null || canvasGroupsElements.Length <= 0) return;

                StopAllCoroutines();

                RebuildSequence();
            }
            else
            {
                MenuFadeService.Instance.Fade(false, canvasGroupsMenu);
            }
        }

        private IEnumerator RebuildSequence()
        {
            yield return new WaitForSeconds(gameDurations.uiTransitionuration);

            foreach(CanvasGroup canvasGroup in canvasGroupsElements)
            {
                canvasGroup.alpha = 0;
                canvasGroup.gameObject.SetActive(true);
            }

            yield return null;

            StartCoroutine(MenuAnimationService.Instance.RevealAll(canvasGroupsElements));
        }

    }
}