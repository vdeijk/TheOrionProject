using System;
using Unity.Cinemachine;
using UnityEngine;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(-100)]
    public class CamerasMenuController: MonoBehaviour
    {
        [field: SerializeField] public float minCamTargetXRotation { get; private set; } = 15f;
        [field: SerializeField] public float maxCamTargetXRotation { get; private set; } = 45f;
        [field: SerializeField] public float minCamTargetYRotation { get; private set; } = -45f;
        [field: SerializeField] public float maxCamTargetYRotation { get; private set; } = 45f;
        [field: SerializeField] public float minCinemachineDist { get; private set; } = 15f;
        [field: SerializeField] public float maxCinemachineDist { get; private set; } = 45f;
        [field: SerializeField] public CinemachineCamera[] cinemachineCameras { get; private set; }
        [field: SerializeField] public Transform[] trackingTargetTransforms { get; private set; }

        private void OnEnable()
        {
            MenuChangeMonobService.OnMenuChanged += MenuManager_OnMenuChanged;
        }

        private void OnDisable()
        {
            MenuChangeMonobService.OnMenuChanged -= MenuManager_OnMenuChanged;
        }

        private void Start()
        {
            CamerasMenuService.Instance.Init(this);
        }

        // Starts or stops camera cycling based on menu
        private void MenuManager_OnMenuChanged(object sender, EventArgs e)
        {
            switch (MenuChangeMonobService.Instance.curMenu)
            {
                case MenuType.Main:
                case MenuType.NewGame:
                case MenuType.Briefing:
                    if (!CamerasMenuService.Instance.isCamCycleActive)
                    {
                        CamerasMenuService.Instance.StartBackgroundCycle();
                    }
                    break;
                default:
                    if (CamerasMenuService.Instance.isCamCycleActive)
                    {
                        CamerasMenuService.Instance.StopBackgroundCycle();
                    }
                    break;
            }
        }
    }
}
