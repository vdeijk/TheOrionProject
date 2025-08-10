using UnityEngine;

namespace TurnBasedStrategy.Data
{
    public class ColorData
    {
        public static readonly ColorData Instance = new ColorData();

        [Header("Button Colors")]
        public readonly Color32 buttonNormal = new Color32(2, 2, 2, 255);     
        public readonly Color32 buttonHover = new Color32(0, 241, 213, 255);
        public readonly Color32 buttonSelected = new Color32(0, 192, 170, 255); // #00C0AA
        public readonly Color32 buttonPressed = new Color32(0, 144, 127, 255); // #00907F
        public readonly Color32 buttonDisabled = new Color32(0, 96, 85, 255); // #006055

        [Header("Theme Colors")]
        public readonly Color32 colorRed = new Color32(255, 0, 0, 255);
        public readonly Color32 colorBlue = new Color32(0, 241, 213, 255);
        public readonly Color32 colorYellow = new Color32(225, 207, 0, 255);
    }
}