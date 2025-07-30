using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class CameraChangeService : Singleton<CameraChangeService>
    {
        public CameraType curCamera;

        [SerializeField] Transform cameraMapTransform;
        [SerializeField] Transform cameraOverheadTransform;
        [SerializeField] Transform cameraAssembleTransform;
        [SerializeField] Transform cameraDriftTransform;
        [SerializeField] Transform cameraActionTransform;
        [SerializeField] Transform cameraSalvageTransform;
        [SerializeField] Transform cameraVictoryTransform;

        private Dictionary<CameraType, Transform> cameraDict;

        private float MaxZoom => CameraControlsService.Instance.MAX_ZOOM;

        protected override void Awake()
        {
            Instance = SetSingleton();
            cameraDict = new Dictionary<CameraType, Transform>
            {
                { CameraType.Map, cameraMapTransform },
                { CameraType.Overhead, cameraOverheadTransform },
                { CameraType.Assembly, cameraAssembleTransform },
                { CameraType.Drift, cameraDriftTransform },
                { CameraType.Action, cameraActionTransform },
                { CameraType.Salvage, cameraSalvageTransform },
                { CameraType.Victory, cameraVictoryTransform }
            };
        }

        public static event EventHandler OnCameraChanged;

        // Switches between map and overhead cameras based on zoom
        public void ToggleMapAndOverheadCameras(float targetZoom)
        {
            bool hasZoomedOut = targetZoom >= MaxZoom && !cameraMapTransform.gameObject.activeSelf;
            bool hasZoomedin = targetZoom < MaxZoom && cameraMapTransform.gameObject.activeSelf;

            if (hasZoomedOut)
            {
                ChangeCamera(CameraType.Map);
            }
            else if (hasZoomedin)
            {
                ChangeCamera(CameraType.Overhead);
            }
        }

        public void ChangeCamera(CameraType cameraType)
        {
            curCamera = cameraType;
            DisableAll();
            cameraDict[cameraType].gameObject.SetActive(true);
            OnCameraChanged(this, EventArgs.Empty);
        }

        private void DisableAll()
        {
            foreach (Transform camTransform in cameraDict.Values)
            {
                camTransform.gameObject.SetActive(false);
            }
        }
    }
}
