using UnityEngine;

namespace TurnBasedStrategy
{
    // Handles movement and positioning logic for units
    public class UnitMovementService : Singleton<UnitMovementService>
    {
        [SerializeField] float hoverHeightOffset = 2f;
        [SerializeField] float airHeightOffset = 4f;
        [SerializeField] float heightSmoothTime = 0.2f;
        [SerializeField] LayerMask terrainLayer;

        // Initializes the position of a unit's Rigidbody based on terrain
        public void InitPos(Rigidbody rb, Unit unit)
        {
            rb.gameObject.SetActive(false);

            GridPosition unitGridPosition = LevelGrid.Instance.GetGridPosition(rb.position);
            Vector3 newPos = LevelGrid.Instance.GetWorldPosition(unitGridPosition);

            Vector3 raycastPos = new Vector3(newPos.x, 0f, newPos.z);
            float terrainHeight = GetTerrainYRaycast(raycastPos);
            float currentHeight = terrainHeight + GetHeightOffset(unit);

            rb.transform.position = new Vector3(newPos.x, currentHeight, newPos.z);

            rb.gameObject.SetActive(true);
        }

        // Updates unit movement based on speed and terrain
        public void UpdateMovement(UnitMovement unitMovement)
        {
            Vector3 currentPos = unitMovement.rb.transform.position;
            float currentHeight = GetTerrainYRaycast(currentPos) + GetHeightOffset(unitMovement.unit);
            Rigidbody rb = unitMovement.rb;
            float MoveSpeed = unitMovement.MoveSpeed;
            Vector3 nextPos = rb.transform.position + rb.transform.forward * MoveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(new Vector3(nextPos.x, currentHeight, nextPos.z));
        }

        // Updates unit rotation and maintains correct height
        public void UpdateRotation(UnitMovement unitMovement, Vector3 curFwd, Vector3 targetDirection)
        {
            Rigidbody rb = unitMovement.rb;
            float rotateSpeed = unitMovement.RotateSpeed;

            Quaternion currentRotation = rb.rotation;

            Vector3 flatDirection = new Vector3(targetDirection.x, 0, targetDirection.z).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(flatDirection);

            Quaternion newRotation = Quaternion.Slerp(
                currentRotation,
                targetRotation,
                rotateSpeed * Time.fixedDeltaTime
            );

            rb.MoveRotation(newRotation);

            Vector3 pos = rb.transform.position;
            float currentHeight = GetTerrainYRaycast(pos) + GetHeightOffset(unitMovement.unit);
            rb.MovePosition(new Vector3(pos.x, currentHeight, pos.z));
        }

        // Stops unit movement and snaps to grid position
        public void StopMovement(UnitMovement unitMovement)
        {
            Unit unit = unitMovement.unit;
            Rigidbody rb = unitMovement.rb;

            Vector3 worldPos = unit.unitBodyTransform.position;
            GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(worldPos);
            Vector3 finalPos = LevelGrid.Instance.GetWorldPosition(gridPosition);

            Vector3 pos = rb.transform.position;
            float currentHeight = GetTerrainYRaycast(pos) + GetHeightOffset(unit);
            rb.position = new Vector3(finalPos.x, currentHeight, finalPos.z);
        }

        // Gets terrain height at a position using raycast
        private float GetTerrainYRaycast(Vector3 nextPos)
        {
            if (Physics.Raycast(new Vector3(nextPos.x, nextPos.y + 100, nextPos.z), Vector3.down, out RaycastHit hit, float.MaxValue, terrainLayer))
            {
                return hit.point.y;
            }
            return nextPos.y;
        }

        // Returns height offset based on unit type
        private float GetHeightOffset(Unit unit)
        {
            BaseData baseData = (BaseData)unit.partsData[PartType.Base];

            switch (baseData.unitType)
            {
                case UnitType.Hover:
                    return hoverHeightOffset;
                case UnitType.Air:
                    return airHeightOffset;
                default:
                    return 0f;
            }
        }
    }
}
