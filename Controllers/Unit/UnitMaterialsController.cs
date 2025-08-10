using System.Collections.Generic;
using UnityEngine;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(-100)]
    // Holds references to renderers for unit materials
    public class UnitMaterialsController : MonoBehaviour
    {
        [field: SerializeField] public Renderer[] propellerRenderers { get; private set; } // Propeller material renderers
        [field: SerializeField] public Renderer[] tracksRenderers { get; private set; }    // Tracks material renderers
    }
}