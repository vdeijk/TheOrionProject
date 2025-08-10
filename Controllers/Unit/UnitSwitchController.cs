using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;
using TurnBasedStrategy.Infra;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(-100)]
    // Handles switching between units with cooldown
    public class UnitSwitchController : MonoBehaviour
    {
        protected bool canSwitch = true;
        protected float duration;

        protected DurationData durationData => DurationData.Instance;

        // Coroutine to reset switch ability after duration
        protected IEnumerator ResetCanSwitch()
        {
            yield return new WaitForSeconds(duration);
            canSwitch = true;
        }

        // Switches to the next unit in the list
        protected void SwitchUp(List<UnitSingleController> units)
        {
            canSwitch = false;
            UnitSelectService.Instance.SelectNextUnit(units);
            SFXMonobService.Instance.PlaySFX(SFXType.ClickButton);
            StartCoroutine(ResetCanSwitch());
        }

        // Switches to the previous unit in the list
        protected void SwitchDown(List<UnitSingleController> units)
        {
            canSwitch = false;
            UnitSelectService.Instance.SelectPrevUnit(units);
            SFXMonobService.Instance.PlaySFX(SFXType.ClickButton);
            StartCoroutine(ResetCanSwitch());
        }
    }
}