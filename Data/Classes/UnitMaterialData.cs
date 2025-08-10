using UnityEngine;

namespace TurnBasedStrategy
{
    [System.Serializable]
    public class UnitMaterialData
    {
        public Material SpiderMat;
        public Material VehicleMat;
        public Material TurretMat;
        public Material TrackMaterial;
        public Material PropellerMaterial;

        public uint LightLayerUintWithHighlight { get; } = 3;
        public uint LightLayerUintWithoutHighlight { get; } = 1;
    }
}
