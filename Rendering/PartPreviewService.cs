using System;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class PartPreviewService : Singleton<PartPreviewService>
    {
        public Transform curPreviewTransform { get; private set; }

        [SerializeField] Transform previewParent;
        [SerializeField] float partRotationSpeed = 10f;

        private void OnEnable()
        {
            MenuChangeService.OnMenuChanged += MenuChangeService_OnMenuChange;
        }

        private void OnDisable()
        {
            MenuChangeService.OnMenuChanged -= MenuChangeService_OnMenuChange;
        }

        // Rotates the preview part for visual feedback in the UI
        private void Update()
        {
            if (curPreviewTransform != null)
            {
                curPreviewTransform.Rotate(Vector3.up, partRotationSpeed * Time.deltaTime, Space.World);
            }
        }

        // Removes the current part preview from the scene
        public void RemovePreview()
        {
            if (curPreviewTransform != null)
            {
                Destroy(curPreviewTransform?.gameObject);
                curPreviewTransform = null;
            }
        }

        // Instantiates and sets up the preview for the selected part
        public void SelectPart()
        {
            PartType curPartType = UnitAssemblyService.Instance.curPartType;
            var curPartData = UnitAssemblyService.Instance.selectedPart;

            if (curPartData != null && curPartData.Prefab != null)
            {
                RemovePreview();
                curPreviewTransform = Instantiate(curPartData.Prefab);
                curPreviewTransform.SetParent(previewParent, false);
                curPreviewTransform.localPosition = new Vector3(-500, 0, -500); // Off-screen until positioned
                curPreviewTransform.localRotation = Quaternion.identity;

                // Set highlight light layer for preview renderers
                uint lightLayer = UnitMaterialService.Instance.lightLayerUintWithHighlight;
                Renderer[] rs = curPreviewTransform.GetComponentsInChildren<Renderer>();
                UnitMaterialService.Instance.SetLightLayers(lightLayer, rs);

                // Sync post-processing volume position with preview
                InventoryVolume.Instance.SetPosition();
            }
        }

        // Removes preview when leaving the assembly menu
        private void MenuChangeService_OnMenuChange(object sender, EventArgs e)
        {
            if (MenuChangeService.Instance.curMenu != MenuType.Assemble)
            {
                RemovePreview();
            }
        }
    }
}