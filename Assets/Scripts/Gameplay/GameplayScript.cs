using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameplayScript : Fighter
{ 
    private int ID;

    private InputActionAsset inputAsset;
    private InputActionMap player;
    private InputAction move;

    private InputAction sprint;

    private Rigidbody2D rb;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float maxSpeed = 10f;

    private Animator anim;
    private bool isGrounded;

    private int currentDamage = 0;

    [SerializeField]
    private TMP_Text text;
    private TMP_Text health;

    public AudioSource src;
    public AudioClip sfx;

    
    /**
     * 0 = Idle
     * 1 = Walking
     * 2 = Jumping
     * 3 = Falling
     * 4 = Hurt
    **/
    
    private int currentState = 0;

    public int jumpCounter = 0;

    

    private void Awake()
    {
        ID = GameObject.FindGameObjectsWithTag("Player").Length;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        inputAsset = this.GetComponent<PlayerInput>().actions;
        player = inputAsset.FindActionMap("Player");

        health = Instantiate(text);

        health.transform.SetParent(GameObject.FindWithTag("PlayerDamage").transform);
    }

    private void OnEnable()
    {
        player.FindAction("Jump").started += Jump;
        
        player.FindAction("Hold A").started += HoldA;
        player.FindAction("Side A").started += SideA;
        player.FindAction("Up A").started += UpA;
        player.FindAction("Down A").started += DownA;
        
        player.FindAction("Hold B").performed += HoldBStart;
        player.FindAction("Hold B").canceled += HoldBEnd;

        player.FindAction("Side B").started += SideB;
        player.FindAction("Up B").started += UpB;
        player.FindAction("Down B").started += DownB;


        move = inputAsset.FindAction("Move");
        sprint = inputAsset.FindAction("Sprint");

        player.Enable();

    }

    

    private void OnDisable()
    {
        player.FindAction("Jump").started -= Jump;
        
        player.FindAction("Hold A").started -= HoldA;
        player.FindAction("Hold A").performed -= HoldA;

        
        player.FindAction("Side A").started -= SideA;
        player.FindAction("Up A").started -= UpA;
        player.FindAction("Down A").started -= DownA;
        
        player.FindAction("Hold B").started -= HoldBStart;
        player.FindAction("Hold B").performed -= HoldBEnd;

        
        player.FindAction("Side B").started -= SideB;
        player.FindAction("Up B").started -= UpB;
        player.FindAction("Down B").started -= DownB;
        player.Disable();
    }

    
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(transform.position, 0.5f, LayerMask.GetMask("Ground")); 
    }

    void FixedUpdate()
    {
        if (Math.Abs(rb.velocity.x) < maxSpeed)
        {
            rb.velocity = new Vector2(move.ReadValue<Vector2>().x*maxSpeed,rb.velocity.y);
        }

        if (rb.velocity.y < 0f )
        {
            rb.velocity += Vector2.down*Physics.gravity.y *Time.fixedDeltaTime;
        }
        
        AnimationUpdate();
        DamageTextChange();
        
        StateUpdate();
    }
    void StateUpdate()
    {
        if (rb.velocity.y < -0.2f)
        {
            currentState = 3; // Falling
        }
        else if (rb.velocity.y > 0.2f)
        {
            currentState = 2; // Jumping
        }
        else if (rb.velocity.magnitude >= 0.2f)
        {
            currentState = 1; //  Walking
        }
        else
        {
            currentState = 0; // Idle
        }

        if (isGrounded)
        {
            jumpCounter = 2;
        }
        
        anim.SetInteger("AnimState",currentState);
    }

    private void DamageTextChange()
    {
        health.GetComponent<TextScript>().GetAnim().SetTrigger("OnHit");
        Color newColor = new Color(1, 1 - hitpoint / 999, 1 - hitpoint / 333, 1);
        health.CrossFadeColor(newColor, 0.1f, true, false);
        health.text = hitpoint.ToString() + "%";
    }

    
    

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(transform.position, 0.5f, LayerMask.GetMask("Ground"));
    }
    
    void AnimationUpdate()
    {
        anim.SetBool("Grounded",isGrounded);
        
        anim.SetFloat("AirSpeedY",rb.velocity.y);
        if (Mathf.Floor(rb.velocity.x) > 0)
        {
            gameObject.transform.localScale = new Vector3(2.5f,2.5f,0);
            anim.SetInteger("AnimState", 1);
        }
        else if (Mathf.Floor(rb.velocity.x) < 0)
        {
            gameObject.transform.localScale = new Vector3(-2.5f,2.5f,0);

            anim.SetInteger("AnimState", 1);
        }
        else
        {
            anim.SetInteger("AnimState", 2);
        }
        
        anim.SetFloat("WalkingSpeed",Math.Abs(rb.velocity.x));

    }

    

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.tag != "Player")
        {
            return;
        }

        src.clip = sfx;
        src.Play();

        Damage dmg = new Damage()
        {
            damageAmount = currentDamage,
            origin = transform.position,
            pushForce = 1.5f
        };
            
        coll.SendMessage("ReceiveDamage",dmg); 
    }

    void UpdateDamage(int dmg)
    {
        currentDamage = dmg;
    }
    
    /**
     * Hier kommen alle Inputs des Controllers.
     */
    
    
    void Jump(InputAction.CallbackContext callbackContext)
    {
        if (isGrounded || jumpCounter > 1)
        {
            rb.velocity = new Vector2(rb.velocity.x ,Vector2.up.y * jumpForce);
            jumpCounter -= 1;
            anim.SetInteger("JumpCounter",jumpCounter);
            anim.SetTrigger("Jump");
        }
    }
    
    private void DownB(InputAction.CallbackContext obj)
    {
        anim.SetTrigger("Down B");
    }

    private void UpB(InputAction.CallbackContext obj)
    {
        anim.SetTrigger("Up B");
    }

    private void SideB(InputAction.CallbackContext obj)
    {
        anim.SetTrigger("Side B");

    }
    
    private void HoldBEnd(InputAction.CallbackContext obj)
    {
        anim.SetBool("Hold B", false);
    }

    private void HoldBStart(InputAction.CallbackContext obj)
    {
        anim.SetBool("Hold B",true);
        anim.SetTrigger("B");
    }

    private void DownA(InputAction.CallbackContext obj)
    {
        anim.SetTrigger("Down A");
    }

    private void UpA(InputAction.CallbackContext obj)
    {
        anim.SetTrigger("Down A");
    }

    private void SideA(InputAction.CallbackContext obj)
    {
        anim.SetTrigger("Down A");
    }

    private void HoldA(InputAction.CallbackContext obj)
    {
        anim.SetTrigger("Down A");
    }
    
    
}

