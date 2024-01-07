using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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

    public GameObject tokenPrefab;
    public Transform token;
    private bool hasToken;

    private int id;

    private void Awake(){

        inputAsset = this.GetComponent<PlayerInput>().actions;
        menu = inputAsset.FindActionMap("Menu");
    }
    
    private void Start()
    {
        ui_canvas = GameObject.FindGameObjectWithTag("MenuCanvas");
        gr = ui_canvas.GetComponent<GraphicRaycaster>();
        
        hasToken = true;
        token = Instantiate(tokenPrefab, GameObject.FindWithTag("MenuCanvas").transform).transform;
        CharacterMenuScript.instance.ShowCharacterInSlot(id, null);
    }

    void Update () {
        transform.position += new Vector3(move.ReadValue<Vector2>().x, move.ReadValue<Vector2>().y, 0)*0.3f*Time.deltaTime;

        if (hasToken)
        { 
            token.position = transform.position;
        }
        
        pointerEventData.position = Camera.main.WorldToScreenPoint(transform.position);
        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(pointerEventData,results);
        
        if (hasToken)
        {
            if (results.Count > 0)
            {
                Transform raycastCharacter = results[0].gameObject.transform;

                if (raycastCharacter != currentCharacter)
                {
                    SetCurrentCharacter(raycastCharacter);
                }
            }
            else
            {
                if (currentCharacter != null)
                {
                    SetCurrentCharacter(null);
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
    
   void TokenFollow (bool trigger)
   {
       hasToken = trigger;
   }

    

    

    private void OnEnable(){

        menu.FindAction("Choose").started += Choose;
        menu.FindAction("Back").started += Back;

        move = inputAsset.FindAction("Move");
    }

    private void OnDisable()
    {
        menu.FindAction("Choose").started -= Choose;
        menu.FindAction("Back").started -= Back;
    }

    private void FixedUpdate()
    {
        transform.position += new Vector3(move.ReadValue<Vector2>().x, move.ReadValue<Vector2>().y, 0)*0.3f;
    }

    private void Back(InputAction.CallbackContext context)
    {
        CharacterMenuScript.instance.confirmedCharacter = null;
        TokenFollow(true);
    }

    private void Choose(InputAction.CallbackContext context)
    {
        if (currentCharacter != null)
        {
            TokenFollow(false);
            CharacterMenuScript.instance.ConfirmCharacter(CharacterMenuScript.instance.characters[currentCharacter.GetSiblingIndex()]);
        }
    }

    public void SetID(int id)
    {
        this.id = id;
    }
}
