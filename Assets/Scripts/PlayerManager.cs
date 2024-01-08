using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public List<GameObject> players;

    public static PlayerManager instance;

    public GameObject playerr;
    public GameObject[] cursors;
    void Start()
    {
        instance = this;
        

    }
    
    public void UpdatePlayers(GameObject player)
    {
        players.Add(player);
    }
    void Update()
    {
        bool go = GameObject.FindGameObjectsWithTag("Cursor").All(player => player.GetComponent<CursorScript>().ready);
        if (go&&players.Count!=0&&GameObject.FindGameObjectsWithTag("Cursor").Length!=0)
        {
            //StartGame(players);
        }
        
    }

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
        cursors = GameObject.FindGameObjectsWithTag("Cursor");
        if (loadedScene.name == "CharacterSelect" && mode == LoadSceneMode.Single)
        {
            
            foreach (GameObject cursor in cursors)
            {
                cursor.GetComponent<SpriteRenderer>().enabled = true;
                cursor.GetComponent<CursorScript>().enabled = true;

            }
            Debug.Log("Code wird bei Szenenwechsel von 'a' nach 'b' ausgeführt.");
        }
        else
        {
            foreach (GameObject cursor in cursors)
            {
                cursor.GetComponent<SpriteRenderer>().enabled = false;
                cursor.GetComponent<CursorScript>().enabled = false;
            }
        }
    }

    private static void StartGame(List<GameObject> players)
    {
        GameObject playerr = null;
        GameObject[] cursors = GameObject.FindGameObjectsWithTag("Cursor");
        foreach (GameObject cursor in cursors)
        {
           Destroy(cursor);
        }
        Debug.Log("Ayyy");
        if (SceneManager.GetActiveScene().name != "FirstMap")
        {
            foreach (GameObject player in players)
            {
                //player.GetComponent<PlayerScript>().InstantiateCharacter();
            }
        }
        SceneManager.LoadScene("FirstMap");

        Debug.Log(SceneManager.GetActiveScene().name);
        
    }
    
    
    
}
