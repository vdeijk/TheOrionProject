using System.Collections.Generic;
using UnityEngine;
using TurnBasedStrategy.Controllers;

namespace TurnBasedStrategy.Data
{
    [System.Serializable]
    public class UnitSalvageData
    {
        [field: SerializeField] public SalvageController Controller { get; private set; }
        [field: SerializeField] public float BaseSalvageChance { get; private set; } = 20;

        public PartType[] SalvageTypes { get; } = new PartType[]
        {
            PartType.Base,
            PartType.Torso,
            PartType.WeaponPrimary,
            PartType.WeaponSecondary
        };
        public List<PartType> AvailableSalvageTypes { get; set; }
        public int CurIndex { get; set; } = 0;
        public int RemainingAttempts { get; set; } = 1;
        public Dictionary<PartType, int> InInventory { get; set; } = new Dictionary<PartType, int>();

        public Dictionary<PartType, PartSO> SalvageableParts;

        public PartType curSalvageType => GetCurrentSalvageType();


        // Returns a list of salvageable part types
        public List<PartType> GetAvailableSalvageTypes()
        {
            List<PartType> availableTypes = new List<PartType>();
            foreach (PartType type in SalvageTypes)
            {
                if (SalvageableParts.ContainsKey(type))
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
            CurIndex = Mathf.Clamp(CurIndex, 0, available.Count - 1);
            return available[CurIndex];
        }
    }
}