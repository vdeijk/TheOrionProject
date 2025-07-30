using System.Collections;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class GridPlacement : MonoBehaviour
    {
        [SerializeField] Transform transformToPlace;
        [SerializeField] LayerMask terrainLayer;

        [SerializeField] Rigidbody rb;

        private void OnEnable()
        {
            UnitSpawnService.OnUnitsTeleported += UnitSpawnService_OnUnitsTeleported;
        }

        private void OnDisable()
        {
            UnitSpawnService.OnUnitsTeleported -= UnitSpawnService_OnUnitsTeleported;
        }

        private void Start()
        {
            PlaceGameObjectOnGrid();
        }

        // Repositions the object when units are teleported
        private void UnitSpawnService_OnUnitsTeleported(object sender, System.EventArgs e)
        {
            PlaceGameObjectOnGrid();
        }

        // Places the object at the correct grid and terrain position
        private void PlaceGameObjectOnGrid()
        {
            if (rb != null)
            {
                rb.gameObject.SetActive(false); // Temporarily disable physics
            }

            GridPosition unitGridPosition = LevelGrid.Instance.GetGridPosition(transformToPlace.position);
            Vector3 newPos = LevelGrid.Instance.GetWorldPosition(unitGridPosition);

            transformToPlace.position = new Vector3(newPos.x, GetTerrainYRaycast(newPos), newPos.z);

            if (rb != null)
            {
                rb.gameObject.SetActive(true); // Re-enable physics
            }
        }

        // Uses a downward raycast to find the terrain height at the given position
        private float GetTerrainYRaycast(Vector3 nextPos)
        {
            if (Physics.Raycast(new Vector3(nextPos.x, nextPos.y + 100, nextPos.z), Vector3.down, out RaycastHit hit, float.MaxValue, terrainLayer))
            {
                return hit.point.y;
            }
            return nextPos.y;
        }
    }
}