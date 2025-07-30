using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TurnBasedStrategy
{
    public class UIFlicker : MonoBehaviour
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
            MenuChangeService.OnMenuChanged += MenuChangeService_OnMenuChange;
        }

        private void OnDisable()
        {
            MenuChangeService.OnMenuChanged -= MenuChangeService_OnMenuChange;
        }

        private void MenuChangeService_OnMenuChange(object sender, EventArgs e)
        {
            if (MenuChangeService.Instance.curMenu == menuType)
            {
                StopAllCoroutines();
                StartCoroutine(UIFlickerService.Instance.FlickerElements(circuitBorderElements));
            }
        }
    }
}