using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ToggleOnSceneCgange : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene loadedScene, LoadSceneMode mode)
    {
        // Check if the loaded scene is named "a"
        if (loadedScene.name != "CharacterSelect")
        {
            // Disable the object if the scene is "a"
            gameObject.SetActive(false);
        }
        // Check if the loaded scene is named "b"
        else if (loadedScene.name == "CharcterSelect")
        {
            // Enable the object if the scene is "b"
            gameObject.SetActive(true);
        }
    }
}
