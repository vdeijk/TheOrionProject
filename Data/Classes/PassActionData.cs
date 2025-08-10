using System;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;

namespace TurnBasedStrategy
{
    [Serializable]
    public class PassActionData : BaseActionData
    {
        public HeatSystemController HeatSystem { get; set; }
        public Transform Prefab { get; set; }
        public Transform AudioPrefab { get; set; }
        public AudioClip HeatVentClip { get; set; }
    }
}