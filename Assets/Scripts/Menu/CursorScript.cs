using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorScript : MonoBehaviour
{

    private InputActionAsset inputAsset;
    private InputActionMap menu;
    private InputAction move;



    private void Awake(){

        inputAsset = this.GetComponent<PlayerInput>().actions;
        menu = inputAsset.FindActionMap("Menu");
    }

    private void OnAnable(){

        menu.FindAction("Choose").started += Choose;
        menu.FindAction("Back").started += Back;

        move = inputAsset.FindAction("Move");
    }

    private void FixedUpdate()
    {
        transform.position += new Vector3(move.ReadValue<Vector2>().x, move.ReadValue<Vector2>().y, 0);
    }

    private void Back(InputAction.CallbackContext context)
    {
        
    }

    private void Choose(InputAction.CallbackContext context)
    {
        
    }
}
