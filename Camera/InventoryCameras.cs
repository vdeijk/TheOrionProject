using UnityEngine;

namespace TurnBasedStrategy
{
    public class InventoryCameras : Singleton<InventoryCameras>
    {
        [SerializeField] Camera inventoryCam;
        [SerializeField] float distance = 8f;
        private Vector3 camOffset = new Vector3(0, 0, -1);

        private void OnEnable()
        {
            UnitAssemblyService.OnPartSelected += UnitAssemblyService_OnPartSelected;
        }

        private void OnDisable()
        {
            UnitAssemblyService.OnPartSelected -= UnitAssemblyService_OnPartSelected;
        }

        // Positions inventory camera to show selected part
        public void PlaceInventoryCam()
        {
            Transform part = PartPreviewService.Instance.curPreviewTransform;
            var renderers = part.GetComponentsInChildren<Renderer>();

            // Create a bounding box that encapsulates all the renderers
            Bounds bounds = renderers[0].bounds;
            foreach (var r in renderers)
                bounds.Encapsulate(r.bounds);

            Vector3 boundsCenter = bounds.center;
            float boundsSize = Mathf.Max(bounds.size.x, bounds.size.z);

            // Set camera position and orientation
            inventoryCam.transform.position = boundsCenter + camOffset.normalized * (boundsSize + distance);
            inventoryCam.transform.LookAt(boundsCenter);
        }

        private void UnitAssemblyService_OnPartSelected(object sender, System.EventArgs e)
        {
            PlaceInventoryCam();
        }
    }
}