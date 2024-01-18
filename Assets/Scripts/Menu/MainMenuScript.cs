using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEditor;

public class MainMenuScript : MonoBehaviour
{
    private InputActionAsset inputAsset;
    private InputActionMap menu;

    public GameObject Point;
  
    private int SelectedButton = 1;
    [SerializeField]
    private int NumberOfButtons;

    public Transform ButtonPosition1;
    public Transform ButtonPosition2;
    public Transform ButtonPosition3;

    private void Awake()
    {
        inputAsset = gameObject.transform.parent.GetComponent<PlayerInput>().actions;
        menu = inputAsset.FindActionMap("Menu");
    }

    public void OnEnable()
    {
        menu.FindAction("Choose").started += Choose;
        menu.FindAction("Up").started += Up;
        menu.FindAction("Down").started += Down;
    }

    public void OnDisable()
    {
        menu.FindAction("Choose").started -= Choose;
        menu.FindAction("Up").started -= Up;
        menu.FindAction("Down").started -= Down;
    }
    private void Choose(InputAction.CallbackContext context)
    {
        if (SelectedButton == 1)
        {
            SceneManager.LoadScene("CharacterSelect");
        }
        //else if (SelectedButton == 2)
        //{
        //    //Settings
        //}
        else if (SelectedButton == 3)
        {
            Application.Quit();
        }
    }
    private void Up(InputAction.CallbackContext context)
    {
        if (SelectedButton > 1)
        {
            SelectedButton -= 1;
        }
        MoveThePointer();
        return;
    }
    private void Down(InputAction.CallbackContext context)
    {
        if (SelectedButton < NumberOfButtons)
        {
            SelectedButton += 1;
        }
        MoveThePointer();
        return;
    }
    private void MoveThePointer()
    {
        if (SelectedButton == 1)
        {
            Point.transform.position = ButtonPosition1.position;
        }
        else if (SelectedButton == 2)
        {
            Point.transform.position = ButtonPosition2.position;
        }
        else if (SelectedButton == 3)
        {
            Point.transform.position = ButtonPosition3.position;
        }
    }
    
}
