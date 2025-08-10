using System;
using System.Collections;
using UnityEngine;
using TurnBasedStrategy.Data;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(-100)]
    /// <summary>
    /// Abstract base class for all unit actions (move, shoot, pass, etc.).
    /// </summary>
    public abstract class ActionBaseController : MonoBehaviour
    {
        public abstract BaseActionData Data { get; }

    }

    [DefaultExecutionOrder(-100)]
    public abstract class ActionBaseController<TData> : ActionBaseController where TData : BaseActionData
    {
        [field: SerializeField] public TData TypedData { get; set; }

        public override BaseActionData Data => TypedData;
    }
}
