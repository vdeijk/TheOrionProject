using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy
{
    [DefaultExecutionOrder(100)]
    public class CamerasMenuService
    {
        public bool isCamCycleActive { get; private set; } = false;

        private int curCamIndex = 0;
        private int curEnemyIndex = 0;
        private static CamerasMenuService _instance;

        private CamerasMenuController controller;

        public static CamerasMenuService Instance => _instance ??= new CamerasMenuService();

        public void Init(CamerasMenuController camerasMenu)
        {
            controller = camerasMenu;
        }

        public void StartBackgroundCycle()
        {
            isCamCycleActive = true;
            curCamIndex = 0;

            controller.trackingTargetTransforms[0].position = GetEnemyPosition();
            controller.trackingTargetTransforms[0].localEulerAngles = GetRandomCamTargetRotation();

            int nextCameraIndex = (curCamIndex + 1) % controller.cinemachineCameras.Length;

            controller.trackingTargetTransforms[nextCameraIndex].position = GetEnemyPosition();
            controller.trackingTargetTransforms[nextCameraIndex].localEulerAngles = GetRandomCamTargetRotation();

            var positionComposer = controller.cinemachineCameras[nextCameraIndex].GetComponent<CinemachinePositionComposer>();
            positionComposer.CameraDistance = GetRandomCinemachineDist();

            controller.cinemachineCameras[nextCameraIndex].gameObject.SetActive(true);
            if (controller.cinemachineCameras[curCamIndex].gameObject.activeSelf)
            {
                controller.cinemachineCameras[curCamIndex].gameObject.SetActive(false);
            }

            curCamIndex = nextCameraIndex;
        }

        public void StopBackgroundCycle()
        {
            isCamCycleActive = false;

            foreach (CinemachineCamera camera in controller.cinemachineCameras)
            {
                camera.gameObject.SetActive(false);
            }
        }

        private float GetRandomCinemachineDist()
        {
            return Random.Range(controller.minCinemachineDist, controller.maxCinemachineDist);
        }

        private Vector3 GetRandomCamTargetRotation()
        {
            float rotX = Random.Range(controller.minCamTargetXRotation, controller.maxCamTargetXRotation);
            float rotY = Random.Range(controller.minCamTargetYRotation, controller.maxCamTargetYRotation);
            return new Vector3(rotX, rotY, 0);
        }

        // Cycles through enemy positions for camera targets
        private Vector3 GetEnemyPosition()
        {
            List<UnitSingleController> enemies = (List<UnitSingleController>)UnitCategoryService.Instance.Data.Enemies;

            if (curEnemyIndex < UnitCategoryService.Instance.Data.Enemies.Count - 1)
            {
                curEnemyIndex++;
            }
            else
            {
                curEnemyIndex = 0;
            }

            return enemies[curEnemyIndex].transform.position;
        }
    }
}
