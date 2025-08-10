using System;
using UnityEngine;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class MenuFadingUI : MonoBehaviour
    {
        [SerializeField] protected float initialValue;
        [SerializeField] protected CanvasGroup[] canvasGroupsMenu;
        [SerializeField] protected CanvasGroup[] canvasGroupsElements;
        [SerializeField] protected MenuType menuType;

        private void OnEnable()
        {
            MenuChangeMonobService.OnMenuChanged += MenuChangeService_OnMenuChange;
        }

        private void OnDisable()
        {
            MenuChangeMonobService.OnMenuChanged -= MenuChangeService_OnMenuChange;
        }

        private void Start()
        {
            foreach (CanvasGroup canvasGroup in canvasGroupsMenu)
            {
                canvasGroup.alpha = initialValue;

                if (initialValue <= 0)
                {
                    canvasGroup.gameObject.SetActive(false);
                }
            }
        }

        protected virtual void MenuChangeService_OnMenuChange(object sender, EventArgs e)
        {
            if (MenuChangeMonobService.Instance.curMenu == menuType)
            {
                MenuFadeMonobService.Instance.Fade(true, canvasGroupsMenu);

                if (canvasGroupsElements == null || canvasGroupsElements.Length <= 0) return;

                StopAllCoroutines();
                StartCoroutine(MenuAnimationMonobService.Instance.RevealAll(canvasGroupsElements));
            }
            else
            {
                MenuFadeMonobService.Instance.Fade(false, canvasGroupsMenu);
            }
        }
    }
}