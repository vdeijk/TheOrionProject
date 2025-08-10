using System;
using TurnBasedStrategy.Controllers;
using UnityEngine;

namespace TurnBasedStrategy.Data
{
    [Serializable]
    public class MoveActionData : BaseActionData
    {
        [field: SerializeField] public float StoppingDistance { get; private set; }
        [field: SerializeField] public LayerMask TerrainLayer { get; private set; }
        [field: SerializeField] public HeatSystemController HeatSystem { get; private set; }

        public Vector3 TargetPosition { get; set; }
        public Vector3 CurPos { get; set; }
    }
}