using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy
{
    // Provides repair functionality for unit parts
    public class UnitRepairService : MonoBehaviour
    {
        private List<Unit> allUnits = new List<Unit>();

        // Repairs the armor of a unit
        public void RepairArmor(Unit unit)
        {
            // Implementation needed
        }

        // Repairs the shield of a unit
        public void RepairShield(Unit unit)
        {
            // Implementation needed
        }

        // Repairs the health of a unit
        public void RepairHealth(Unit unit)
        {
            // Implementation needed
        }

        // Repairs all parts of a unit
        public void RepairAll(Unit unit)
        {
            RepairArmor(unit);
            RepairShield(unit);
            RepairHealth(unit);
        }
    }
}