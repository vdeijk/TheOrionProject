using System.Collections.Generic;
using UnityEngine;
using TurnBasedStrategy.Data;

namespace TurnBasedStrategy
{
    [System.Serializable]
    public class SpawnBaseData
    {
        [field: SerializeField] public int Max { get; private set; } = 20;
        [field: SerializeField] public Transform FactionTransform { get; private set; }
        [field: SerializeField] public int FactionSide { get; private set; }
        [field: SerializeField] public Transform Prefab { get; private set; }
        [field: SerializeField] public Material SpawnMaterial { get; private set; }
        [field: SerializeField] public DurationData DurationData { get; private set; }
        [field: SerializeField] public Transform[] SpawnPoints { get; set; }

        public List<MechsState> Mechs { get; set; }
    }
}
