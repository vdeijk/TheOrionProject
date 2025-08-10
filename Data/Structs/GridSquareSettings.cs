using UnityEngine;

namespace TurnBasedStrategy.Data
{
    public struct GridSquareSettings
    {
        public Vector2Int gridPosition;
        public float slope;
        public float height;
        public bool inaccessible;
        public float roughNoise;
        public float forestNoise;
        public GridSquareType gridSquareType;
    }
}