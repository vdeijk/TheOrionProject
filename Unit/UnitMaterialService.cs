using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace TurnBasedStrategy
{
    // Handles material assignment and light layers for unit parts
    public class UnitMaterialService : Singleton<UnitMaterialService>
    {
        public uint lightLayerUintWithHighlight { get; private set; }
        public uint lightLayerUintWithoutHighlight { get; private set; }
        [field: SerializeField] public Material hologramMaterial { get; private set; }
        [SerializeField] Material spawnMaterial;
        [SerializeField] Material spiderMat;
        [SerializeField] Material vehicleMat;
        [SerializeField] Material turretMat;
        [SerializeField] Material trackMaterial;
        [SerializeField] Material propellerMaterial;
        [SerializeField] LightLayerEnum lightLayerDefault;
        [SerializeField] LightLayerEnum lightLayerHighlight;
        [SerializeField] GameDurations gameDurations;
        [SerializeField] float spawnProgress = 0.6f;

        protected override void Awake()
        {
            Instance = SetSingleton();
            lightLayerUintWithHighlight = (uint)lightLayerDefault | (uint)lightLayerHighlight;
            lightLayerUintWithoutHighlight = (uint)lightLayerDefault;
        }

        private void OnEnable()
        {
            UnitActionSystem.Instance.OnActionCompleted += UnitActionSystem_OnActionCompleted;
            PhaseManager.OnPhaseChanged += ControlModeManager_OnPhaseChanged;
            UnitCategoryService.OnUnitAdded += UnitManager_OnUnitAdded;
        }

        private void OnDisable()
        {
            UnitActionSystem.Instance.OnActionCompleted -= UnitActionSystem_OnActionCompleted;
            PhaseManager.OnPhaseChanged -= ControlModeManager_OnPhaseChanged;
            UnitCategoryService.OnUnitAdded -= UnitManager_OnUnitAdded;
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
        public void SetMesh(Unit unit, uint lightLayerUint)
        {
            foreach (var partEntry in unit.partsData)
            {
                PartType partType = partEntry.Key;
                PartData partData = partEntry.Value;
                if (partData == null) continue;
                Material material = GetUnitMaterial(partData.unitMaterialType);
                if (unit.transformData.TryGetValue(partType, out Transform partTransform))
                {
                    SetMaterials(partTransform, material, lightLayerUint, GetAllRenderers(partTransform));
                }
            }
            Transform basePartTransform = unit.transformData[PartType.Base];
            UnitMaterials unitMaterials = basePartTransform.GetComponentInChildren<UnitMaterials>();
            if (unitMaterials == null) return;
            if (unitMaterials.propellerRenderers != null)
            {
                SetMaterials(basePartTransform, propellerMaterial, lightLayerUint, unitMaterials.propellerRenderers);
            }
            if (unitMaterials.tracksRenderers != null)
            {
                SetMaterials(basePartTransform, trackMaterial, lightLayerUint, unitMaterials.tracksRenderers);
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
        public void SetAllMeshes(List<Unit> units)
        {
            foreach (Unit unit in units)
            {
                SetMesh(unit, lightLayerUintWithHighlight);
            }
        }

        // Coroutine for unit spawn material fade-in
        private void UnitManager_OnUnitAdded(object sender, UnitCategoryService.OnUnitAddedArgs e)
        {
            StartCoroutine(SpawnUnit(e.unit));
        }

        // Removes highlight when action points are depleted
        private void UnitActionSystem_OnActionCompleted(object sender, EventArgs e)
        {
            Unit unit = UnitSelectService.Instance.selectedUnit;
            if (unit != null && unit.unitMindTransform.GetComponent<ActionSystem>().actionPoints <= 0)
            {
                SetMesh(unit, lightLayerUintWithoutHighlight);
            }
        }

        // Adds highlight mesh to all units on phase change
        private void ControlModeManager_OnPhaseChanged(object sender, EventArgs e)
        {
            Unit[] allUnits = UnitCategoryService.Instance.all.ToArray();
            foreach (Unit unit in allUnits)
            {
                SetMesh(unit, lightLayerUintWithHighlight);
            }
        }

        // Returns the correct material for a unit part type
        private Material GetUnitMaterial(UnitMaterialType unitMaterialType)
        {
            switch (unitMaterialType)
            {
                case UnitMaterialType.Spider:
                    return spiderMat;
                case UnitMaterialType.Vehicle:
                    return vehicleMat;
                case UnitMaterialType.Turret:
                    return turretMat;
                default:
                    throw null;
            }
        }

        // Coroutine for spawning unit with fade-in effect
        private IEnumerator SpawnUnit(Unit unit)
        {
            Renderer[] renderers = GetAllRenderers(unit.unitBodyTransform);
            Material[] spawnMaterials = new Material[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                spawnMaterials[i] = new Material(spawnMaterial);
                spawnMaterials[i].SetFloat("_SpawnProgress", 0f);
                renderers[i].material = spawnMaterials[i];
            }
            float elapsed = 0f;
            while (elapsed < gameDurations.unitSpawnDuration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / gameDurations.unitSpawnDuration);
                foreach (var mat in spawnMaterials)
                {
                    mat.SetFloat("_SpawnProgress", progress);
                }
                yield return null;
            }
            SetMesh(unit, lightLayerUintWithHighlight);
        }

        // Gets all mesh and skinned mesh renderers under a transform
        private Renderer[] GetAllRenderers(Transform parentTransform)
        {
            var allRenderers = new List<Renderer>();
            allRenderers.AddRange(parentTransform.GetComponentsInChildren<MeshRenderer>(true));
            allRenderers.AddRange(parentTransform.GetComponentsInChildren<SkinnedMeshRenderer>(true));
            return allRenderers.ToArray();
        }
    }
}

