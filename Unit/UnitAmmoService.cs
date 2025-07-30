using UnityEngine;

namespace TurnBasedStrategy
{
    // Provides access to a unit's AmmoSystem by part type
    public class UnitAmmoService : Singleton<UnitAmmoService>
    {
        public AmmoSystem GetAmmoSystem(PartType partType)
        {
            Unit unit = UnitSelectService.Instance.selectedUnit;
            var ammoSystems = unit.unitMindTransform.GetComponents<AmmoSystem>();
            foreach (var ammoSystem in ammoSystems)
            {
                if (ammoSystem.partType == partType) return ammoSystem;
            }
            return null;
        }
    }
}
