using UnityEngine;
using TurnBasedStrategy.Controllers;

namespace TurnBasedStrategy.Data
{
    [System.Serializable]
    public class UnitAssemblyData
    {
        [field: SerializeField] public AssemblyController Controller { get; private set; }

        public readonly PartType[] PartTypes = new PartType[]
        {
            PartType.Base,
            PartType.Torso,
            PartType.WeaponPrimary,
            PartType.WeaponSecondary
        };
        public Transform CurPreviewTransform { get; set; }
        public PartSO SelectedPart { get; set; }
        public int CurIndex { get; set; } = 0;

        public PartType CurPartType => GetCurrentPartype();

        private PartType GetCurrentPartype()
        {
            CurIndex = Mathf.Clamp(CurIndex, 0, PartTypes.Length - 1);

            return PartTypes[CurIndex];
        }
    }
}