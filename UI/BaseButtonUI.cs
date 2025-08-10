using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    // Abstract base class for UI buttons with custom state and color transitions
    public abstract class BaseButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        public enum ButtonState
        {
            Normal,
            Hover,
            Pressed,
            Disabled,
            Selected
        }

        [SerializeField] protected Button button;
        [SerializeField] protected Image buttonImage;

        protected bool isSelected = false;

        private void OnEnable()
        {
            SetButtonState(ButtonState.Normal, false); // Reset to normal state on enable
        }

        protected virtual void Start()
        {
            button.onClick.AddListener(OnButtonClicked); // Register click event
        }

        // Handles pointer enter event for hover effect
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            SetButtonState(GetButtonState(eventData), true);
        }

        // Handles pointer exit event to restore state
        public virtual void OnPointerExit(PointerEventData eventData)
        {
            SetButtonState(GetButtonState(eventData), true);
        }

        // Handles pointer down event for pressed effect
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            SetButtonState(GetButtonState(eventData), true);
        }

        // Handles pointer up event to restore state
        public virtual void OnPointerUp(PointerEventData eventData)
        {
            SetButtonState(GetButtonState(eventData), true);
        }

        // Sets the button's color based on its state, optionally with a smooth transition
        public void SetButtonState(ButtonState state, bool isSmoothed)
        {
            Color targetColor = state switch
            {
                ButtonState.Normal => ColorData.Instance.buttonNormal,
                ButtonState.Hover => ColorData.Instance.buttonHover,
                ButtonState.Pressed => ColorData.Instance.buttonPressed,
                ButtonState.Disabled => ColorData.Instance.buttonDisabled,
                ButtonState.Selected => ColorData.Instance.buttonSelected,
                _ => ColorData.Instance.buttonNormal
            };

            if (isSmoothed && gameObject.activeInHierarchy)
            {
                StopAllCoroutines();
                StartCoroutine(ButtonTransitionMonobService.Instance.LerpColor(buttonImage, targetColor));
            }
            else
            {
                buttonImage.color = targetColor;
            }
        }

        // Returns the current button state based on selection and interactability
        public ButtonState GetButtonState()
        {
            if (!button.interactable)
            {
                return ButtonState.Disabled;
            }

            if (isSelected)
            {
                return ButtonState.Selected;
            }

            return ButtonState.Normal;
        }

        // Called when the button is clicked; must be implemented by derived classes
        protected abstract void OnButtonClicked();

        // Determines button state based on pointer event data
        private ButtonState GetButtonState(PointerEventData eventData)
        {
            if (!button.interactable)
            {
                return ButtonState.Disabled;
            }

            bool hasEventData = eventData != null;
            bool isPressed = eventData.button == PointerEventData.InputButton.Left &&
                eventData.pressPosition == eventData.position;
            bool isHovering = hasEventData && RectTransformUtility.RectangleContainsScreenPoint(
                   transform as RectTransform,
                   eventData.position,
                   eventData.pressEventCamera);

            if (isPressed)
            {
                return ButtonState.Pressed;
            }

            if (isHovering)
            {
                return ButtonState.Hover;
            }

            if (isSelected)
            {
                return ButtonState.Selected;
            }

            return ButtonState.Normal;
        }
    }
}
