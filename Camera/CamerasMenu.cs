using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class CamerasMenu : Singleton<CamerasMenu>
    {
        [SerializeField] CinemachineCamera[] cinemachineCameras;
        [SerializeField] float viewTime;
        [SerializeField] Transform[] trackingTargetTransforms;
        [SerializeField] float minCamTargetXRotation = 15f;
        [SerializeField] float maxCamTargetXRotation = 45f;
        [SerializeField] float minCamTargetYRotation = -45f;
        [SerializeField] float maxCamTargetYRotation = 45f;
        [SerializeField] float minCinemachineDist = 15f;
        [SerializeField] float maxCinemachineDist = 45f;

        private int curCamIndex = 0;
        private bool isCamCycleActive = false;
        private int curEnemyIndex = 0;

        private void OnEnable()
        {
            MenuChangeService.OnMenuChanged += MenuManager_OnMenuChanged;
        }

        private void OnDisable()
        {
            MenuChangeService.OnMenuChanged -= MenuManager_OnMenuChanged;
        }

        // Starts or stops camera cycling based on menu
        private void MenuManager_OnMenuChanged(object sender, EventArgs e)
        {
            switch (MenuChangeService.Instance.curMenu)
            {
                case MenuType.Main:
                case MenuType.NewGame:
                case MenuType.Briefing:
                    if (!isCamCycleActive)
                    {
                        StartBackgroundCycle();
                    }
                    break;
                default:
                    if (isCamCycleActive)
                    {
                        StopBackgroundCycle();
                    }
                    break;
            }
        }

        private void StartBackgroundCycle()
        {
            isCamCycleActive = true;
            curCamIndex = 0;

            trackingTargetTransforms[0].position = GetEnemyPosition();
            trackingTargetTransforms[0].localEulerAngles = GetRandomCamTargetRotation();

            int nextCameraIndex = (curCamIndex + 1) % cinemachineCameras.Length;

            trackingTargetTransforms[nextCameraIndex].position = GetEnemyPosition();
            trackingTargetTransforms[nextCameraIndex].localEulerAngles = GetRandomCamTargetRotation();

            var positionComposer = cinemachineCameras[nextCameraIndex].GetComponent<CinemachinePositionComposer>();
            positionComposer.CameraDistance = GetRandomCinemachineDist();

            cinemachineCameras[nextCameraIndex].gameObject.SetActive(true);
            if (cinemachineCameras[curCamIndex].gameObject.activeSelf)
            {
                cinemachineCameras[curCamIndex].gameObject.SetActive(false);
            }

            curCamIndex = nextCameraIndex;
        }

        private void StopBackgroundCycle()
        {
            isCamCycleActive = false;
            StopAllCoroutines();

            foreach (CinemachineCamera camera in cinemachineCameras)
            {
                camera.gameObject.SetActive(false);
            }
        }

        private float GetRandomCinemachineDist()
        {
            return UnityEngine.Random.Range(minCinemachineDist, maxCinemachineDist);
        }

        private Vector3 GetRandomCamTargetRotation()
        {
            float rotX = UnityEngine.Random.Range(minCamTargetXRotation, maxCamTargetXRotation);
            float rotY = UnityEngine.Random.Range(minCamTargetYRotation, maxCamTargetYRotation);
            return new Vector3(rotX, rotY, 0);
        }

        // Cycles through enemy positions for camera targets
        private Vector3 GetEnemyPosition()
        {
            List<Unit> enemies = UnitCategoryService.Instance.enemies;

            if (curEnemyIndex < UnitCategoryService.Instance.enemies.Count - 1)
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
