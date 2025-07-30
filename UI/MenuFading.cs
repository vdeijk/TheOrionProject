using System;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class MenuFading : MonoBehaviour
    {
        [SerializeField] protected float initialValue;
        [SerializeField] protected CanvasGroup[] canvasGroupsMenu;
        [SerializeField] protected CanvasGroup[] canvasGroupsElements;
        [SerializeField] protected MenuType menuType;

        private void OnEnable()
        {
            MenuChangeService.OnMenuChanged += MenuChangeService_OnMenuChange;
        }

        private void OnDisable()
        {
            MenuChangeService.OnMenuChanged -= MenuChangeService_OnMenuChange;
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
            if (MenuChangeService.Instance.curMenu == menuType)
            {
                MenuFadeService.Instance.Fade(true, canvasGroupsMenu);

                if (canvasGroupsElements == null || canvasGroupsElements.Length <= 0) return;

                StopAllCoroutines();
                StartCoroutine(MenuAnimationService.Instance.RevealAll(canvasGroupsElements));
            }
            else
            {
                MenuFadeService.Instance.Fade(false, canvasGroupsMenu);
            }
        }
    }
}