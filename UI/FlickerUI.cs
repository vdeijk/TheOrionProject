using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    public class FlickerUI : MonoBehaviour
    {
        [SerializeField] Transform flickerElementsParent;
        [SerializeField] MenuType menuType = MenuType.Main;

        private Image[] circuitBorderElements;

        private void Awake()
        {
            circuitBorderElements = flickerElementsParent.GetComponentsInChildren<Image>();
        }

        private void OnEnable()
        {
            MenuChangeMonobService.OnMenuChanged += MenuChangeService_OnMenuChange;
        }

        private void OnDisable()
        {
            MenuChangeMonobService.OnMenuChanged -= MenuChangeService_OnMenuChange;
        }

        private void MenuChangeService_OnMenuChange(object sender, EventArgs e)
        {
            if (MenuChangeMonobService.Instance.curMenu == menuType)
            {
                StopAllCoroutines();
                StartCoroutine(UIFlickerMonobService.Instance.FlickerElements(circuitBorderElements));
            }
        }
    }
}