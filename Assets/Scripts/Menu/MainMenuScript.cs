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

    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject OptionsMenu;

    public GameObject Point;
  
    private int SelectedButton = 1;
    [SerializeField] private int NumberOfButtons;

    public Transform ButtonPosition1;
    public Transform ButtonPosition2;
    public Transform ButtonPosition3;
    
    public AudioClip buttonBack;
    public AudioClip buttonSelect;
    public AudioClip gameStart;

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
            AudioManager.Instance.PlaySound(gameStart);
            GameManager.instance.LoadCharacterSelect();
        }
        else if (SelectedButton == 2)
        {
            ShowOptionsMenu();
        }
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

    public void ShowOptionsMenu()
    {
        if (MainMenu.activeSelf)
        {
            MainMenu.SetActive(false);
            OptionsMenu.SetActive(true);
            AudioManager.Instance.PlaySound(buttonSelect);
        }
    }
    public void ShowMainMenu()
    {
        if (!MainMenu.activeSelf)
        {
            OptionsMenu.SetActive(false);
            MainMenu.SetActive(true);
            AudioManager.Instance.PlaySound(buttonBack);
        }
        
    }

}
