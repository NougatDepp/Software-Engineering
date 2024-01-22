using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;


public class HealthBarUpdater : MonoBehaviour
{
    public GameObject player;
    private TextMeshProUGUI healthText;
    private Sprite artwork;
    private float health;
    private float lastHealth;

    void Start()
    {
        healthText = gameObject.transform.Find("Health").GetComponent<TextMeshProUGUI>();
        gameObject.transform.Find("Player").GetComponent<TextMeshProUGUI>().text = "P"+player.GetComponent<PlayerScript>().id;
        gameObject.transform.Find("Icon").GetComponent<Image>().sprite = player.GetComponent<PlayerScript>().playerCharacter.characterSprite;
        lastHealth = 0;
    }

    void Update()
    {
        if (player.transform.Find("Character") == null)
        {
            Destroy(gameObject);
        }
        else
        {
            health = player.transform.Find("Character").GetComponent<GameplayScript>().hitpoint;
            if (health != lastHealth)
            {
                OnTextChange();
                lastHealth = health;
            }
            UpdateHealthBar();
        }
    }

    private void OnTextChange()
    {
        gameObject.transform.DOPunchPosition(new Vector3(Random.value,Random.value,0).normalized * 10, .3f, 10,1);
    }

    private void UpdateHealthBar()
    {
        Color newColor = new Color(1, 1 - health / 999, 1 - health / 333, 1);
        healthText.CrossFadeColor(newColor, 0.1f, true, false);
        healthText.text = health + "%";
    }
}