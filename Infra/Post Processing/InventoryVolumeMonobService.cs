using UnityEngine;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.Infra
{
    public class InventoryVolumeMonobService : SingletonBaseService<InventoryVolumeMonobService>
    {
        [SerializeField] Transform volumeTransform;

        // Sets the post-processing volume's position to match the current part preview
        public void SetPosition()
        {
            volumeTransform.position = AssemblyService.Instance.Data.CurPreviewTransform.position;
        }
    }
}