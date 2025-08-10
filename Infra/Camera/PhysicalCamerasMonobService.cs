using UnityEngine;
using System.Collections;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.Infra
{
    public class PhysicalCameraMonobService : SingletonBaseService<PhysicalCameraMonobService>
    {
        public PhysicalCameraData Data { get; private set; }

        private Vector3 camOffset = new Vector3(0, 0, -1);

        public void Init(PhysicalCameraData data)
        {
            Data = data;
        }

        // Positions inventory camera to show selected part
        public void SetupInventoryCam()
        {
            Transform part = AssemblyService.Instance.Data.CurPreviewTransform;
            var renderers = part.GetComponentsInChildren<Renderer>();

            // Create a bounding box that encapsulates all the renderers
            Bounds bounds = renderers[0].bounds;
            foreach (var r in renderers)
                bounds.Encapsulate(r.bounds);

            Vector3 boundsCenter = bounds.center;
            float boundsSize = Mathf.Max(bounds.size.x, bounds.size.z);

            Data.InventoryCam.transform.position = boundsCenter + camOffset.normalized * (boundsSize + Data.Distance);
            Data.InventoryCam.transform.LookAt(boundsCenter);
        }

        // Adjusts far clip plane based on camera mode
        public void SetupActionCam()
        {
            if (CameraChangeMonobService.Instance.curCamera == TurnBasedStrategy.Data.CameraType.Action)
            {
                Data.BillboardCam.farClipPlane = 80;
            }
            else
            {
                Data.BillboardCam.farClipPlane = 8000;
            }
        }

        public void SetupBillboardCam()
        {
            StopAllCoroutines();
            switch (CameraChangeMonobService.Instance.curCamera)
            {
                case TurnBasedStrategy.Data.CameraType.Map:
                    FadeWithDelay(true, Data.IconCam.transform);
                    FadeWithoutDelay(false, Data.BillboardCam.transform);
                    break;
                case TurnBasedStrategy.Data.CameraType.Assembly:
                    FadeWithoutDelay(false, Data.IconCam.transform);
                    FadeWithoutDelay(false, Data.IconCam.transform);
                    break;
                case TurnBasedStrategy.Data.CameraType.Overhead:
                    FadeWithoutDelay(false, Data.BillboardCam.transform);
                    FadeWithoutDelay(false, Data.IconCam.transform);
                    FadeWithDelay(true, Data.BillboardCam.transform);
                    break;
                case TurnBasedStrategy.Data.CameraType.Action:
                    FadeWithoutDelay(false, Data.BillboardCam.transform);
                    FadeWithDelay(true, Data.BillboardCam.transform);
                    FadeWithoutDelay(false, Data.IconCam.transform);
                    break;
                default:
                    FadeWithoutDelay(false, Data.BillboardCam.transform);
                    FadeWithoutDelay(false, Data.IconCam.transform);
                    break;
            }
        }

        // Fades cameras in/out based on camera mode
        private void FadeWithDelay(bool isActivated, Transform camera)
        {
            float delay = DurationData.Instance.CameraBlendDuration;
            StartCoroutine(Fade(isActivated, delay, camera));
        }

        private void FadeWithoutDelay(bool isActivated, Transform camera)
        {
            StartCoroutine(Fade(isActivated, 0, camera));
        }


        // Coroutine to activate/deactivate camera after delay
        private IEnumerator Fade(bool isActivated, float delay, Transform camera)
        {
            yield return new WaitForSecondsRealtime(delay);
            camera.gameObject.SetActive(isActivated);
        }
    }
}
