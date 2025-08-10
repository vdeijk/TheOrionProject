using System;
using UnityEngine;

namespace TurnBasedStrategy.Data
{
    [Serializable]
    public struct GridMaterialType
    {
        public GridVisualType gridVisualType;
        public Material material;
    }
}