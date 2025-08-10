using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TurnBasedStrategy.Data;
using TurnBasedStrategy.Domain;

namespace TurnBasedStrategy.Controllers
{
    [DefaultExecutionOrder(-100)]
    public class GridEditorController : EditorWindow
    {
        private GridData Data => GridCoordinatorService.Instance.Data;

        [MenuItem("MyMenu/Do Something")]
        public static void ShowWindow()
        {
            GetWindow(typeof(GridEditorController));
        }

        private void OnGUI()
        {
            if (GUILayout.Button("Create Grid"))
            {
                LevelGeneratorService.Instance.CreateGrid();
            }

            if (GUILayout.Button("Delete Objects"))
            {
                Data.Controller.DeleteObjects();
            }

            if (GUILayout.Button("Place Objects"))
            {
                LevelGeneratorService.Instance.PlaceObjects();
                EditorUtility.SetDirty(this); // Ensure changes are saved
            }
        }
    }
}

