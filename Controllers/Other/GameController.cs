using UnityEngine;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Game;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(200)]
    public class GameController : MonoBehaviour
    {
        [field: SerializeField] public Transform defaultPosTransform { get; private set; }
        [field: SerializeField] public PartSO starterBase { get; private set; }
        [field: SerializeField] public PartSO starterTorso { get; private set; }
        [field: SerializeField] public PartSO starterWeapon { get; private set; }
        [field: SerializeField] public float FastScale { get; private set; } = 1;
        [field: SerializeField] public float NormalScale { get; private set; } = 2;
        [field: SerializeField] public float MaxScrap { get; private set; } = 99999;

        private void Start()
        {
            SaveDataManager.Instance.Init();
            SaveSettingsManager.Instance.Init();
            PartsManager.Instance.Init(this);
            ScrapManager.Instance.Init(this);
            TimeScaleManager.Instance.Init(this);
            FrameRateManager.Instance.Init();
            ControlModeManager.Instance.Init(this);

            ControlModeManager.Instance.EnterMenuMode();
        }

        // Central update loop for input handling and game control
        private void Update()
        {
            InputManager.Instance.ObtainUserInputs();
            InputManager.Instance.HandlePauseInput();

            if (InputManager.Instance.areControlsEnabled)
            {
                InputManager.Instance.HandleCameraInput();
                InputManager.Instance.HandleMouseInput();
            }
        }
    }
}