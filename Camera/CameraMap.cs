using System;
using Unity.Cinemachine;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class CameraMap : MonoBehaviour
    {
        [SerializeField] Transform mapCamTransform;
        [SerializeField] CinemachineCamera mapCam;
        private void Awake()
        {
            mapCamTransform.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            CameraChangeService.OnCameraChanged += CameraChangeService_OnCameraChanged;
        }

        private void OnDisable()
        {
            CameraChangeService.OnCameraChanged -= CameraChangeService_OnCameraChanged;
        }

        // Handles map camera activation and fog density based on menu and camera state
        private void CameraChangeService_OnCameraChanged(object sender, EventArgs e)
        {
            if (CameraChangeService.Instance.curCamera == CameraType.Map)
            {
                var positionComposer = mapCam.GetComponent<CinemachinePositionComposer>();

                if (MenuChangeService.Instance.curMenu == MenuType.Briefing)
                {
                    positionComposer.TargetOffset = new Vector3(-175, 0, 0);
                }
                else
                {
                    positionComposer.TargetOffset = new Vector3(0, 0, 0);
                }

                FogController.Instance.SetToMinDensity();
                mapCamTransform.gameObject.SetActive(true);
            }
            else
            {
                FogController.Instance.SetToMaxDensity();
                mapCamTransform.gameObject.SetActive(false);
            }
        }
    }
}