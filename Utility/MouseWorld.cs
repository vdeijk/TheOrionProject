using UnityEngine;

namespace TurnBasedStrategy
{
    // Provides mouse position in world space using raycast
    public class MouseWorld : MonoBehaviour
    {
        [SerializeField] LayerMask mousePlaneLayermask;
        private static MouseWorld instance;
        private RaycastHit hit;

        private void Awake()
        {
            instance = this;
        }

        // Returns the world position under the mouse cursor
        public static Vector3 GetPosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.mousePlaneLayermask);
            return raycastHit.point;
        }
    }
}
