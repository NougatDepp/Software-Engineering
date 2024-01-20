﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEditor;

public class MainMenuScript : MonoBehaviour
{
    private InputActionAsset inputAsset;
    private InputActionMap menu;

    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject OptionsMenu;

    public GameObject Point;
  
    private int SelectedButton = 1;
    [SerializeField] private int NumberOfButtons;

    public Transform ButtonPosition1;
    public Transform ButtonPosition2;
    public Transform ButtonPosition3;
    
    private AudioManager src;


    public void Start()
    {
        OptionsMenu.SetActive(false);
        MainMenu.SetActive(true);
    }

    private void Awake()
    {
        inputAsset = GetComponent<PlayerInput>().actions;
        menu = inputAsset.FindActionMap("Menu");
    }

    public void OnEnable()
    {
        src = AudioManager.Instance;
        menu.FindAction("Choose").started += Choose;
        menu.FindAction("Up").started += Up;
        menu.FindAction("Down").started += Down;

        menu.Enable();
    }

    public void OnDisable()
    {
        menu.FindAction("Choose").started -= Choose;
        menu.FindAction("Up").started -= Up;
        menu.FindAction("Down").started -= Down;
        
        menu.Disable();
    }
    private void Choose(InputAction.CallbackContext context)
    {
        if (SelectedButton == 1)
        {
            src.PlaySound(src.gameStart);
            GameManager.instance.LoadCharacterSelect();
        }
        else if (SelectedButton == 2)
        {
            ShowOptionsMenu();
        }
        else if (SelectedButton == 3)
        {
            src.PlaySound(src.buttonBack);
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
    }
    private void Down(InputAction.CallbackContext context)
    {
        if (SelectedButton < NumberOfButtons)
        {
            SelectedButton += 1;
        }
        MoveThePointer();
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

    public void ShowOptionsMenu()
    {
        if (MainMenu.activeSelf)
        {
            src.PlaySound(src.buttonSelect);
            MainMenu.SetActive(false);
            OptionsMenu.SetActive(true);
        }
    }
    public void ShowMainMenu()
    {
        if (!MainMenu.activeSelf)
        {
            src.PlaySound(src.buttonBack);
            OptionsMenu.SetActive(false);
            MainMenu.SetActive(true);
        }
        
    }

}
