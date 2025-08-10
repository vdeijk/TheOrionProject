using System;
using Unity.Cinemachine;
using UnityEngine;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(-100)]
    public class VCamMapController : MonoBehaviour
    {
        [SerializeField] Transform mapCamTransform;
        [SerializeField] CinemachineCamera mapCam;
        private void Awake()
        {
            mapCamTransform.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            CameraChangeMonobService.OnCameraChanged += CameraChangeService_OnCameraChanged;
        }

        private void OnDisable()
        {
            CameraChangeMonobService.OnCameraChanged -= CameraChangeService_OnCameraChanged;
        }

        // Handles map camera activation and fog density based on menu and camera state
        private void CameraChangeService_OnCameraChanged(object sender, EventArgs e)
        {
            if (CameraChangeMonobService.Instance.curCamera == Data.CameraType.Map)
            {
                var positionComposer = mapCam.GetComponent<CinemachinePositionComposer>();

                if (MenuChangeMonobService.Instance.curMenu == MenuType.Briefing)
                {
                    positionComposer.TargetOffset = new Vector3(-175, 0, 0);
                }
                else
                {
                    positionComposer.TargetOffset = new Vector3(0, 0, 0);
                }

                FogMonobService.Instance.SetToMinDensity();
                mapCamTransform.gameObject.SetActive(true);
            }
            else
            {
                FogMonobService.Instance.SetToMaxDensity();
                mapCamTransform.gameObject.SetActive(false);
            }
        }
    }
}