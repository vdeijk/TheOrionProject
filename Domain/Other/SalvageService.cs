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
    // Handles the salvage and recycling of unit parts after defeat
    public class SalvageService
    {
        private Dictionary<PartType, PartSO> obtainedParts;
        private Dictionary<PartType, PartSO> recycledParts;
        private static SalvageService _instance;

        public static SalvageService Instance => _instance ??= new SalvageService();
        public UnitSalvageData Data { get; private set; }

        public static event EventHandler OnCurSalvageTypeChanged;

        public void Init(UnitSalvageData data)
        {
            Data = data;
        }

        // Initializes salvage state for the selected unit
        public void Setup()
        {
            UnitSingleController unit = UnitSelectService.Instance.Data.SelectedTarget;
            Data.RemainingAttempts = 3;
            Data.CurIndex = 0;
            Data.SalvageableParts = new Dictionary<PartType, PartSO>(unit.Data.PartsData);
            obtainedParts = new Dictionary<PartType, PartSO>();
            recycledParts = new Dictionary<PartType, PartSO>();
            Data.InInventory = CalculateAllInInventory();
            ChangePart();
        }

        // Recycles the current part for scrap
        public void Recycle()
        {
            if (obtainedParts.ContainsKey(Data.curSalvageType) || recycledParts.ContainsKey(Data.curSalvageType))
            {
                SFXMonobService.Instance.PlaySFX(SFXType.Failure);
                return;
            }
            recycledParts.Add(Data.curSalvageType, Data.SalvageableParts[Data.curSalvageType]);
            Data.InInventory = CalculateAllInInventory();
            float scrapGain = Data.SalvageableParts[Data.curSalvageType].Cost / 2;
            ScrapManager.Instance.AddScrap(scrapGain);
            ChangePart();
            HueShiftMonobService.Instance.Alter(true);
            SFXMonobService.Instance.PlaySFX(SFXType.Success);
            UpdateRenaminingAttempts();
        }

        // Attempts to salvage the current part
        public void Salvage()
        {
            if (obtainedParts.ContainsKey(Data.curSalvageType) || recycledParts.ContainsKey(Data.curSalvageType))
            {
                SFXMonobService.Instance.PlaySFX(SFXType.Failure);
                return;
            }
            UnitSingleController unit = UnitSelectService.Instance.Data.SelectedUnit;
            TorsoSO torsoData = unit.Data.PartsData[PartType.Torso] as TorsoSO;
            if (UnityEngine.Random.value * 100f <= torsoData.SalvageBoost + Data.BaseSalvageChance)
            {
                obtainedParts.Add(Data.curSalvageType, Data.SalvageableParts[Data.curSalvageType]);
                Data.InInventory = CalculateAllInInventory();
                ChangePart();
                HueShiftMonobService.Instance.Alter(true);
                SFXMonobService.Instance.PlaySFX(SFXType.Success);
            }
            else
            {
                HueShiftMonobService.Instance.Alter(false);
                SFXMonobService.Instance.PlaySFX(SFXType.Failure);
            }

            UpdateRenaminingAttempts();
        }

        // Moves to the next available salvage part
        public void NextPart()
        {
            Data.AvailableSalvageTypes = Data.GetAvailableSalvageTypes();
            Data.CurIndex = (Data.CurIndex + 1) % Data.AvailableSalvageTypes.Count;
            ChangePart();
        }

        // Moves to the previous available salvage part
        public void PreviousPart()
        {
            Data.AvailableSalvageTypes = Data.GetAvailableSalvageTypes();
            Data.CurIndex = (Data.CurIndex - 1 + Data.AvailableSalvageTypes.Count) % Data.AvailableSalvageTypes.Count;
            ChangePart();
        }


        // Updates highlight and camera for the current salvage part
        private void ChangePart()
        {
            bool hasBeenObtained = obtainedParts.ContainsKey(Data.curSalvageType);
            bool hasBeenRecycled = recycledParts.ContainsKey(Data.curSalvageType);
            UnitSingleController unit = UnitSelectService.Instance.Data.SelectedTarget;
            PartHighlightService.Instance.Highlight(Data.curSalvageType, unit, !hasBeenObtained && !hasBeenRecycled);
            CamerasSalvageService.Instance.SetTarget(Data.curSalvageType);
            OnCurSalvageTypeChanged?.Invoke(this, EventArgs.Empty);
        }

        public void UpdateRenaminingAttempts()
        {
            Data.RemainingAttempts--;

            if (Data.RemainingAttempts <= 0)
            {
                foreach (var kvp in obtainedParts)
                {
                    PartsManager.Instance.AddPlayerPart(kvp.Value, kvp.Key);
                }

                Data.Controller.HandleCoroutines();
            }
        }

        // Calculates how many of each part type are in inventory
        private Dictionary<PartType, int> CalculateAllInInventory()
        {
            var result = new Dictionary<PartType, int>();
            foreach (PartType partType in Data.SalvageTypes)
            {
                int count = 0;
                if (Data.SalvageableParts.TryGetValue(partType, out var partData))
                {
                    if (PartsManager.Instance.playerParts.TryGetValue(partType, out var playerPartList))
                    {
                        foreach (var ownedPart in playerPartList)
                        {
                            if (ownedPart == partData)
                                count++;
                        }
                    }
                    if (obtainedParts.TryGetValue(partType, out var obtainedPart))
                    {
                        if (obtainedPart == partData)
                            count++;
                    }
                }
                result[partType] = count;
            }
            return result;
        }
    }
}
