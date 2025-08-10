using System;
using UnityEngine;

namespace TurnBasedStrategy.Infra
{
    public class SFXMonobService : SingletonBaseService<SFXMonobService>
    {
        [SerializeField] AudioSource ASExecuteAction;
        [SerializeField] AudioSource ASSelectedUnitChange;
        [SerializeField] AudioSource ASDelectedUnit;
        [SerializeField] AudioSource ASPhaseStart;
        [SerializeField] AudioSource ASActionCompleted;
        [SerializeField] AudioSource ASTileSelected;
        [SerializeField] AudioSource ASButtonClick;
        [SerializeField] AudioSource ASPause;
        [SerializeField] AudioSource ASSuccess;
        [SerializeField] AudioSource ASFailure;
        [SerializeField] AudioSource ASPopup;

        public void PlaySFX(SFXType SFXType)
        {
            switch (SFXType)
            {
                case SFXType.SelectUnit:
                    ASSelectedUnitChange.Play(); // Play sound effect for selecting a unit
                    break;
                case SFXType.DeselectUnit:
                    ASDelectedUnit.Play(); // Play sound effect for deselecting a unit
                    break;
                case SFXType.StartPhase:
                    ASPhaseStart.Play(); // Play sound effect for starting a phase
                    break;
                case SFXType.CompleteAction:
                    ASActionCompleted.Play(); // Play sound effect for completing an action
                    break;
                case SFXType.SelectTile:
                    ASTileSelected.Play(); // Play sound effect for selecting a tile
                    break;
                case SFXType.ClickButton:
                    ASButtonClick.Play(); // Play sound effect for clicking a button
                    break;
                case SFXType.Pause:
                    ASPause.Play(); // Play sound effect for pausing the game
                    break;
                case SFXType.ExecuteAction:
                    ASExecuteAction.Play(); // Play sound effect for executing an action
                    break;
                case SFXType.Success:
                    ASSuccess.Play(); // Play sound effect for success
                    break;
                case SFXType.Failure:
                    ASFailure.Play(); // Play sound effect for failure
                    break;
                case SFXType.Popup:
                    ASPopup.Play(); // Play sound effect for popup
                    break;
            }
        }
    }
}

