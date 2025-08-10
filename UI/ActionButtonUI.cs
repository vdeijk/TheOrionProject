using TMPro;
using UnityEngine;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.UI
{
    [DefaultExecutionOrder(300)]
    // Base class for all action buttons (move, shoot, pass), inherits from BaseButtonUI
    public abstract class ActionButtonUI : BaseButtonUI
    {
        [SerializeField] protected TextMeshProUGUI actionNameField;
        [SerializeField] protected TextMeshProUGUI actionRangeField;
        [SerializeField] protected TextMeshProUGUI actionTypeField;

        [field: SerializeField] public ActionType actionType { get; protected set; }

        protected override void Start()
        {
            base.Start();
            // Display action type as button label
            actionNameField.text = actionType.ToString().ToUpper();
        }

        public abstract void UpdateButton();

        protected override void OnButtonClicked()
        {
            switch(actionType)
            {
                case ActionType.Move:
                    ActionCoordinatorService.Instance.SelectMoveAction();
                    break;
                case ActionType.ShootPrimary:
                case ActionType.ShootSecondary:
                    ActionCoordinatorService.Instance.SelectShootAction();
                    break;
                case ActionType.Pass:
                    ActionCoordinatorService.Instance.SelectPassAction();
                    break;
            }
        }
    }
}