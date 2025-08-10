using UnityEngine;
using TurnBasedStrategy.Data;

namespace TurnBasedStrategy.Domain
{
    [DefaultExecutionOrder(100)]
    // Provides mouse position in world space using raycast
    public class MouseWorldService
    {
        private static MouseWorldService _instance;
        private RaycastHit hit;

        public static MouseWorldService Instance => _instance ??= new MouseWorldService();
        public GridData Data => GridCoordinatorService.Instance.Data;

        // Returns the world position under the mouse cursor
        public Vector3 GetPosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, Data.TerrainLayer))
            {
                return raycastHit.point;
            }

            return Vector3.zero;
        }
    }
}
