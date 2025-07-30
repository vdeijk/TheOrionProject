using UnityEngine;

namespace TurnBasedStrategy
{
    // Rotates a specified transform around the Y axis
    public class ModelRotation : MonoBehaviour
    {
        [SerializeField] Transform transformToRotate;
        [SerializeField] float rotationSpeed = 20f;

        // Rotates the transform every frame
        private void Update()
        {
            transformToRotate.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }
}