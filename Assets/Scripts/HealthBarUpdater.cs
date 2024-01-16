using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class HealthBarUpdater : MonoBehaviour
{
    public GameObject player;
    private TextMeshProUGUI textComponent;
    private float health;
    private float lastHealth;

    void Start()
    {
        textComponent = gameObject.transform.GetComponent<TextMeshProUGUI>();
        lastHealth = 0;
    }

    void Update()
    {
        Destroy(gameObject);
        UpdateHealthBar();
        health = player.GetComponent<GameplayScript>().hitpoint;
        if (health != lastHealth)
        {
            OnTextChange();
            lastHealth = health;
        }
    }

    private void OnTextChange()
    {
        gameObject.transform.DOPunchPosition(new Vector3(Random.value,Random.value,0).normalized * 10, .3f, 10,1);
    }

    void UpdateHealthBar()
    {
        //health.GetComponent<TextScript>().GetAnim().SetTrigger("OnHit");
        Color newColor = new Color(1, 1 - health / 999, 1 - health / 333, 1);
        textComponent.CrossFadeColor(newColor, 0.1f, true, false);
        textComponent.text = health + "%";
    }
}