using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy.Infra
{
    public class CameraChangeMonobService : SingletonBaseService<CameraChangeMonobService>
    {
        public Data.CameraType curCamera;

        [SerializeField] Transform cameraMapTransform;
        [SerializeField] Transform cameraOverheadTransform;
        [SerializeField] Transform cameraAssembleTransform;
        [SerializeField] Transform cameraDriftTransform;
        [SerializeField] Transform cameraActionTransform;
        [SerializeField] Transform cameraSalvageTransform;
        [SerializeField] Transform cameraVictoryTransform;

        private Dictionary<Data.CameraType, Transform> cameraDict;

        private float MaxZoom => CameraControlsMonobService.Instance.MAX_ZOOM;

        protected override void Awake()
        {
            Instance = SetSingleton();
            cameraDict = new Dictionary<Data.CameraType, Transform>
            {
                { Data.CameraType.Map, cameraMapTransform },
                {Data. CameraType.Overhead, cameraOverheadTransform },
                { Data.CameraType.Assembly, cameraAssembleTransform },
                {Data. CameraType.Drift, cameraDriftTransform },
                {Data. CameraType.Action, cameraActionTransform },
                { Data.CameraType.Salvage, cameraSalvageTransform },
                {Data. CameraType.Victory, cameraVictoryTransform }
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
                ChangeCamera(Data.CameraType.Map);
            }
            else if (hasZoomedin)
            {
                ChangeCamera(Data.CameraType.Overhead);
            }
        }

        public void ChangeCamera(Data.CameraType cameraType)
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
