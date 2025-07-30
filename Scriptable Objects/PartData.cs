using UnityEngine;

namespace TurnBasedStrategy
{
    [CreateAssetMenu]
    public class PartData : ScriptableObject
    {
        public string Name;
        public int Cost;
        public string Description;
        public Transform Prefab;
        public string[] Effects;
        public UnitMaterialType unitMaterialType;
    }
}
