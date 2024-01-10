using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour
{
     public void Changeeee()
     {
          SceneManager.LoadScene("FirstMap");
     }
     
     public void Changeeee2()
     {
          SceneManager.LoadScene("CharacterSelect");
     }
}
