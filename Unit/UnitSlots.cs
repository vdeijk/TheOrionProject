using UnityEngine;

// Holds references to part slots for a unit
public class UnitSlots : MonoBehaviour
{
    [field: SerializeField] public Transform[] slots { get; private set; }
}
