using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SmashCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 focus = Vector3.one;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
        {
            focus += player.transform.position;
        }

        if (players.Length > 0)focus /= players.Length;
        focus += new Vector3(0, 0, -10);
        transform.position = focus;
        focus = Vector3.zero;
    }
}
