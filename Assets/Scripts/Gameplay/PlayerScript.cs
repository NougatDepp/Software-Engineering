using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    public int id;
    public GameObject character;
    private void Awake()
    {
        PlayerManager.instance.UpdatePlayers(gameObject);
        id = PlayerManager.instance.players.Count;
        gameObject.transform.name = "Player " + id;

        gameObject.transform.Find("Cursor").GetComponent<CursorScript>().SetID(id-1);
    }

    private void Update()
    {

    }
    
    public void SetCharacter(GameObject character)
    {
        this.character = character;
    }

    public void InstantiateCharacter()
    {
        Instantiate(character, transform);
    }
}
