using UnityEngine;

namespace TurnBasedStrategy
{
    [CreateAssetMenu]
    public class ColorPalette : ScriptableObject
    {
        [Header("Button Colors")]
        public Color buttonNormal = Color.white;
        public Color buttonHover = Color.cyan;
        public Color buttonPressed = new Color(0.7f, 0.7f, 0.7f, 1f); // Light gray
        public Color buttonDisabled = new Color(0.5f, 0.5f, 0.5f, 1f); // Dark gray
        public Color buttonSelected = new Color(1f, 0.6f, 0f, 1f); // Orange - fully opaque!

        [Header("Theme Colors")]
        public Color colorRed = Color.red;
        public Color colorBlue = Color.blue;
        public Color colorYellow = Color.yellow;
    }
}