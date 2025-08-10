using UnityEngine;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(-100)]
    // Rotates a specified transform around the Y axis
    public class UtilityRotationController : MonoBehaviour
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