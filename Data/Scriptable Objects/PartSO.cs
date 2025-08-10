using UnityEngine;

namespace TurnBasedStrategy.Data
{
    [CreateAssetMenu]
    public class PartSO : ScriptableObject
    {
        public string Name;
        public int Range;
        public int Cost;
        public string Description;
        public Transform Prefab;
        public string[] Effects;
        public UnitMaterialType unitMaterialType;
    }
}
