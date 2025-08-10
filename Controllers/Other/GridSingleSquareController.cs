using UnityEngine;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.Controllers
{
    public class GridSingleSquareController : MonoBehaviour
    {
        [SerializeField] MeshRenderer meshRender;

        private void Awake()
        {
            // Initialize as hidden and fully transparent
            Color color = meshRender.material.color;
            color.a = 0;
            meshRender.material.color = color;
            meshRender.enabled = false;
        }

        // Shows the grid visual with the given material and alpha based on unit action points
        public void Show(Material material)
        {
            if (meshRender.material.color.a >= 1 && meshRender.enabled) return;

            meshRender.material = material;

            UnitSingleController unit = UnitSelectService.Instance.Data.SelectedUnit;
            if (unit != null && unit.Data.UnitMindTransform.GetComponent<UnitActionController>().actionPoints > 0)
            {
                Color color = meshRender.material.color;
                color.a = 1;
                meshRender.material.color = color;
                meshRender.enabled = true;
            }
            else
            {
                Color color = meshRender.material.color;
                color.a = .5f;
                meshRender.material.color = color;
                meshRender.enabled = true;
            }
        }

        // Hides the grid visual
        public void Hide()
        {
            if (meshRender.material.color.a <= 0 && meshRender.enabled) return;
            meshRender.enabled = false;
        }
    }
}
