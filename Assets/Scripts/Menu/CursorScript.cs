using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CursorScript : MonoBehaviour
{
    private InputActionAsset inputAsset;
    private InputActionMap menu;
    private InputAction move;


    //Cursor Detection
    private GameObject ui_canvas;
    private GraphicRaycaster gr;
    private PointerEventData pointerEventData = new PointerEventData(null);


    public Transform currentCharacter;
    public bool ready;

    public GameObject tokenPrefab;
    public Transform token;
    private bool hasToken;

    private AudioManager src;

    private int id;

    private void Start()
    {
        CharacterMenuScript.instance.ShowCharacterInSlot(id, null);
    }

    private void OnEnable()
    {

        src = AudioManager.Instance;
        inputAsset = gameObject.transform.parent.GetComponent<PlayerInput>().actions;
        menu = inputAsset.FindActionMap("Menu");
        
        ui_canvas = GameObject.FindGameObjectWithTag("MenuCanvas");
        gr = ui_canvas.GetComponent<GraphicRaycaster>();
        token = Instantiate(tokenPrefab, transform.position, Quaternion.identity, GameObject.FindWithTag("MenuCanvas").transform).transform;
        hasToken = true;

        menu.FindAction("Choose").started += Choose;
        menu.FindAction("Back").started += Back;
        menu.FindAction("Go").started += Go;


        move = inputAsset.FindAction("Move");

        menu.Enable();
    }

    private void OnDisable()
    {
        menu.FindAction("Choose").started -= Choose;
        menu.FindAction("Back").started -= Back;
        menu.FindAction("Go").started -= Go;
        
    }

    private void FixedUpdate()
    {
        if (SceneManager.GetActiveScene().name != "CharacterSelect") gameObject.SetActive(false);
        transform.position += new Vector3(move.ReadValue<Vector2>().x, move.ReadValue<Vector2>().y, 0) * 0.3f;
    }

    void Update()
    {
        transform.position += new Vector3(move.ReadValue<Vector2>().x, move.ReadValue<Vector2>().y, 0) * 0.3f * Time.deltaTime;

        if (hasToken)
        {
            token.position = transform.position;
        }

        pointerEventData.position = Camera.main.WorldToScreenPoint(transform.position);
        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(pointerEventData, results);

        if (hasToken)
        {
            if (results.Count > 0)
            {
                Transform raycastCharacter = results[0].gameObject.transform;

                if (raycastCharacter != currentCharacter)
                {
                    SetCurrentCharacter(raycastCharacter);
                    src.PlaySound(src.clickSound);
                }
            }
            else
            {
                if (currentCharacter != null)
                {
                    SetCurrentCharacter(null);
                    src.PlaySound(src.clickSound);
                }
            }
        }
    }

    void SetCurrentCharacter(Transform t)
    {
        currentCharacter = t;

        if (t != null)
        {
            int index = t.GetSiblingIndex();
            Character character = CharacterMenuScript.instance.characters[index];
            CharacterMenuScript.instance.ShowCharacterInSlot(id, character);
        }
        else
        {
            CharacterMenuScript.instance.ShowCharacterInSlot(id, null);
        }
    }

    void TokenFollow(bool trigger)
    {
        hasToken = trigger;
    }

    private void SetChoosenCharacter()
    {
        gameObject.transform.parent.GetComponent<PlayerScript>()
            .SetCharacter(CharacterMenuScript.instance.characters[currentCharacter.GetSiblingIndex()]);
    }

    private void RemoveCharacter()
    {
        gameObject.transform.parent.GetComponent<PlayerScript>()
            .SetCharacter(null);
    }
    public void SetID(int id)
    {
        this.id = id;
    }

    private void Go(InputAction.CallbackContext obj)
    {
        src.PlaySound(src.gameStart);
        GameManager.instance.LoadStartGame();
    }

    private void Back(InputAction.CallbackContext context)
    {
        src.PlaySound(src.buttonBack);
        if (hasToken)
        {
            GameManager.instance.LoadMainMenu();
            hasToken = false;
            return;
        }
        CharacterMenuScript.instance.confirmedCharacter = null;
        TokenFollow(true);
        RemoveCharacter();
        ready = false;
    }

    private void Choose(InputAction.CallbackContext context)
    {
        if (currentCharacter != null)
        {
            src.PlaySound(src.buttonSelect);

            TokenFollow(false);
            CharacterMenuScript.instance.ConfirmCharacter(id, CharacterMenuScript.instance.characters[currentCharacter.GetSiblingIndex()]);
            SetChoosenCharacter();
            ready = true;
        }
    }
}
