using UnityEngine;

namespace TurnBasedStrategy.Controllers
{
    // Holds references to part slots for a unit
    public class UnitSlotsController : MonoBehaviour
    {
        [field: SerializeField] public Transform[] slots { get; private set; }
    }
}