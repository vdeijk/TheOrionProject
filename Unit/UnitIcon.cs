using UnityEngine;
using UnityEngine.UI;

namespace TurnBasedStrategy
{
    // Sets the unit icon and color based on faction
    public class UnitIcon : MonoBehaviour
    {
        [SerializeField] UnitFaction unitFaction;
        [SerializeField] Sprite iconSpriteAlly;
        [SerializeField] Sprite iconSpriteEnemy;
        [SerializeField] Image iconImage;

        private Color32 allyColor = new Color32(0, 241, 213, 205);
        private Color32 enemyColor = new Color32(225, 0, 0, 205);

        private void Start()
        {
            if (unitFaction.IS_ENEMY)
            {
                iconImage.sprite = iconSpriteEnemy;
                iconImage.color = enemyColor;
            }
            else
            {
                iconImage.sprite = iconSpriteAlly;
                iconImage.color = allyColor;
            }
        }
    }
}