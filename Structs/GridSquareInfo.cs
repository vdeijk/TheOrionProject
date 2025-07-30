namespace TurnBasedStrategy
{
    public struct GridSquareInfo
    {
        public GridPosition gridPosition;
        public float slope;
        public float height;
        public bool inaccessible;
        public float roughNoise;
        public float forestNoise;
        public GridSquareType gridSquareType;
    }
}