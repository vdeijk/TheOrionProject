using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy
{
    [DefaultExecutionOrder(100)]
    // Provides access to a unit's AmmoSystem by part type
    public class UnitAmmoService
    {
        private static UnitAmmoService _instance;
        public static UnitAmmoService Instance => _instance ??= new UnitAmmoService();

        private UnitAmmoService() { }

        public UnitAmmoController GetAmmoSystem(PartType partType)
        {
            UnitSingleController unit = UnitSelectService.Instance.Data.SelectedUnit;
            var ammoSystems = unit.Data.UnitMindTransform.GetComponents<UnitAmmoController>();
            foreach (var ammoSystem in ammoSystems)
            {
                if (ammoSystem.partType == partType) return ammoSystem;
            }
            return null;
        }
    }
}
