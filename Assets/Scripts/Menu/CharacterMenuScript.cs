using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CharacterMenuScript : MonoBehaviour
{
    public static CharacterMenuScript instance;
    
    public List<Character> characters = new List<Character>();

    public GameObject charCellPrefab;
    public Transform playerSlotsContainer;
    
    public Character confirmedCharacter;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {

        foreach (Character character in characters)
        {
            SpawnCharacterCell(character);
        }
    }

    private void SpawnCharacterCell(Character character)
    {
        GameObject charCell = Instantiate(charCellPrefab, transform);
        
        charCell.name = character.characterName;

        Image artwork = charCell.transform.Find("Artwork").GetComponent<Image>();
        TextMeshProUGUI name = charCell.transform.Find("NameRect").GetComponent<TextMeshProUGUI>();

        artwork.sprite = character.characterSprite;
        name.text = character.characterName;
    }

    public void ShowCharacterInSlot(int player, Character character)
    {
        bool nullChar = (character == null);

        Color alpha = nullChar ? Color.clear : Color.white;
        Sprite artwork = nullChar ? null : character.characterSprite;
        string name = nullChar ? null : character.characterName;
        string playernumber = "Player " + player;

        Transform slot = playerSlotsContainer.GetChild(player);
        
        
        slot.Find("Artwork").GetComponent<Image>().sprite = artwork;
        slot.Find("PlayerX").GetComponent<TextMeshProUGUI>().text = playernumber;
        slot.Find("CharName").GetComponent<TextMeshProUGUI>().text = name;
    }

    public void ConfirmCharacter(Character character)
    {
        if (confirmedCharacter == null)
        {
            confirmedCharacter = character;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
