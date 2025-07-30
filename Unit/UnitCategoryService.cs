using System;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy
{
    // Manages lists of all units, allies, and enemies, and handles unit addition/removal
    public class UnitCategoryService : Singleton<UnitCategoryService>
    {
        public class OnUnitAddedArgs : EventArgs
        {
            public Unit unit;
        }

        public List<Unit> all { get; private set; } = new List<Unit>();
        public List<Unit> allies { get; private set; } = new List<Unit>();
        public List<Unit> enemies { get; private set; } = new List<Unit>();

        public static event EventHandler<OnUnitAddedArgs> OnUnitAdded;
        public static event EventHandler OnUnitRemoved;

        private void OnEnable()
        {
            // Listen for unit death events to update lists
            HealthSystem.OnunitDead += HealthSystem_OnUnitDead;
            HeatSystem.OnunitDead += HeatSystem_OnUnitDead;
        }

        private void OnDisable()
        {
            HealthSystem.OnunitDead -= HealthSystem_OnUnitDead;
            HeatSystem.OnunitDead -= HeatSystem_OnUnitDead;
        }

        // Returns a list of Mech objects for all allied units
        public List<Mech> GetPlayerMechs()
        {
            var mechs = new List<Mech>();
            foreach (var unit in allies)
            {
                var mech = new Mech
                {
                    mechData = new Dictionary<PartType, PartData>(unit.partsData)
                };
                mechs.Add(mech);
            }

            return mechs;
        }

        // Removes all allied units from the scene and clears lists
        public void RemoveAlliedUnits()
        {
            foreach (var unit in allies)
            {
                if (unit != null && unit.gameObject != null)
                {
                    Destroy(unit.gameObject);
                }
            }

            allies.Clear();

            all = new List<Unit>(enemies);
            allies = new List<Unit>();
        }

        // Adds a unit to the appropriate lists and triggers event
        public void AddUnit(Unit unit)
        {
            UnitFaction unitFaction = unit.unitEntityTransform.GetComponent<UnitFaction>();

            all.Add(unit);
            if (unitFaction.IS_ENEMY) enemies.Add(unit);
            else allies.Add(unit);

            OnUnitAdded?.Invoke(this, new OnUnitAddedArgs
            {
                unit = unit
            });
        }

        // Removes a unit from the appropriate lists and triggers event
        public void RemoveUnit(Unit unit)
        {
            UnitFaction unitFaction = unit.unitEntityTransform.GetComponent<UnitFaction>();

            all.Remove(unit);
            if (unitFaction.IS_ENEMY) enemies.Remove(unit);
            else allies.Remove(unit);

            OnUnitRemoved?.Invoke(this, EventArgs.Empty);
        }

        private void HealthSystem_OnUnitDead(object sender, HealthSystem.OnUnitDeadEventArgs e)
        {
            RemoveUnit(e.unit);
        }

        private void HeatSystem_OnUnitDead(object sender, HeatSystem.OnUnitDeadEventArgs e)
        {
            RemoveUnit(e.unit);
        }
    }
}
