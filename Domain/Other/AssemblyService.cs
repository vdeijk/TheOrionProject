using System;
using System.Collections.Generic;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Game;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.Domain
{
    [DefaultExecutionOrder(100)]
    // Handles part selection, assembly, and recycling for units
    public class AssemblyService
    {
        public UnitAssemblyData Data { get; private set; }

        private static AssemblyService _instance;

        public Dictionary<PartType, List<PartSO>> playerParts => PartsManager.Instance.playerParts;
        public static AssemblyService Instance => _instance ??= new AssemblyService();

        public static event EventHandler OnPartSelected;
        public static event EventHandler OnPartDeselected;

        public void Init(UnitAssemblyData Data)
        {
            Data = new UnitAssemblyData();
        }

        // Advances to the next part type
        public void NextPartType()
        {
            Data.CurIndex = (Data.CurIndex + 1) % Data.PartTypes.Length;

            SelectPart();
        }

        // Goes to the previous part type
        public void PreviousPartType()
        {
            Data.CurIndex = (Data.CurIndex - 1 + Data.PartTypes.Length) % Data.PartTypes.Length;

            SelectPart();
        }

        // Resets to the first part type
        public void Setup()
        {
            Data.CurIndex = 0;

            SelectPart();
        }

        // Selects a part and triggers highlight/preview/events
        public void SelectPart(PartSO partData)
        {
            Data.SelectedPart = partData;

            UnitSingleController unit = UnitSelectService.Instance.Data.SelectedUnit;
            PartHighlightService.Instance.Highlight(Data.CurPartType, unit, true);

            if (playerParts[Data.CurPartType].Count <= 0 || partData == null)
            {
                OnPartDeselected?.Invoke(this, EventArgs.Empty);
                return;
            }

            Data.Controller.InstantiatePart();

            OnPartSelected?.Invoke(this, EventArgs.Empty);
        }

        public void DestroyPartPreview()
        {
            Data.Controller.DestroyPartPreview();
        }

        // Recycles the selected part for scrap and updates UI
        public void Recycle()
        {
            float scrapGain = Data.SelectedPart.Cost / 2;
            ScrapManager.Instance.AddScrap(scrapGain);
            SFXMonobService.Instance.PlaySFX(SFXType.SelectUnit);
            HueShiftMonobService.Instance.Alter(true);

            DestroyPartPreview();
            PartHighlightService.Instance.Destroy();
            PartsManager.Instance.RemovePlayerPart(Data.SelectedPart, Data.CurPartType);

            SelectPart();
        }

        // Assembles the selected part onto the unit
        public void Assemble()
        {
            var unit = UnitSelectService.Instance.Data.SelectedUnit;

            var updatedPartsData = new Dictionary<PartType, PartSO>(unit.Data.PartsData);
            updatedPartsData[Data.CurPartType] = Data.SelectedPart;

            Data.Controller.DestroyUnitPreview(unit);

            var baseData = (BaseSO)updatedPartsData[PartType.Base];
            var torsoData = (TorsoSO)updatedPartsData[PartType.Torso];
            var weaponPrimary = (WeaponSO)updatedPartsData[PartType.WeaponPrimary];
            var weaponSecondary = (WeaponSO)updatedPartsData[PartType.WeaponSecondary];

            var transformData = UnitConstructService.Instance.GetTransformData(baseData, torsoData, weaponPrimary, weaponSecondary, unit);

            var context = new UnitPartsSettings(updatedPartsData, transformData);
            unit.ReplaceParts(context);

            UnitMaterialService.Instance.SetMesh(unit, UnitMaterialService.Instance.Data.LightLayerUintWithHighlight);

            SFXMonobService.Instance.PlaySFX(SFXType.SelectUnit);
            HueShiftMonobService.Instance.Alter(true);

            PartsManager.Instance.RemovePlayerPart(Data.SelectedPart, Data.CurPartType);

            SelectPart();
        }

        // Selects the first part in the current part type, or none if empty
        private void SelectPart()
        {
            if (playerParts[Data.CurPartType].Count > 0)
            {
                SelectPart(playerParts[Data.CurPartType][0]);
            }
            else
            {
                SelectPart(null);
            }
        }
    }
}