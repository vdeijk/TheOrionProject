using UnityEngine;
using TMPro;

namespace TurnBasedStrategy
{
    public class GridDebugObject : MonoBehaviour
    {
        [SerializeField] TextMeshPro textMeshPro;

        private GridObject gridObject;

        // Associates this debug object with a specific grid cell
        public void SetGridObject(GridObject gridObject)
        {
            this.gridObject = gridObject;
        }

        // Updates the debug display with the grid cell's current state
        private void Update()
        {
            textMeshPro.text = gridObject.ToString();
        }
    }
}
