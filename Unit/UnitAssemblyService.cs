using System;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy
{
    // Handles part selection, assembly, and recycling for units
    public class UnitAssemblyService : Singleton<UnitAssemblyService>
    {
        // Context for unit parts and transforms
        public class UnitPartsContext
        {
            public Dictionary<PartType, PartData> PartsData { get; }
            public Dictionary<PartType, Transform> TransformData { get; }
            public Transform CamTransform { get; }

            public UnitPartsContext(
                Dictionary<PartType, PartData> partsData,
                Dictionary<PartType, Transform> transformData)
            {
                PartsData = partsData;
                TransformData = transformData;
            }
        }

        public PartData selectedPart { get; private set; }

        private int curIndex = 0;
        private PartType[] partTypes = new PartType[]
        {
            PartType.Base,
            PartType.Torso,
            PartType.WeaponPrimary,
            PartType.WeaponSecondary
        };

        public Dictionary<PartType, List<PartData>> playerParts => PartsManager.Instance.playerParts;
        public PartType curPartType => GetCurrentPartype();

        public static event EventHandler OnPartSelected;
        public static event EventHandler OnPartDeselected;

        // Advances to the next part type
        public void NextPartType()
        {
            curIndex = (curIndex + 1) % partTypes.Length;

            SelectPart();
        }

        // Goes to the previous part type
        public void PreviousPartType()
        {
            curIndex = (curIndex - 1 + partTypes.Length) % partTypes.Length;

            SelectPart();
        }

        // Resets to the first part type
        public void Setup()
        {
            curIndex = 0;

            SelectPart();
        }

        // Selects a part and triggers highlight/preview/events
        public void SelectPart(PartData partData)
        {
            selectedPart = partData;

            Unit unit = UnitSelectService.Instance.selectedUnit;
            HighlightService.Instance.HighlightCurrentPart(curPartType, unit, true);

            if (playerParts[curPartType].Count <= 0 || partData == null)
            {
                OnPartDeselected?.Invoke(this, EventArgs.Empty);
                return;
            }

            PartPreviewService.Instance.SelectPart();

            OnPartSelected?.Invoke(this, EventArgs.Empty);
        }

        // Recycles the selected part for scrap and updates UI
        public void Recycle()
        {
            float scrapGain = selectedPart.Cost / 2;
            ScrapManager.Instance.AddScrap(scrapGain);
            SFXController.Instance.PlaySFX(SFXType.SelectUnit);
            HueShiftController.Instance.Alter(true);

            PartPreviewService.Instance.RemovePreview();
            HighlightService.Instance.RemoveHighlight();
            PartsManager.Instance.RemovePlayerPart(selectedPart, curPartType);

            SelectPart();
        }

        // Assembles the selected part onto the unit
        public void Assemble()
        {
            var unit = UnitSelectService.Instance.selectedUnit;

            var updatedPartsData = new Dictionary<PartType, PartData>(unit.partsData);
            updatedPartsData[curPartType] = selectedPart;

            foreach (Transform child in unit.transformData[PartType.Base])
            {
                Destroy(child.gameObject);
            }
            Destroy(unit.transformData[PartType.Base].gameObject);

            var baseData = (BaseData)updatedPartsData[PartType.Base];
            var torsoData = (TorsoData)updatedPartsData[PartType.Torso];
            var weaponPrimary = (WeaponData)updatedPartsData[PartType.WeaponPrimary];
            var weaponSecondary = (WeaponData)updatedPartsData[PartType.WeaponSecondary];

            var transformData = UnitConstructService.Instance.GetTransformData(baseData, torsoData, weaponPrimary, weaponSecondary, unit);

            var context = new UnitPartsContext(updatedPartsData, transformData);
            unit.ReplaceParts(context);

            UnitMaterialService.Instance.SetMesh(unit, UnitMaterialService.Instance.lightLayerUintWithHighlight);

            SFXController.Instance.PlaySFX(SFXType.SelectUnit);
            HueShiftController.Instance.Alter(true);

            PartsManager.Instance.RemovePlayerPart(selectedPart, curPartType);

            SelectPart();
        }

        // Selects the first part in the current part type, or none if empty
        private void SelectPart()
        {
            if (playerParts[curPartType].Count > 0)
            {
                SelectPart(playerParts[curPartType][0]);
            }
            else
            {
                SelectPart(null);
            }
        }

        // Returns the current part type
        private PartType GetCurrentPartype()
        {
            curIndex = Mathf.Clamp(curIndex, 0, partTypes.Length - 1);

            return partTypes[curIndex];
        }
    }
}