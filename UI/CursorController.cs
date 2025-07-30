using UnityEngine;

namespace TurnBasedStrategy
{
    public class CursorController : Singleton<CursorController>
    {
        public void SetCursorHiddenAndLocked()
        {
            Instance.SetCursor(false, CursorLockMode.Locked);
        }

        public void SetCursorVisibleAndUnlocked()
        {
            Instance.SetCursor(true, CursorLockMode.None);
        }

        private void SetCursor(bool isCursorVisible, CursorLockMode cursorLockMode)
        {
            Cursor.visible = isCursorVisible;
            Cursor.lockState = cursorLockMode;
        }
    }
}
