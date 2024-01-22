using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEditor;
using UnityEngine.Serialization;

public class MainMenuScript : MonoBehaviour
{
    private InputActionAsset inputAsset;
    private InputActionMap menu;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;

    public GameObject point;
  
    private int selectedButton = 1;
    [SerializeField] private int numberOfButtons;

    public Transform buttonPosition1;
    public Transform buttonPosition2;
    public Transform buttonPosition3;
    
    private AudioManager src;


    public void Start()
    {
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
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

    private void MoveThePointer()
    {
        if (selectedButton == 1)
        {
            point.transform.position = buttonPosition1.position;
        }
        else if (selectedButton == 2)
        {
            point.transform.position = buttonPosition2.position;
        }
        else if (selectedButton == 3)
        {
            point.transform.position = buttonPosition3.position;
        }
    }

    public void ShowOptionsMenu()
    {
        if (mainMenu.activeSelf)
        {
            src.PlaySound(src.buttonSelect);
            mainMenu.SetActive(false);
            optionsMenu.SetActive(true);
        }
    }
    public void ShowMainMenu()
    {
        if (!mainMenu.activeSelf)
        {
            src.PlaySound(src.buttonBack);
            optionsMenu.SetActive(false);
            mainMenu.SetActive(true);
        }
    }
    
    private void Choose(InputAction.CallbackContext context)
    {
        if (selectedButton == 1)
        {
            src.PlaySound(src.gameStart);
            GameManager.instance.LoadCharacterSelect();
        }
        else if (selectedButton == 2)
        {
            ShowOptionsMenu();
        }
        else if (selectedButton == 3)
        {
            src.PlaySound(src.buttonBack);
            Application.Quit();
        }
    }
    private void Up(InputAction.CallbackContext context)
    {
        if (selectedButton > 1)
        {
            selectedButton -= 1;
        }
        MoveThePointer();
    }
    private void Down(InputAction.CallbackContext context)
    {
        if (selectedButton < numberOfButtons)
        {
            selectedButton += 1;
        }
        MoveThePointer();
    }

}
