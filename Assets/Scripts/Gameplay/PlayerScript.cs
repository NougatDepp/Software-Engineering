using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    public int id;
    public GameObject character;
    public int lives = 3;
    private void Awake()
    {
        GameManager.instance.UpdatePlayers(gameObject);
        id = GameManager.instance.players.Count;
        gameObject.transform.name = "Player " + id;

        gameObject.transform.Find("Cursor").GetComponent<CursorScript>().SetID(id-1);
        
    }

    public void SetCharacter(GameObject character)
    {
        this.character = character;
    }

    public void InstantiateCharacter(Transform transformSpawnpoint)
    {
        GameObject player = Instantiate(character,transform);
        player.gameObject.transform.position = transformSpawnpoint.position;
    }
}
