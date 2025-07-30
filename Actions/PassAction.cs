using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy
{
    /// <summary>
    /// Handles the pass action, which dissipates heat and plays a venting effect.
    /// </summary>
    public class PassAction : BaseAction
    {
        [SerializeField] HeatSystem heatSystem;
        [SerializeField] Transform prefab;
        [SerializeField] Transform audioPrefab;
        [SerializeField] AudioClip heatVentClip;

        protected override void Awake()
        {
            base.Awake();
            actionName = "Pass";
        }

        private void OnDisable()
        {
            UnitCategoryService.OnUnitAdded -= UnitManager_OnUnitAdded;
        }

        private void Start()
        {
            UnitCategoryService.OnUnitAdded += UnitManager_OnUnitAdded;
        }

        /// <summary>
        /// Executes the pass action, dissipating heat and playing effects.
        /// </summary>
        public override void TakeAction(GridPosition gridPosition)
        {
            ActionStart();

            TorsoData torsoData = (TorsoData)unit.partsData[PartType.Torso];
            heatSystem.DissipateHeat(torsoData.MaxHeat / 2);

            audioPrefab.GetComponent<AudioSource>().clip = heatVentClip;
            Transform unitBodyTransform = unit.unitBodyTransform;

            prefab.localScale = new Vector3(6, 6, 6);
            Instantiate(prefab, unitBodyTransform.position, unitBodyTransform.rotation);
            Instantiate(audioPrefab, unitBodyTransform.position, unitBodyTransform.rotation);

            StartCoroutine(Delay());
        }

        /// <summary>
        /// Delays completion of the action to allow effects to play.
        /// </summary>
        private IEnumerator Delay()
        {
            yield return new WaitForSeconds(0.3f);

            ActionComplete();
        }

        /// <summary>
        /// Returns a low-value AI action for passing.
        /// </summary>
        protected override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
        {
            return new EnemyAIAction
            {
                gridPosition = gridPosition,
                actionValue = 1,
                baseAction = this,
            };
        }

        private void UnitManager_OnUnitAdded(object sender, UnitCategoryService.OnUnitAddedArgs e)
        {
            Range = 1;
            APCost = 1;
        }
    }
}
