using UnityEngine;

// Stores the shoulder height transform for a unit
public class UnitDimensions : MonoBehaviour
{
    public Transform shoulderHeightTransform { get; private set; }

    public void SetShoulderHeightTransform(Transform shoulderHeightTransform)
    {
        this.shoulderHeightTransform = shoulderHeightTransform;
    }
}
