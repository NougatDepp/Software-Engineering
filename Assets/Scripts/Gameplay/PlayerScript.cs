using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class PlayerScript : MonoBehaviour
{
    public int id;
    public Character playerCharacter;
    public int placement = 1;
    private GameObject player;
    private void Awake()
    {
        GameManager.instance.UpdatePlayers(gameObject);
        id = GameManager.instance.players.Count;
        gameObject.transform.name = "Player " + id;

        gameObject.transform.Find("Cursor").GetComponent<CursorScript>().SetID(id-1);
        
    }

    public void PlacementUpdater()
    {
        placement = GameObject.FindGameObjectsWithTag("Character").Length;
    }

    public void SetCharacter(Character character)
    {
        this.playerCharacter = character;
    }

    public void InstantiateCharacter(Transform transformSpawnpoint)
    {
        player = Instantiate(playerCharacter.character,transform);
        player.gameObject.transform.position = transformSpawnpoint.position;
    }

    public PlayerScript GetPlayerScript()
    {
        return this;
    }
}
