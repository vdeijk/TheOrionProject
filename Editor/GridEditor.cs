using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace TurnBasedStrategy
{
    public class GridEditor : EditorWindow
    {
        [MenuItem("MyMenu/Do Something")]
        public static void ShowWindow()
        {
            GetWindow(typeof(GridEditor));
        }

        private GridData gridData;

        private void OnGUI()
        {
            // Assign GridData asset for editing
            gridData = (GridData)EditorGUILayout.ObjectField("Grid Data", gridData, typeof(GridData), false);
            if (gridData == null) return;

            if (GUILayout.Button("Create Grid"))
            {
                LevelGenerator.Instance.CreateGrid();
            }

            if (GUILayout.Button("Delete Objects"))
            {
                LevelGenerator.Instance.DeleteObjects();
            }

            if (GUILayout.Button("Place Objects"))
            {
                LevelGenerator.Instance.PlaceObjects();
                EditorUtility.SetDirty(this); // Ensure changes are saved
            }
        }
    }
}

