using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace TurnBasedStrategy
{
    // Handles switching between units with cooldown
    public class UnitSwitching : MonoBehaviour
    {
        [field: SerializeField] protected GameDurations gameDurations { get; private set; }
        protected bool canSwitch = true;
        protected float duration;

        // Coroutine to reset switch ability after duration
        protected IEnumerator ResetCanSwitch()
        {
            yield return new WaitForSeconds(duration);
            canSwitch = true;
        }

        // Switches to the next unit in the list
        protected void SwitchUp(List<Unit> units)
        {
            canSwitch = false;
            UnitSelectService.Instance.SelectNextUnit(units);
            SFXController.Instance.PlaySFX(SFXType.ClickButton);
            StartCoroutine(ResetCanSwitch());
        }

        // Switches to the previous unit in the list
        protected void SwitchDown(List<Unit> units)
        {
            canSwitch = false;
            UnitSelectService.Instance.SelectPrevUnit(units);
            SFXController.Instance.PlaySFX(SFXType.ClickButton);
            StartCoroutine(ResetCanSwitch());
        }
    }
}