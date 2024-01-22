using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Placer : MonoBehaviour
{
    private GameObject[] playerList;
    
    void Start()
    {
        playerList = GameObject.FindGameObjectsWithTag("Player");
        StartCoroutine(Placements());
    }

    public IEnumerator Placements()
    {
        yield return new WaitForSeconds(1.5f);

        for (int i = 1; i <= playerList.Length; i++)
        {
            PlayerScript playerScript = FindPlayerWithPlace(i);
            Character character = playerScript.playerCharacter;
            bool nullChar = (character == null);

            Color alpha = nullChar ? Color.clear : Color.white;
            Sprite artwork = nullChar ? null : character.characterSprite;
            string name = nullChar ? null : character.characterName;

            Transform slot = gameObject.transform.Find(i.ToString());
            Transform slotArtwork = slot.Find("Artwork");

            DG.Tweening.Sequence s = DOTween.Sequence();
            s.Append(slotArtwork.DOLocalMoveX(-300, .05f).SetEase(Ease.OutCubic));
            s.AppendCallback(() => slotArtwork.GetComponent<Image>().sprite = artwork);
            s.AppendCallback(() => slotArtwork.GetComponent<Image>().color = alpha);
            s.Append(slotArtwork.DOLocalMoveX(300, 0));
            s.Append(slotArtwork.DOLocalMoveX(0, .05f).SetEase(Ease.OutCubic));

            slot.Find("Artwork").GetComponent<Image>().sprite = artwork;
            slot.Find("CharName").GetComponent<TextMeshProUGUI>().text = name;
            slot.Find("PlayerX").GetComponent<TextMeshProUGUI>().text =
                "Player " + playerScript.id; 

            yield return new WaitForSeconds(0.5f);
        }
        
        yield return new WaitForSeconds(6);

        GameManager.instance.LoadCharacterSelect();

    }
    
    public PlayerScript FindPlayerWithPlace(int targetPlace)
    {
        foreach (GameObject player in playerList)
        {
            if (player.GetComponent<PlayerScript>().placement == targetPlace)
            {
                return player.GetComponent<PlayerScript>().GetPlayerScript();
            }
        }
        return null;
    }

}
