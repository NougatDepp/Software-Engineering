using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public List<GameObject> players;

    public static GameManager instance;

    public GameObject[] cursors;
    
    private bool hasCodeExecuted = false;

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

    
    public void SceneChange(Scene scene,LoadSceneMode mode)
    {
        cursors = GameObject.FindGameObjectsWithTag("Cursor");
        if (scene.name == "CharacterSelect" && mode == LoadSceneMode.Single)
        {
            
            foreach (GameObject cursor in cursors)
            {
                cursor.GetComponent<SpriteRenderer>().enabled = true;
                cursor.GetComponent<CursorScript>().enabled = true;
            }
        }
        else
        {
            foreach (GameObject cursor in cursors)
            {
                cursor.GetComponent<SpriteRenderer>().enabled = false;
                cursor.GetComponent<CursorScript>().enabled = false;
            }

            foreach (GameObject player in players)
            {
                player.GetComponent<PlayerScript>().InstantiateCharacter();
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
