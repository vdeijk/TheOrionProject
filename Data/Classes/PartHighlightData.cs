using UnityEngine;
using TurnBasedStrategy.Controllers;

namespace TurnBasedStrategy.Data
{
    [System.Serializable]
    public class PartHighlightData
    {
        public PartType CurPartType { get; set; } = PartType.Base;
        public Transform HighlightedPart { get; set; }
        public UnitSingleController LastHighlightedUnit { get; set; }

        public Material HologramMaterial;
        public PartHighlightController Controller;
    }
}