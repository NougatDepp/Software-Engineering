using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorDetection : MonoBehaviour
{
    private GameObject ui_canvas;
    private GraphicRaycaster gr;
    private PointerEventData pointerEventData = new PointerEventData(null);

    public Transform currentCharacter;

    public Transform token;
    public bool hasToken;
    private void Start()
    {
        ui_canvas = GameObject.FindGameObjectWithTag("MenuCanvas");
        gr = ui_canvas.GetComponent<GraphicRaycaster>();
    }

    private void Update()
    {
        pointerEventData.position = Camera.main.WorldToScreenPoint(transform.position);
        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(pointerEventData, results);

        

        if (results.Count > 0)
        {
            SetCurrentCharacter(results[0].gameObject.transform);
            Debug.Log(results[0].gameObject.transform.GetSiblingIndex());
        }
        else
        {
            currentCharacter = null;
        }

        if (hasToken)
        {
            token.position = transform.position;
        }
        
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
                CharacterMenuScript.instance.ShowCharacterInSlot(0, character);
            }
            else
            {
                CharacterMenuScript.instance.ShowCharacterInSlot(0, null);
            }
        }
    
        void TokenFollow (bool trigger)
        {
            hasToken = trigger;
        }
}
