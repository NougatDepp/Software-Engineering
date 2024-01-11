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
    
    [SerializeField]
    private bool isGrounded;
    private bool isMoving;
    private bool isFalling;
    private bool isJumping;
    private bool isAttacking;
    private bool isHurt;
    
    [SerializeField]
    private bool isBlocking;

    [SerializeField]
    private int currentDamage = 0;

    public AudioSource src;
    public AudioClip sfx;
    
    [SerializeField]
    public string activeState = "Idle";
    [SerializeField]
    public string oldState = "Idle";
    
    
    private const string PLAYER_IDLE = "Idle";
    private const string PLAYER_WALKING = "Walk";
    private const string PLAYER_FALLING = "Falling";
    private const string PLAYER_HURT = "Hurt";
    
    private int jumpCounter;

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
        
        player.FindAction("Block").started += Block;
        player.FindAction("ResetAnimator").performed += ReAnim;




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
        
        player.FindAction("Block").started -= Block;
        player.FindAction("ResetAnimator").performed -= ReAnim;
        player.Disable();
    }


    void FixedUpdate()
    {
        if (Math.Abs(rb.velocity.x) < maxSpeed && 0.2f < Math.Abs(move.ReadValue<Vector2>().x))
        {
            rb.velocity += new Vector2(move.ReadValue<Vector2>().x*maxSpeed,0);
        }
        else if (!isHurt)rb.velocity -= new Vector2(rb.velocity.x*0.3f,0);

        if (isAttacking&&isGrounded)
        {
            rb.velocity = new Vector2(0,rb.velocity.y);
        }

        if (rb.velocity.y < 0f )
        {
            rb.velocity += Vector2.down*Physics.gravity.y *Time.fixedDeltaTime;
        }
        
        if (isGrounded)
        { 
            jumpCounter = 2;
        }

        if (rb.velocity.x > 0.3f)
        {
            gameObject.transform.localScale = new Vector2(2.5f, 2.5f);
        }
        else if (rb.velocity.x < -0.3f)
        {
            gameObject.transform.localScale = new Vector2(-2.5f, 2.5f);
        }




        isMoving = Math.Abs(move.ReadValue<Vector2>().x) >= 0.3f;

        isGrounded = Physics2D.OverlapCircle(transform.position, 0.5f, LayerMask.GetMask("Ground"));

        isFalling = rb.velocity.y < -0.2f;

        anim.SetFloat("WalkingSpeed",Math.Abs(rb.velocity.x));
        
        StateUpdate();
    }
    void StateUpdate() 
    {

        if (!isHurt && !isAttacking)
        {
            if (isFalling)
            {
                activeState = PLAYER_FALLING;
            }
            else if (isMoving && isGrounded)
            {
                activeState = PLAYER_WALKING;
            }
            else 
            { 
                activeState = PLAYER_IDLE;
            }
            ChangeCurrentState(activeState);
        }
        
    }

    private void ChangeCurrentState(string newState)
    {
        if (oldState == newState) return;
        if (!isAttacking&&!isJumping)
        {
            oldState = newState;
            anim.Play(newState);
        }
    }
    
    private void Attack(string newState)
    {
        if (!isAttacking&&!isJumping)
        {
            isAttacking = true;
            oldState = newState;
            anim.Play(newState);
        }
    }

    private void Jump(string newState)
    {
        if (!isAttacking)
        {
            isJumping = true;
            oldState = newState;
            anim.Play(newState);
        }
    }

    //Diese drei Methoden werden vom Animator aufgerufen.
    public void SetAttackStatus()
    {
        isAttacking = false;
    }
    
    public void SetJumpStatus()
    {
        isJumping = false;
    }
    
    public void SetHurtStatus()
    {
        isHurt = false;
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

    protected override void ReceiveDamage(Damage dmg)
    {
        if(isBlocking) return;
        
        base.ReceiveDamage(dmg);
        if (((pushDirection+new Vector2(0,0.3f))*hitpoint/3).magnitude >= 10)
        {
            isHurt = true;
            Attack("Hurt");
            Invoke("SetHurtStatus",0.3f);
            Invoke("SetAttackStatus",0.3f);
        }
    }

    /**
     * Hier kommen alle Inputs des Controllers.
     */
    
    void Jump(InputAction.CallbackContext callbackContext)
    {
        if (isGrounded || jumpCounter > 1)
        {
            rb.velocity = new Vector2(rb.velocity.x ,Vector2.up.y * jumpForce);
            Debug.Log(jumpCounter);
            jumpCounter -= 1;
            Debug.Log(jumpCounter);

            Jump("Jump");
        }
    }
    
    private void DownB(InputAction.CallbackContext obj)
    {
        Attack("Down_B");
    }

    private void UpB(InputAction.CallbackContext obj)
    {
        Attack("Up_B");
    }

    private void SideB(InputAction.CallbackContext obj)
    {
        Attack("Side_B");
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
        Attack("Down_A");
    }

    private void UpA(InputAction.CallbackContext obj)
    {
        Attack("Up_A");
    }

    private void SideA(InputAction.CallbackContext obj)
    {
        Attack("Side_A");
    }

    private void HoldA(InputAction.CallbackContext obj)
    {
       //Attack("Down_A");
    }

    private void Block(InputAction.CallbackContext obj)
    {
        Attack("Block");
    }
    
    private void ReAnim(InputAction.CallbackContext obj)
    {
        isAttacking = false;
        isJumping = false;
        anim.Play("Idle");
    }
    
}

