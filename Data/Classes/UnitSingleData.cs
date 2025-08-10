using System;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy.Data
{
    [Serializable]
    public class UnitSingleData
    {
        [field: SerializeField] public Transform UnitEntityTransform { get; private set; }
        [field: SerializeField] public Transform UnitMindTransform { get; private set; }
        [field: SerializeField] public Transform UnitBodyTransform { get; private set; }

        public Dictionary<PartType, PartSO> PartsData { get; set; }
        public Dictionary<PartType, Transform> TransformData { get; set; }
    }
}