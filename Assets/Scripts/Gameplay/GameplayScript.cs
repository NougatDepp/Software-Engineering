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
    private bool isInAnimation;
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
            if (rb.velocity.x + Math.Abs(move.ReadValue<Vector2>().x) >= maxSpeed)
            {
                rb.velocity = new Vector2(maxSpeed, rb.velocity.y);
            }
            else if (move.ReadValue<Vector2>().x >= 0)
            {
                rb.velocity += new Vector2(maxSpeed - move.ReadValue<Vector2>().x , 0);
            }else if (move.ReadValue<Vector2>().x <= 0)
            {
                rb.velocity += new Vector2(-maxSpeed + move.ReadValue<Vector2>().x , 0);

            }
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
        
        Debug.Log(rb.velocity.x);

        isMoving = Math.Abs(move.ReadValue<Vector2>().x) >= 0.3f;

        isGrounded = Physics2D.OverlapCircle(transform.position, 0.5f, LayerMask.GetMask("Ground"));

        isFalling = rb.velocity.y < -0.2f;

        anim.SetFloat("WalkingSpeed",Math.Abs(rb.velocity.x));
        
        StateUpdate();
    }
    void StateUpdate() 
    {

        if (!isHurt && !isInAnimation)
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
        if (!isInAnimation)
        {
            oldState = newState;
            anim.Play(newState);
        }
    }
    
    private void ChangeStateOnButtonPressed(string newState)
    {
        if (!isInAnimation||newState == "Jump")
        {
            isInAnimation = true;
            oldState = newState;
            anim.Play(newState);
            Invoke("Delay",0.05f);
        }
    }

    private void Delay()
    {
        float delay = anim.GetCurrentAnimatorStateInfo(0).length;
        Invoke("ResetAttackStatus", delay);
        Debug.Log(delay);
    }

    private void Jump(string newState)
    {
        if (!isInAnimation)
        {
            isJumping = true;
            oldState = newState;
            anim.Play(newState);
        }
    }

    public void ResetAttackStatus()
    {
        isInAnimation = false;
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
            ChangeStateOnButtonPressed("Hurt");
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
            jumpCounter -= 1;

            ChangeStateOnButtonPressed("Jump");
        }
    }
    
    private void DownB(InputAction.CallbackContext obj)
    {
        ChangeStateOnButtonPressed("Down_B");
    }

    private void UpB(InputAction.CallbackContext obj)
    {
        ChangeStateOnButtonPressed("Up_B");
    }

    private void SideB(InputAction.CallbackContext obj)
    {
        ChangeStateOnButtonPressed("Side_B");
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
        ChangeStateOnButtonPressed("Down_A");
    }

    private void UpA(InputAction.CallbackContext obj)
    {
        ChangeStateOnButtonPressed("Up_A");
    }

    private void SideA(InputAction.CallbackContext obj)
    {
        ChangeStateOnButtonPressed("Side_A");
    }

    private void HoldA(InputAction.CallbackContext obj)
    {
       //Attack("Down_A");
    }

    private void Block(InputAction.CallbackContext obj)
    {
        ChangeStateOnButtonPressed("Block");
    }
    
    private void ReAnim(InputAction.CallbackContext obj)
    {
        isInAnimation = false;
        isJumping = false;
        anim.Play("Idle");
    }
    
}

