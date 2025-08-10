using Unity.Cinemachine;
using TurnBasedStrategy.Controllers;
using TurnBasedStrategy.Data;

namespace TurnBasedStrategy
{
    public class SalvageCamerasData
    {
        public bool IsBlending { get; set; } = false;
        public CameraSettings BaseSettings { get; set; }
        public CameraSettings TorsoSettings { get; set; }
        public CameraSettings WeaponPrimarySettings { get; set; }
        public CameraSettings WeaponSecondarySettings { get; set; }
        public CinemachineCamera BaseCamera { get; set; }
        public CinemachineCamera TorsoCamera { get; set; }
        public CinemachineCamera PrimaryWeaponCamera { get; set; }
        public CinemachineCamera SecondaryWeaponCamera { get; set; }
        public VCamSalvageController SalvageCamera { get; set; }
        public float Distance { get; set; }
    }
}