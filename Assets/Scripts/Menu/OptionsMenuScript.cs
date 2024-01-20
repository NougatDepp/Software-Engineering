using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class OptionsMenuScript : MonoBehaviour
{
    
    private InputActionAsset inputAsset;
    private InputActionMap menu;
    
    public Slider volumeSlider;

    private const string VolumeKey = "GameVolume";

    private void Awake()
    {
        inputAsset = gameObject.transform.parent.GetComponent<PlayerInput>().actions;
        menu = inputAsset.FindActionMap("Menu");
    }
    
    void Start()
    {
        if (!PlayerPrefs.HasKey(VolumeKey))
        {
            PlayerPrefs.SetFloat(VolumeKey, 0.5f);
        }
        volumeSlider.value = PlayerPrefs.GetFloat(VolumeKey);
    }
    
    public void OnEnable()
    {
        menu.FindAction("Back").started += Back;
        menu.FindAction("Left").started += Left;
        menu.FindAction("Right").started += Right;
        menu.Enable();
    }

    public void OnDisable()
    {
        menu.FindAction("Back").started -= Back;
        menu.FindAction("Left").started -= Left;
        menu.FindAction("Right").started -= Right;
    }

    private void Back(InputAction.CallbackContext obj)
    {
        gameObject.transform.parent.GetComponent<MainMenuScript>().ShowMainMenu();
    }
    
    private void Right(InputAction.CallbackContext obj)
    {
        if (volumeSlider.value <= 0.95f)
        {
            volumeSlider.value += 0.05f;
        }else if (volumeSlider.value <= 1)
        {
            volumeSlider.value = 1;
        }
    }

    private void Left(InputAction.CallbackContext obj)
    {
        if (volumeSlider.value >= 0.05f)
        {
            volumeSlider.value -= 0.05f;
        }else if (volumeSlider.value >= 0)
        {
            volumeSlider.value = 0;
        }    
    }

    //Aufgerufen vom Slider in Unity
    public void SetVolume ()
    {
        float volume = volumeSlider.value;
        AudioManager.Instance.SetGlobalVolume(volume);
        PlayerPrefs.SetFloat(VolumeKey, volume);
        PlayerPrefs.Save();
    }
    
    
    
   
    
}
