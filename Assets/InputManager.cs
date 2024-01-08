
using UnityEngine;


public class InputManager : MonoBehaviour
{
    [HideInInspector]
    
    private static InputManager instance;


    private void Awake()
    {
        // Überprüfen, ob bereits eine Instanz existiert
        if (instance != null && instance != this)
        {
            // Eine andere Instanz existiert bereits, dieses GameObject zerstören
            Destroy(gameObject);
            return;
        }

        // Dies ist die einzige Instanz, sie als Singleton festlegen
        instance = this;

        // GameObject mit DontDestroyOnLoad beibehalten
        DontDestroyOnLoad(gameObject);

        // Fügen Sie hier Ihren weiteren Initialisierungscode hinzu
    }
}
