using System;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class HighlightService : Singleton<HighlightService>
    {
        [SerializeField] ColorPalette colorPalette;

        private Transform highlightedPart;
        private PartType curPartType = PartType.Base;
        private Unit lastHighlightedUnit;

        private void OnEnable()
        {
            MenuChangeService.OnMenuChanged += MenuChangeService_OnMenuChanged;
        }

        private void OnDisable()
        {
            MenuChangeService.OnMenuChanged -= MenuChangeService_OnMenuChanged;
        }

        // Removes the currently highlighted part from the scene
        public void RemoveHighlight()
        {
            if (highlightedPart != null)
            {
                Destroy(highlightedPart?.gameObject);
                highlightedPart = null;
            }
        }

        // Highlights the selected part of a unit, applying color and material effects
        public void HighlightCurrentPart(PartType curPartType, Unit unit, bool isColorBlue)
        {
            this.curPartType = curPartType;
            RemoveHighlight();
            if (lastHighlightedUnit != null && lastHighlightedUnit != unit)
            {
                SetRenderers(lastHighlightedUnit, true); // Restore previous unit's renderers
            }
            PartData partData = unit.partsData[curPartType];
            Transform newPart = Instantiate(partData.Prefab);
            SetRenderers(unit, false); // Hide original part renderers
            newPart.SetParent(SetParent(unit));
            newPart.localPosition = Vector3.zero;
            newPart.localRotation = Quaternion.identity;
            newPart.localScale = Vector3.one;
            uint lightLayerUint = UnitMaterialService.Instance.lightLayerUintWithHighlight;
            Renderer[] newRenderers = newPart.GetComponentsInChildren<Renderer>();
            Material instanceMaterial = new Material(UnitMaterialService.Instance.hologramMaterial);
            instanceMaterial.SetColor("_RimColor", isColorBlue ? colorPalette.colorBlue : colorPalette.colorRed);
            UnitMaterialService.Instance.SetMaterials(newPart, instanceMaterial, lightLayerUint, newRenderers);
            highlightedPart = newPart;
            lastHighlightedUnit = unit;
        }

        // Removes highlight and restores renderers when leaving relevant menus
        private void MenuChangeService_OnMenuChanged(object sender, EventArgs e)
        {
            MenuType prevMenu = MenuChangeService.Instance.prevMenu;
            if (prevMenu == MenuType.Assemble || prevMenu == MenuType.Salvage)
            {
                RemoveHighlight();
                Unit unit = UnitSelectService.Instance.selectedUnit;
                if (lastHighlightedUnit != null) SetRenderers(lastHighlightedUnit, true);
            }
        }

        // Enables/disables renderers for the current part type
        private void SetRenderers(Unit unit, bool isCurTypeEnabled)
        {
            if (unit == null) return;
            foreach (var kvp in unit.transformData)
            {
                Renderer[] otherRenderers = kvp.Value.GetComponentsInChildren<Renderer>(true);
                foreach (Renderer renderer in otherRenderers)
                {
                    if (kvp.Key == curPartType)
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
        private Transform SetParent(Unit unit)
        {
            switch (curPartType)
            {
                case PartType.Base:
                    return unit.unitBodyTransform;
                case PartType.Torso:
                    UnitSlots baseSlots = unit.transformData[PartType.Base].GetComponent<UnitSlots>();
                    return baseSlots.slots[0];
                case PartType.WeaponPrimary:
                case PartType.WeaponSecondary:
                    UnitSlots torsoSlots = unit.transformData[PartType.Torso].GetComponent<UnitSlots>();
                    if (curPartType == PartType.WeaponPrimary)
                    {
                        return torsoSlots.slots[0];
                    }
                    else
                    {
                        return torsoSlots.slots[1];
                    }
            }
            return unit.unitBodyTransform;
        }
    }
}

