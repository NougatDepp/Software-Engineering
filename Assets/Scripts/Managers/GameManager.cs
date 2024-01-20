using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EasyTransition;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    
    public List<GameObject> players;

    public GameObject[] cursors;

    private bool readyToStartGame;

    public TransitionSettings transitionPrefab;
    public float loadDelay;

    public GameObject transitionObject;

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
        readyToStartGame = cursors.All(player => player.gameObject.GetComponent<CursorScript>().ready && players.Count >= 2);
        if (SceneManager.GetActiveScene().name == "FirstMap" && GameObject.FindGameObjectsWithTag("Character").Length == 1)
        {
            StartCoroutine(LoadWinnigScreen());
        }
    }
    
    public void LoadMainMenu()
    {
        cursors = GameObject.FindGameObjectsWithTag("Cursor");
        
        foreach (GameObject player in players)
        {
            Destroy(player);
        }

        players.Clear();
        Destroy(GameObject.FindWithTag("InputManager"));
        
        foreach (GameObject cursor in cursors)
        {
            cursor.GetComponent<SpriteRenderer>().enabled = false;
            cursor.GetComponent<CursorScript>().enabled = false;
        }

        StartCoroutine(LoadScene("MainMenu"));
    }
    
    public void LoadCharacterSelect()
    {
        StartCoroutine(LoadScene("CharacterSelect"));
    }
    
    public IEnumerator LoadWinnigScreen()
    {
        yield return new WaitForSeconds(1);
        foreach (GameObject character in GameObject.FindGameObjectsWithTag("Character"))
        {
            Destroy(character);
        }
        StartCoroutine(LoadScene("WinningScreen"));
    }
    
    public void LoadStartGame()
    {
        if (readyToStartGame)
        {
            StartCoroutine(LoadScene("FirstMap"));
        }
    }
    
    private IEnumerator LoadScene(String sceneName)
    {
        yield return new WaitForSeconds(1);
        if(GameObject.FindGameObjectsWithTag("Transition").Length == 0)
        {
            GameObject t = Instantiate(transitionObject);
            t.GetComponent<TransitionScript>().Transition(sceneName,transitionPrefab,loadDelay);
        }
        yield return null;
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
