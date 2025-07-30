using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Manages switching and activation of scene objects for different game states
public class SceneManager : MonoBehaviour
{
    private GameObject[] gameSceneObjects;
    private GameObject[] assemblySceneObjects;
    private GameObject[][] allSceneObjects;

    private void Start()
    {
        allSceneObjects = new GameObject[2][];

        // Find and store references to main game scene objects by tag
        gameSceneObjects = new GameObject[]
        {
            GameObject.FindWithTag("GameUI"),
            GameObject.FindWithTag("GamePostProcessing"),
            GameObject.FindWithTag("GameCameras"),
            GameObject.FindWithTag("GameLighting")
        };

        assemblySceneObjects = GetAssemblySceneObjects();

        allSceneObjects[0] = gameSceneObjects;
        allSceneObjects[1] = assemblySceneObjects;

        UpdateSceneState(1); // Start with assembly scene active
    }

    // Switches active scene objects based on game state
    public void SwitchMainScene(int scene)
    {
        UpdateSceneState(scene);
    }

    // Finds root objects in the assembly scene by tag
    private GameObject[] GetAssemblySceneObjects()
    {
        Scene assemblyScene = GetSceneByName("Assembly");
        if (!assemblyScene.IsValid() || !assemblyScene.isLoaded)
        {
            return new GameObject[0];
        }

        List<GameObject> foundObjects = new List<GameObject>();

        foreach (GameObject obj in assemblyScene.GetRootGameObjects())
        {
            if (obj.CompareTag("AssemblyUI") || obj.CompareTag("AssemblyPostProcessing") ||
                obj.CompareTag("AssemblyCameras") || obj.CompareTag("AssemblyLighting"))
            {
                foundObjects.Add(obj);
            }
        }

        return foundObjects.ToArray();
    }

    // Gets a scene by name from loaded scenes
    private Scene GetSceneByName(string sceneName)
    {
        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
        {
            Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
            if (scene.name == sceneName)
            {
                return scene;
            }
        }

        return new Scene();
    }

    // Activates objects for the selected scene, deactivates others
    private void UpdateSceneState(int activeScene)
    {
        for (int i = 0; i < allSceneObjects.Length; i++)
        {
            SetActiveObjects(false, allSceneObjects[i]);
        }

        SetActiveObjects(true, allSceneObjects[activeScene]);
    }

    // Sets active state for a list of objects
    private void SetActiveObjects(bool state, params GameObject[] objects)
    {
        foreach (var obj in objects)
        {
            if (obj != null)
            {
                obj.SetActive(state);
            }
        }
    }
}
