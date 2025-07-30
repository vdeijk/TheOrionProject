using TMPro;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class EndTurnUI : MonoBehaviour
    {
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] TextMeshProUGUI endTurnText;

        private bool isInMission => ControlModeManager.Instance.gameControlType == GameControlType.Mission;

        private void OnEnable()
        {
            UnitCategoryService.OnUnitRemoved += UnitCategoryService_OnUnitRemoved;
            MenuChangeService.OnMenuChanged += MenuChangeService_OnMenuChanged;
        }

        private void OnDisable()
        {
            UnitCategoryService.OnUnitRemoved -= UnitCategoryService_OnUnitRemoved;
            MenuChangeService.OnMenuChanged -= MenuChangeService_OnMenuChanged;
        }

        private void Start()
        {
            endTurnText.text = "BACK";
        }

        public void HandleClick()
        {
            int allies = UnitCategoryService.Instance.allies.Count;
            int enemies = UnitCategoryService.Instance.enemies.Count;
            BaseAction baseAction = UnitActionSystem.Instance.selectedAction;

            if (allies <= 0)
            {
                StartCoroutine(ControlModeManager.Instance.EnterDefeatMode());
            }
            else if (enemies <= 0)
            {
                StartCoroutine(ControlModeManager.Instance.EnterVictoryMode());
            }
            else if (!isInMission)
            {
                StartCoroutine(ControlModeManager.Instance.EnterPrepMode(false));
            }
            else if (baseAction == null || !baseAction.isActive)
            {
                PhaseManager.Instance.SetNextPhase();
            }
        }

        private void MenuChangeService_OnMenuChanged(object sender, System.EventArgs e)
        {
            Check();
        }

        private void UnitCategoryService_OnUnitRemoved(object sender, System.EventArgs e)
        {
            Check();
        }

        private void Check()
        {
            int allies = UnitCategoryService.Instance.allies.Count;
            int enemies = UnitCategoryService.Instance.enemies.Count;

            if (allies <= 0 || enemies <= 0)
            {
                endTurnText.text = "END MISSION";
            }
            else if (isInMission)
            {
                endTurnText.text = "END TURN";
            }
            else
            {
                endTurnText.text = "BACK";
            }
        }
    }
}
