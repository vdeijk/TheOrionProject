using UnityEngine;
using System.Collections.Generic;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;

namespace TurnBasedStrategy
{
    [DefaultExecutionOrder(100)]
    // Handles material assignment and light layers for unit parts
    public class UnitMaterialService
    {
        public static UnitMaterialService Instance => _instance ??= new UnitMaterialService();

        private static UnitMaterialService _instance;

        public UnitMaterialData Data { get; private set; }

        public void Init(UnitMaterialData data)
        {
            Data = data;
        }

        // Sets light layers for an array of renderers
        public void SetLightLayers(uint lightLayerUint, Renderer[] rs)
        {
            foreach (Renderer r in rs)
            {
                r.renderingLayerMask = lightLayerUint;
            }
        }

        // Assigns materials and light layers to all unit parts
        public void SetMesh(UnitSingleController unit, uint lightLayerUint)
        {
            foreach (var partEntry in unit.Data.PartsData)
            {
                PartType partType = partEntry.Key;
                PartSO partData = partEntry.Value;
                if (partData == null) continue;
                Material material = GetUnitMaterial(partData.unitMaterialType);
                if (unit.Data.TransformData.TryGetValue(partType, out Transform partTransform))
                {
                    SetMaterials(partTransform, material, lightLayerUint, GetAllRenderers(partTransform));
                }
            }
            Transform basePartTransform = unit.Data.TransformData[PartType.Base];
            UnitMaterialsController unitMaterials = basePartTransform.GetComponentInChildren<UnitMaterialsController>();
            if (unitMaterials == null) return;
            if (unitMaterials.propellerRenderers != null)
            {
                SetMaterials(basePartTransform, Data.PropellerMaterial, lightLayerUint, unitMaterials.propellerRenderers);
            }
            if (unitMaterials.tracksRenderers != null)
            {
                SetMaterials(basePartTransform, Data.TrackMaterial, lightLayerUint, unitMaterials.tracksRenderers);
            }
        }

        // Sets material and light layer for an array of renderers
        public void SetMaterials(Transform partTransform, Material newMaterial, uint lightLayerUint, Renderer[] rs)
        {
            foreach (Renderer r in rs)
            {
                Material[] materials = r.materials;
                materials[0] = newMaterial;
                r.materials = materials;
                r.renderingLayerMask = lightLayerUint;
            }
        }

        // Assigns highlight mesh to all units in the list
        public void SetAllMeshes(List<UnitSingleController> units)
        {
            foreach (UnitSingleController unit in units)
            {
                SetMesh(unit, Data.LightLayerUintWithHighlight);
            }
        }

        // Gets all mesh and skinned mesh renderers under a transform
        public Renderer[] GetAllRenderers(Transform parentTransform)
        {
            var allRenderers = new List<Renderer>();
            allRenderers.AddRange(parentTransform.GetComponentsInChildren<MeshRenderer>(true));
            allRenderers.AddRange(parentTransform.GetComponentsInChildren<SkinnedMeshRenderer>(true));
            return allRenderers.ToArray();
        }

        // Returns the correct material for a unit part type
        private Material GetUnitMaterial(UnitMaterialType unitMaterialType)
        {
            switch (unitMaterialType)
            {
                case UnitMaterialType.Spider:
                    return Data.SpiderMat;
                case UnitMaterialType.Vehicle:
                    return Data.VehicleMat;
                case UnitMaterialType.Turret:
                    return Data.TurretMat;
                default:
                    throw null;
            }
        }
    }
}

