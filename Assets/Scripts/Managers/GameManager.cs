using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    
    public static GameManager instance;
    
    public List<GameObject> players;

    public GameObject[] cursors;

    private bool readyToStartGame;

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
        cursors = GameObject.FindGameObjectsWithTag("Cursor");
        readyToStartGame = cursors.All(player => player.gameObject.GetComponent<CursorScript>().ready);
        
    }

    public void StartGame()
    {
        if (readyToStartGame)
        {
            SceneManager.LoadScene("FirstMap");       
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

            GameObject[] spawnpoints = GameObject.FindGameObjectsWithTag("Spawnpoint");
            
            foreach (GameObject player in players)
            {
                player.GetComponent<PlayerScript>().InstantiateCharacter(spawnpoints[player.GetComponent<PlayerScript>().id-1].gameObject.transform);
            }
        }
    }
}
