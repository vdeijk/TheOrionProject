using UnityEngine;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(-100)]
    // Handles unit faction assignment and enemy status
    public class UnitFactionController : MonoBehaviour
    {
        public bool IS_ENEMY { get; private set; }

        // Sets the faction and updates enemy status
        public void SetFaction(int factionSide)
        {
            IS_ENEMY = GetIsEnemy(factionSide);
        }

        // Returns true if factionSide is enemy
        private bool GetIsEnemy(int factionSide)
        {
            if (factionSide == 1)
            {
                return true;
            }
            return false;
        }
    }
}