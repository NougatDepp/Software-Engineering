using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeHandler : MonoBehaviour
{
    private bool hasCodeExecuted = false;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!hasCodeExecuted)
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.SceneChange(scene,mode);
            }
            hasCodeExecuted = true;
        }
    }
}