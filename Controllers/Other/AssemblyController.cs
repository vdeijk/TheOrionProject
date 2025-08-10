using System;
using UnityEngine;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(-100)]
    public class AssemblyController : MonoBehaviour
    {
        [field: SerializeField] public UnitAssemblyData Data { get; private set; }

        [SerializeField] Transform previewParent;
        [SerializeField] float partRotationSpeed = 10f;

        private void Awake()
        {
            AssemblyService.Instance.Init(Data);
        }

        private void OnEnable()
        {
            MenuChangeMonobService.OnMenuChanged += MenuChangeService_OnMenuChange;
        }

        private void OnDisable()
        {
            MenuChangeMonobService.OnMenuChanged -= MenuChangeService_OnMenuChange;
        }

        // Rotates the preview part for visual feedback in the UI
        private void Update()
        {
            if (Data.CurPreviewTransform != null)
            {
                Data.CurPreviewTransform.Rotate(Vector3.up, partRotationSpeed * Time.deltaTime, Space.World);
            }
        }

        // Removes the current part preview from the scene
        public void DestroyPartPreview()
        {
            if (Data.CurPreviewTransform != null)
            {
                Destroy(Data.CurPreviewTransform?.gameObject);
                Data.CurPreviewTransform = null;
            }
        }

        public void DestroyUnitPreview(UnitSingleController unit)
        {
            foreach (Transform child in unit.Data.TransformData[PartType.Base])
            {
                Destroy(child.gameObject);
            }
            Destroy(unit.Data.TransformData[PartType.Base].gameObject);
        }


        // Instantiates and sets up the preview for the selected part
        public void InstantiatePart()
        {
            PartType curPartType = AssemblyService.Instance.Data.CurPartType;
            var curPartData = AssemblyService.Instance.Data.SelectedPart;

            if (curPartData != null && curPartData.Prefab != null)
            {
                DestroyPartPreview();

                Data.CurPreviewTransform = Instantiate(curPartData.Prefab);
                Data.CurPreviewTransform.SetParent(previewParent, false);
                Data.CurPreviewTransform.localPosition = new Vector3(-500, 0, -500); // Off-screen until positioned
                Data.CurPreviewTransform.localRotation = Quaternion.identity;

                // Set highlight light layer for preview renderers
                uint lightLayer = UnitMaterialService.Instance.Data.LightLayerUintWithHighlight;
                Renderer[] rs = Data.CurPreviewTransform.GetComponentsInChildren<Renderer>();
                UnitMaterialService.Instance.SetLightLayers(lightLayer, rs);

                // Sync post-processing volume position with preview
                InventoryVolumeMonobService.Instance.SetPosition();
            }
        }

        // Removes preview when leaving the assembly menu
        private void MenuChangeService_OnMenuChange(object sender, EventArgs e)
        {
            if (MenuChangeMonobService.Instance.curMenu != MenuType.Assemble)
            {
                DestroyPartPreview();
            }
        }
    }
}