using UnityEngine;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Controllers;

namespace TurnBasedStrategy.Domain
{
    [DefaultExecutionOrder(100)]
    /// <summary>
    /// Handles the pass action, which dissipates heat and plays a venting effect.
    /// </summary>
    public class ActionPassService : BaseActionService<PassActionData>
    {
        private static ActionPassService _instance;

        public static ActionPassService Instance => _instance ??= new ActionPassService();

        public override PassActionData Data
        {
            get
            {
                var selectedUnit = UnitSelectService.Instance.Data.SelectedUnit;
                if (selectedUnit == null) return null;
                var action = selectedUnit.Data.UnitEntityTransform.GetComponent<ActionPassController>();
                return action?.TypedData;
            }
        }

        /// <summary>
        /// Executes the pass action, dissipating heat and playing effects.
        /// </summary>
        public override void TakeAction(Vector2Int gridPosition)
        {
            TorsoSO torsoData = (TorsoSO)unit.Data.PartsData[PartType.Torso];
            Data.HeatSystem.DissipateHeat(torsoData.MaxHeat / 2);

            Data.AudioPrefab.GetComponent<AudioSource>().clip = Data.HeatVentClip;
            Transform unitBodyTransform = unit.Data.UnitBodyTransform;

            Data.Prefab.localScale = new Vector3(6, 6, 6);

            var passAction = (ActionPassController)Data.BaseAction;
            passAction.InstantiatePrefab(Data.Prefab, unitBodyTransform.position, unitBodyTransform.rotation);
            passAction.InstantiatePrefab(Data.AudioPrefab, unitBodyTransform.position, unitBodyTransform.rotation);

            passAction.StopAllCoroutines();
            passAction.StartCoroutine(passAction.Delay());
        }

        /// <summary>
        /// Returns a low-value AI action for passing.
        /// </summary>
        protected override UnitAIData GetEnemyAction(Vector2Int gridPosition)
        {
            return new UnitAIData
            {
                GridPosition = gridPosition,
                ActionValue = 1,
                BaseActionData = Data,
            };
        }
    }
}
