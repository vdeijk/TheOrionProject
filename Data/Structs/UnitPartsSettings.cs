using System.Collections.Generic;
using UnityEngine;
using TurnBasedStrategy.Data;

namespace TurnBasedStrategy
{
    // Context for unit parts and transforms
    public struct UnitPartsSettings
    {
        public Dictionary<PartType, PartSO> PartsData { get; }
        public Dictionary<PartType, Transform> TransformData { get; }

        public UnitPartsSettings(
            Dictionary<PartType, PartSO> partsData,
            Dictionary<PartType, Transform> transformData)
        {
            PartsData = partsData;
            TransformData = transformData;
        }
    }
}