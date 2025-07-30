using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy
{
    // Handles the salvage and recycling of unit parts after defeat
    public class UnitSalvageService : Singleton<UnitSalvageService>
    {
        public int remainingAttempts { get; private set; } = 1;
        public Dictionary<PartType, int> inInventory { get; private set; } = new Dictionary<PartType, int>();
        public Dictionary<PartType, PartData> obtainedParts { get; private set; }
        public Dictionary<PartType, PartData> recycledParts { get; private set; }
        public Dictionary<PartType, PartData> salvageableParts;

        [SerializeField] GameDurations gameDurations;
        [SerializeField] float baseSalvageChance = 20;

        private PartType[] salvageTypes = new PartType[]
        {
            PartType.Base,
            PartType.Torso,
            PartType.WeaponPrimary,
            PartType.WeaponSecondary
        };
        private List<PartType> availableSalvageTypes;
        private int curIndex = 0;

        public PartType curSalvageType => GetCurrentSalvageType();
        public static event EventHandler OnCurSalvageTypeChanged;

        // Initializes salvage state for the selected unit
        public void Setup()
        {
            Unit unit = UnitSelectService.Instance.selectedTarget;
            remainingAttempts = 3;
            curIndex = 0;
            salvageableParts = new Dictionary<PartType, PartData>(unit.partsData);
            obtainedParts = new Dictionary<PartType, PartData>();
            recycledParts = new Dictionary<PartType, PartData>();
            inInventory = CalculateAllInInventory();
            ChangePart();
        }

        // Recycles the current part for scrap
        public void Recycle()
        {
            if (obtainedParts.ContainsKey(curSalvageType) || recycledParts.ContainsKey(curSalvageType))
            {
                SFXController.Instance.PlaySFX(SFXType.Failure);
                return;
            }
            recycledParts.Add(curSalvageType, salvageableParts[curSalvageType]);
            inInventory = CalculateAllInInventory();
            float scrapGain = salvageableParts[curSalvageType].Cost / 2;
            ScrapManager.Instance.AddScrap(scrapGain);
            ChangePart();
            HueShiftController.Instance.Alter(true);
            SFXController.Instance.PlaySFX(SFXType.Success);
            UpdateRenaminingAttempts();
        }

        // Attempts to salvage the current part
        public void Salvage()
        {
            if (obtainedParts.ContainsKey(curSalvageType) || recycledParts.ContainsKey(curSalvageType))
            {
                SFXController.Instance.PlaySFX(SFXType.Failure);
                return;
            }
            Unit unit = UnitSelectService.Instance.selectedUnit;
            TorsoData torsoData = unit.partsData[PartType.Torso] as TorsoData;
            if (UnityEngine.Random.value * 100f <= torsoData.SalvageBoost + baseSalvageChance)
            {
                obtainedParts.Add(curSalvageType, salvageableParts[curSalvageType]);
                inInventory = CalculateAllInInventory();
                ChangePart();
                HueShiftController.Instance.Alter(true);
                SFXController.Instance.PlaySFX(SFXType.Success);
            }
            else
            {
                HueShiftController.Instance.Alter(false);
                SFXController.Instance.PlaySFX(SFXType.Failure);
            }
            UpdateRenaminingAttempts();
        }

        // Moves to the next available salvage part
        public void NextPart()
        {
            availableSalvageTypes = GetAvailableSalvageTypes();
            curIndex = (curIndex + 1) % availableSalvageTypes.Count;
            ChangePart();
        }

        // Moves to the previous available salvage part
        public void PreviousPart()
        {
            availableSalvageTypes = GetAvailableSalvageTypes();
            curIndex = (curIndex - 1 + availableSalvageTypes.Count) % availableSalvageTypes.Count;
            ChangePart();
        }

        // Ends the salvage phase and applies obtained parts
        public IEnumerator End()
        {
            yield return new WaitForSecondsRealtime(gameDurations.explosionDuration);
            TimeScaleManager.Instance.SetTimeScaleNormal();
            Unit unit = UnitSelectService.Instance.selectedTarget;
            unit.unitMindTransform.GetComponent<HealthSystem>().Die();
        }

        // Updates highlight and camera for the current salvage part
        private void ChangePart()
        {
            bool hasBeenObtained = obtainedParts.ContainsKey(curSalvageType);
            bool hasBeenRecycled = recycledParts.ContainsKey(curSalvageType);
            Unit unit = UnitSelectService.Instance.selectedTarget;
            HighlightService.Instance.HighlightCurrentPart(curSalvageType, unit, !hasBeenObtained && !hasBeenRecycled);
            SalvageCamera.Instance.SetCameraTarget(curSalvageType);
            OnCurSalvageTypeChanged?.Invoke(this, EventArgs.Empty);
        }

        // Calculates how many of each part type are in inventory
        private Dictionary<PartType, int> CalculateAllInInventory()
        {
            var result = new Dictionary<PartType, int>();
            foreach (PartType partType in salvageTypes)
            {
                int count = 0;
                if (salvageableParts.TryGetValue(partType, out var partData))
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

        // Updates remaining salvage attempts and applies obtained parts if finished
        private void UpdateRenaminingAttempts()
        {
            remainingAttempts--;
            if (remainingAttempts <= 0)
            {
                foreach (var kvp in obtainedParts)
                {
                    PartsManager.Instance.AddPlayerPart(kvp.Value, kvp.Key);
                }
                StopAllCoroutines();
                StartCoroutine(End());
            }
        }

        // Returns a list of salvageable part types
        private List<PartType> GetAvailableSalvageTypes()
        {
            List<PartType> availableTypes = new List<PartType>();
            foreach (PartType type in salvageTypes)
            {
                if (salvageableParts.ContainsKey(type))
                {
                    availableTypes.Add(type);
                }
            }
            return availableTypes;
        }

        // Returns the current salvage part type
        private PartType GetCurrentSalvageType()
        {
            List<PartType> available = GetAvailableSalvageTypes();
            curIndex = Mathf.Clamp(curIndex, 0, available.Count - 1);
            return available[curIndex];
        }
    }
}
