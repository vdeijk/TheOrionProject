using System;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;

namespace TurnBasedStrategy
{
    [DefaultExecutionOrder(100)]
    public class PartHighlightService
    {
        public PartHighlightData Data { get; private set; }

        private static PartHighlightService _instance;

        public static PartHighlightService Instance => _instance ??= new PartHighlightService();

        public void Init(PartHighlightData data)
        {
            Data = data;
        }

        public void SetPartType(PartType partType)
        {
            Data.CurPartType = partType;
        }

        // Removes the currently highlighted part from the scene
        public void Destroy()
        {
            if (Data.HighlightedPart != null)
            {
                Data.Controller.DestroyHighlight();
                Data.HighlightedPart = null;
            }
        }

        // Highlights the selected part of a unit, applying color and material effects
        public void Highlight(PartType curSalvageType, UnitSingleController unit, bool isColorBlue)
        {
            Destroy();

            if (Data.LastHighlightedUnit != null && Data.LastHighlightedUnit != unit)
            {
                SetRenderers(Data.LastHighlightedUnit, true); // Restore previous unit's renderers
            }

            SetRenderers(unit, false); // Hide original part renderers

            Transform newPart = Data.Controller.Instantiate(unit);
            newPart.SetParent(SetParent(unit));
            newPart.localPosition = Vector3.zero;
            newPart.localRotation = Quaternion.identity;
            newPart.localScale = Vector3.one;

            uint lightLayerUint = UnitMaterialService.Instance.Data.LightLayerUintWithHighlight;
            Renderer[] newRenderers = newPart.GetComponentsInChildren<Renderer>();
            Material instanceMaterial = new Material(Data.HologramMaterial);
            instanceMaterial.SetColor("_RimColor", isColorBlue ? ColorData.Instance.colorBlue : ColorData.Instance.colorRed);
            UnitMaterialService.Instance.SetMaterials(newPart, instanceMaterial, lightLayerUint, newRenderers);

            Data.HighlightedPart = newPart;
            Data.LastHighlightedUnit = unit;
        }

        // Enables/disables renderers for the current part type
        public void SetRenderers(UnitSingleController unit, bool isCurTypeEnabled)
        {
            if (unit == null) return;
            foreach (var kvp in unit.Data.TransformData)
            {
                Renderer[] otherRenderers = kvp.Value.GetComponentsInChildren<Renderer>(true);
                foreach (Renderer renderer in otherRenderers)
                {
                    if (kvp.Key == Data.CurPartType)
                    {
                        renderer.enabled = isCurTypeEnabled;
                    }
                    else
                    {
                        renderer.enabled = true;
                    }
                }
            }
        }

        // Determines the parent transform for the highlighted part based on part type
        public Transform SetParent(UnitSingleController unit)
        {
            switch (Data.CurPartType)
            {
                case PartType.Base:
                    return unit.Data.UnitBodyTransform;
                case PartType.Torso:
                    UnitSlotsController baseSlots = unit.Data.TransformData[PartType.Base].GetComponent<UnitSlotsController>();
                    return baseSlots.slots[0];
                case PartType.WeaponPrimary:
                case PartType.WeaponSecondary:
                    UnitSlotsController torsoSlots = unit.Data.TransformData[PartType.Torso].GetComponent<UnitSlotsController>();
                    if (Data.CurPartType == PartType.WeaponPrimary)
                    {
                        return torsoSlots.slots[0];
                    }
                    else
                    {
                        return torsoSlots.slots[1];
                    }
            }

            return unit.Data.UnitBodyTransform;
        }
    }
}

