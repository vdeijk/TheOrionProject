using System;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy.Data
{
    public class GameData
    {
        public float FastScale { get; private set; } = 1;
        public float NormalScale { get; private set; } = 2;
        public float MaxScrap { get; private set; } = 99999;

    }
}