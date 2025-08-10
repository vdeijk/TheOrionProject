using System.Collections.Generic;
using UnityEngine;
using TurnBasedStrategy.Game;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy
{
    public class SpawnAllyService : SpawnBaseService
    {
        public SpawnAllyData Data { get; private set; }

        private static SpawnAllyService _instance;

        public static SpawnAllyService Instance => _instance ??= new SpawnAllyService();

        public void Init(SpawnAllyData data)
        {
            Data = data;
        }

        // Initializes player mechs with starter parts
        public List<MechsState> InitMechs()
        {
            Data.Mechs = new List<MechsState>();

            for (int i = 0; i < Data.Max; i++)
            {
                var mechData = new Dictionary<PartType, PartSO>
            {
                { PartType.Base, PartsManager.Instance._gameController.starterBase },
                { PartType.Torso, PartsManager.Instance._gameController.starterTorso },
                { PartType.WeaponPrimary, PartsManager.Instance._gameController.starterWeapon },
                { PartType.WeaponSecondary, PartsManager.Instance._gameController.starterWeapon  }
            };

                Data.Mechs.Add(new MechsState { mechData = mechData });
            }

            return Data.Mechs;
        }

        public void SetMechs(List<MechsState> mechs)
        {
            Data.Mechs = mechs;
        }

        // Refills missing allies and spawns them
        public void Refill()
        {
            Data.Mechs.Clear();
            int currentCount = UnitCategoryService.Instance.Data.Allies.Count;
            int count = Data.Max - currentCount;

            for (int i = 0; i < count; i++)
            {
                var mechData = new Dictionary<PartType, PartSO>
                {
                    { PartType.Base, PartsManager.Instance._gameController.starterBase },
                    { PartType.Torso, PartsManager.Instance._gameController.starterTorso },
                    { PartType.WeaponPrimary, PartsManager.Instance._gameController.starterWeapon },
                    { PartType.WeaponSecondary, PartsManager.Instance._gameController.starterWeapon }
                };
                Data.Mechs.Add(new MechsState { mechData = mechData });
            }

            if (count > 0)
            {
                int startIndex = Data.Mechs.Count - count;
                var newMechs = Data.Mechs.GetRange(startIndex, count);

                Data.Mechs = newMechs;
                SpawnUnits(Data);
            }
        }

        // Spawns all allied units
        public void SpawnAllies()
        {
            int NumberOfAllies = Data.Mechs.Count;

            SpawnUnits(Data);
        }
    }
}
