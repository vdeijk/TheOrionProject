using UnityEngine;

namespace TurnBasedStrategy
{
    public class InventoryVolume : Singleton<InventoryVolume>
    {
        [SerializeField] Transform volumeTransform;

        // Sets the post-processing volume's position to match the current part preview
        public void SetPosition()
        {
            volumeTransform.position = PartPreviewService.Instance.curPreviewTransform.position;
        }
    }
}