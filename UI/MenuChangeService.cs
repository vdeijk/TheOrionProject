using System.Collections;
using System;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class MenuChangeService : Singleton<MenuChangeService>
    {
        public MenuType curMenu { get; private set; } = MenuType.Main;
        public MenuType prevMenu { get; private set; } = MenuType.Main;

        public delegate void MenuChangeDelegate();

        [SerializeField] float resetTime;

        private bool canChangeMenu = true;

        public static event EventHandler OnMenuChanged;

        public IEnumerator Delay(float delay, MenuChangeDelegate menuChange)
        {
            yield return new WaitForSecondsRealtime(delay);

            menuChange();
        }

        public void ToPrevMenu()
        {
            if (prevMenu != curMenu)
            {
                ChangeMenu(prevMenu);
            }
        }

        public void ToMainMenu()
        {
            ChangeMenu(MenuType.Main);
        }

        public void ToIntroMenu()
        {
            ChangeMenu(MenuType.NewGame);
        }

        public void ToPreparationMenu()
        {
            ChangeMenu(MenuType.Preparation);
        }

        public void ToAssembleMenu()
        {
            ChangeMenu(MenuType.Assemble);
        }

        public void ToOptionsMenu()
        {
            ChangeMenu(MenuType.Options);
        }

        public void ToPauseMenu()
        {
            ChangeMenu(MenuType.Pause);
        }

        public void ToNone()
        {
            ChangeMenu(MenuType.None);
        }

        public void ToVictoryMenu()
        {
            ChangeMenu(MenuType.MissionEnd);
        }

        public void ToDefeatMenu()
        {
            ChangeMenu(MenuType.GameOver);
        }

        public void ToSalvageMenu()
        {
            ChangeMenu(MenuType.Salvage);
        }

        public void ToRepairMenu()
        {
            ChangeMenu(MenuType.Repair);
        }

        public void ToCreditsMenu()
        {
            ChangeMenu(MenuType.Credits);
        }

        private void ChangeMenu(MenuType newMenu)
        {
            if (!canChangeMenu) return;

            prevMenu = curMenu;
            curMenu = newMenu;
            canChangeMenu = false;

            StartCoroutine(ResetCanChangeMode());

            OnMenuChanged?.Invoke(this, EventArgs.Empty);
        }

        private IEnumerator ResetCanChangeMode()
        {
            yield return new WaitForSecondsRealtime(resetTime);

            canChangeMenu = true;
        }
    }
}
