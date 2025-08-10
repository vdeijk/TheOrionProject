using System;
using System.Collections;
using UnityEngine;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class BriefingFadingUI : MenuFadingUI
    {
        [SerializeField] BriefingContentUI briefingContent;

        private DurationData durationData => DurationData.Instance;

        protected override void MenuChangeService_OnMenuChange(object sender, EventArgs e)
        {
            if (MenuChangeMonobService.Instance.curMenu == menuType)
            {
                MenuFadeMonobService.Instance.Fade(true, canvasGroupsMenu);

                if (canvasGroupsElements == null || canvasGroupsElements.Length <= 0) return;

                StopAllCoroutines();

                RebuildSequence();
            }
            else
            {
                MenuFadeMonobService.Instance.Fade(false, canvasGroupsMenu);
            }
        }

        private IEnumerator RebuildSequence()
        {
            yield return new WaitForSeconds(durationData.UiTransitionuration);

            foreach(CanvasGroup canvasGroup in canvasGroupsElements)
            {
                canvasGroup.alpha = 0;
                canvasGroup.gameObject.SetActive(true);
            }

            yield return null;

            StartCoroutine(MenuAnimationMonobService.Instance.RevealAll(canvasGroupsElements));
        }

    }
}