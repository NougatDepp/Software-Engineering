using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour
{
    public GameObject textPrefab;
    public GridLayoutGroup gridLayoutGroup;

    void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        
        foreach (GameObject player in players)
        {
            CreateHealthBar(player);
        }
    }

    void CreateHealthBar(GameObject player)
    {
        GameObject healthBar = Instantiate(textPrefab, gridLayoutGroup.transform);

        HealthBarUpdater updater = healthBar.AddComponent<HealthBarUpdater>();
        updater.player = player;
    }
}
