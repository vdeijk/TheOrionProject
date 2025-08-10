using System;
using System.Collections;
using UnityEngine;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(-100)]
    [Serializable]
    /// <summary>
    /// Handles the pass action, which dissipates heat and plays a venting effect.
    /// </summary>
    public class ActionPassController : ActionBaseController<PassActionData>
    {
        public void InstantiatePrefab(Transform prefab, Vector3 pos, Quaternion rot)
        {
            Instantiate(prefab, pos, rot);
        }

        /// <summary>
        /// Delays completion of the action to allow effects to play.
        /// </summary>
        public IEnumerator Delay()
        {
            yield return new WaitForSeconds(0.3f);

            ActionPassService.Instance.CompleteAction();
        }
    }
}
