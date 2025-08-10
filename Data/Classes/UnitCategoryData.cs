using System;
using System.Collections.Generic;
using TurnBasedStrategy.Controllers;

namespace TurnBasedStrategy.Data
{
    [Serializable]
    public class UnitCategoryData
    {
        public List<UnitSingleController> All { get; set; } = new List<UnitSingleController>();
        public List<UnitSingleController> Allies { get; set; } = new List<UnitSingleController>();
        public List<UnitSingleController> Enemies { get; set; } = new List<UnitSingleController>();
    }
}