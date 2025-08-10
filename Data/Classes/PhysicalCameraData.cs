using System;
using UnityEngine;

namespace TurnBasedStrategy.Data
{
    [Serializable]
    public class PhysicalCameraData
    {
        [field: SerializeField] public Camera BillboardCam { get; private set; }
        [field: SerializeField] public Camera InventoryCam { get; private set; }
        [field: SerializeField] public Camera IconCam { get; private set; }
        [field: SerializeField] public float Distance { get; private set; } = 8f;
        [field: SerializeField] public DurationData DurationData { get; private set; }

    }
}