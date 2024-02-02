using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class PlayerTests :InputTestFixture
{
    private GameObject testObject;
    private GameObject groundObject;
    private GameObject managerObject;
    private Camera mainCamera;
    private Gamepad gamepad;

    
    [SetUp]
    public void PhysicsTestSetUp()
    {
        if (SceneManager.GetActiveScene().name != "TestingScene")
        {
            SceneManager.LoadSceneAsync("TestingScene");
            
        }
        testObject = GameObject.FindWithTag("Character");
    }

    
    [UnityTest]
    
    public IEnumerator Test1PlayerJoin()
    {
        gamepad = InputSystem.AddDevice<Gamepad>();
        Press(gamepad.startButton);
        yield return new WaitForSeconds(0.5f);
        Assert.AreEqual(1, GameObject.FindGameObjectsWithTag("Character").Length);
        yield return null;
    }
    
    [UnityTest]
    public IEnumerator Test2PhysicsTestFalling()
    {
        yield return new WaitForSeconds(0.5f);

        testObject = GameObject.FindWithTag("Character");

        yield return new WaitForSeconds(1.5f);
        Assert.AreEqual("Falling",testObject.GetComponent<GameplayScript>().activeState);
        yield return null;
    }
    
    [UnityTest]
    public IEnumerator Test3PhysicsTestGrounded()
    {
        testObject.transform.position = new Vector3(7, 0, 0);
        yield return new WaitForSeconds(0.5f);
        Assert.AreEqual("Idle",testObject.GetComponent<GameplayScript>().activeState);
        yield return null;
    }
    
   
    
}
