using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InstaciateScript : MonoBehaviour
{
    public GameObject gameManager;
    private void Awake()
    {
        Instantiate(gameManager);
    }
}
