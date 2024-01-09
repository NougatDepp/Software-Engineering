using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using Sequence = Unity.VisualScripting.Sequence;

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
        string playernumber = "Player " + (player+1);

        Transform slot = playerSlotsContainer.GetChild(player);
        Transform slotArtwork = slot.Find("Artwork");

        DG.Tweening.Sequence s = DOTween.Sequence();
        s.Append(slotArtwork.DOLocalMoveX(-300, .05f).SetEase(Ease.OutCubic));
        s.AppendCallback(() => slotArtwork.GetComponent<Image>().sprite = artwork);
        s.AppendCallback(() => slotArtwork.GetComponent<Image>().color = alpha);
        s.Append(slotArtwork.DOLocalMoveX(300, 0));
        s.Append(slotArtwork.DOLocalMoveX(0, .05f).SetEase(Ease.OutCubic));

        
        slot.Find("Artwork").GetComponent<Image>().sprite = artwork;
        slot.Find("PlayerX").GetComponent<TextMeshProUGUI>().text = playernumber;
        slot.Find("CharName").GetComponent<TextMeshProUGUI>().text = name;
    }

    public void ConfirmCharacter(int player, Character character)
    {
        if (confirmedCharacter == null)
        {
            confirmedCharacter = character;
            playerSlotsContainer.GetChild(player).DOPunchPosition(Vector3.down * 3, .3f, 10,1);

        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
