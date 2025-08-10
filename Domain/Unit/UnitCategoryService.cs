using System;
using System.Collections.Generic;
using UnityEngine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.Domain
{
    [DefaultExecutionOrder(100)]
    // Manages lists of all units, allies, and enemies, and handles unit addition/removal
    public class UnitCategoryService
    {
        public class OnUnitAddedArgs : EventArgs
        {
            public UnitSingleController unit;
        }
        
        public UnitCategoryData Data { get; private set; }

        private static UnitCategoryService _instance;

        public static UnitCategoryService Instance => _instance ??= new UnitCategoryService();

        public static event EventHandler<OnUnitAddedArgs> OnUnitAdded;
        public static event EventHandler OnUnitRemoved;

        public void Init(UnitCategoryData data)
        {
            Data = data;
        }

        // Returns a list of Mech objects for all allied units
        public List<MechsState> GetPlayerUnitsData()
        {
            var mechs = new List<MechsState>();
            foreach (var unit in Data.Allies)
            {
                var mech = new MechsState
                {
                    mechData = new Dictionary<PartType, PartSO>(unit.Data.PartsData)
                };
                mechs.Add(mech);
            }

            return mechs;
        }

        // Removes all allied units from the scene and clears lists
        public void RemoveAlliedUnits()
        {
            foreach (var unit in Data.Allies)
            {
                if (unit != null)
                {
                    UnitMonobService.Instance.DestroyUnit(unit);
                }
            }

            Data.Allies.Clear();

            Data.All = new List<UnitSingleController>(Data.Enemies);
            Data.Allies = new List<UnitSingleController>();
        }

        // Adds a unit to the appropriate lists and triggers event
        public void AddUnit(UnitSingleController unit)
        {
            UnitFactionController unitFaction = unit.Data.UnitEntityTransform.GetComponent<UnitFactionController>();

            Data.All.Add(unit);
            if (unitFaction.IS_ENEMY) Data.Enemies.Add(unit);
            else Data.Allies.Add(unit);

            OnUnitAdded?.Invoke(this, new OnUnitAddedArgs
            {
                unit = unit
            });
        }

        // Removes a unit from the appropriate lists and triggers event
        public void RemoveUnit(UnitSingleController unit)
        {
            UnitFactionController unitFaction = unit.Data.UnitEntityTransform.GetComponent<UnitFactionController>();

            Data.All.Remove(unit);
            if (unitFaction.IS_ENEMY) Data.Enemies.Remove(unit);
            else Data.Allies.Remove(unit);

            OnUnitRemoved?.Invoke(this, EventArgs.Empty);
        }
    }
}
