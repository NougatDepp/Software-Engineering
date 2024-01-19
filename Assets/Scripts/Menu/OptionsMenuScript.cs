using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class OptionsMenuScript : MonoBehaviour
{

    private InputActionAsset inputAsset;
    private InputActionMap menu;

    private void Awake()
    {
        inputAsset = GetComponent<PlayerInput>().actions;
        menu = inputAsset.FindActionMap("Menu");
    }

    public void OnEnable()
    {
        menu.FindAction("Back").started += Back;
    }

    public void OnDisable()
    {
        menu.FindAction("Back").started -= Back;
    }

    private void Back(InputAction.CallbackContext context)
    {
        
    }

    public void SetVolume (float volume)
    {

    }
}
