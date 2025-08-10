using UnityEngine;

namespace TurnBasedStrategy
{
    [DefaultExecutionOrder(300)]
    public class CursorController
    {
        private static CursorController _instance;

        public static CursorController Instance => _instance ??= new CursorController();

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
