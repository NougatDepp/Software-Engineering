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
    
    private Rigidbody2D rb;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float maxSpeed = 10f;

    private Animator anim;
    private bool isGrounded;

    [SerializeField]
    private int currentDamage = 0;

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

    public bool isHurt;

    

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        ID = GameObject.FindGameObjectsWithTag("Player").Length;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        inputAsset = gameObject.transform.parent.GetComponent<PlayerInput>().actions;
        player = inputAsset.FindActionMap("Player");

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
    }

    void FixedUpdate()
    {
        if (Math.Abs(rb.velocity.x) < maxSpeed && currentState < 4)
        {
            rb.velocity = new Vector2(move.ReadValue<Vector2>().x*maxSpeed,rb.velocity.y);
        }else rb.velocity = new Vector2(rb.velocity.x*0.9f,rb.velocity.y);

        if (rb.velocity.y < 0f )
        {
            rb.velocity += Vector2.down*Physics.gravity.y *Time.fixedDeltaTime;
        }
        
        if (isGrounded)
        { 
            jumpCounter = 2;
        }
        
        isGrounded = Physics2D.OverlapCircle(transform.position, 0.5f, LayerMask.GetMask("Ground"));

        
        AnimationUpdate();
        
        StateUpdate();
    }
    void StateUpdate()
    {
        if (currentState < 4)
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
        }
        anim.SetInteger("AnimState",currentState);
    }

    void AnimationUpdate()
    {
        anim.SetBool("Grounded",isGrounded);
        
        anim.SetFloat("AirSpeedY",rb.velocity.y);
        if (move.ReadValue<Vector2>().x > 0)
        {
            gameObject.transform.localScale = new Vector3(2.5f,2.5f,0);
            anim.SetInteger("AnimState", 1);
        }
        else if (move.ReadValue<Vector2>().x < 0)
        {
            gameObject.transform.localScale = new Vector3(-2.5f,2.5f,0);

            anim.SetInteger("AnimState", 1);
        }
        
        anim.SetFloat("WalkingSpeed",Math.Abs(rb.velocity.x));

    }

    

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.tag != "Character")
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

    private void TriggerAttack(string attack)
    {
        currentState = 4;
        anim.Play("Side_A");
    }

    private void ResetCurrentState()
    {
        currentState = 0;
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
        TriggerAttack("Down_B");
    }

    private void UpB(InputAction.CallbackContext obj)
    {
        TriggerAttack("Up_B");
    }

    private void SideB(InputAction.CallbackContext obj)
    {
        TriggerAttack("Side_B");

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
        TriggerAttack("Down_A");
    }

    private void UpA(InputAction.CallbackContext obj)
    {
        TriggerAttack("Up_A");
    }

    private void SideA(InputAction.CallbackContext obj)
    {
        TriggerAttack("Side_A");
    }

    private void HoldA(InputAction.CallbackContext obj)
    {
        //TriggerAttack("Down A");
    }
    
    
}

