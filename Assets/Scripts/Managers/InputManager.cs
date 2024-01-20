
using UnityEngine;


public class InputManager : MonoBehaviour
{
    [HideInInspector]
    
    private static InputManager instance;
    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);

    }
}
