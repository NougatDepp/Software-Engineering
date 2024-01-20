using System; 
using System.Collections;
using System.Collections.Generic;
using Microsoft.Win32.SafeHandles;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class GameplayScript : Fighter
{ 

    private InputActionAsset inputAsset;
    private InputActionMap player;
    private InputAction move;

    public Rigidbody2D rb;
    
    [SerializeField] public float jumpForce = 5f;
    [SerializeField] private float maxSpeed = 10f;

    private Animator anim;
    private SpriteRenderer spriteRenderer;
    
    [SerializeField] public bool isGrounded;
    private bool isMoving;
    private bool isAirborne;
    private bool isInAnimation;
    private bool isHurt;
    
    [SerializeField]
    private bool isBlocking;

    [SerializeField]
    private int currentDamage = 0;
    
    [SerializeField]
    private int playerLives = 0;

    //Aktiver und Alter State
    [SerializeField]
    public string activeState = "Idle";
    [SerializeField]
    public string oldState = "Idle";
    
    //States in denen sich der Player befindet
    private const string PLAYER_IDLE = "Idle";
    private const string PLAYER_WALKING = "Walk";
    private const string PLAYER_FALLING = "Falling";

    public int jumpCounter;
    
    
    //Effekte beim Blocken, Schlagen und beim Knockout
    [SerializeField]
    private GameObject hitEffect;
    
    [SerializeField]
    private GameObject blockEffect;
    
    [SerializeField]
    private GameObject deathEffect;
    
    //Audio
    private AudioManager src;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        src = AudioManager.Instance;
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
        
        player.Disable();
    }

    
    void FixedUpdate()
    {

        if (isGrounded && !isInAnimation)
        {
            if (0.2f < Math.Abs(move.ReadValue<Vector2>().x))
            {
                rb.velocity = new Vector2(move.ReadValue<Vector2>().x * maxSpeed, rb.velocity.y);
            }
        }
        else if(rb.velocity.x < maxSpeed)
        {
            rb.velocity += new Vector2(move.ReadValue<Vector2>().x/maxSpeed*2 , 0);
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

        isAirborne = !isGrounded;
        
        anim.SetFloat("WalkingSpeed",Math.Abs(rb.velocity.x));
        
        StateUpdate();
    }
    void StateUpdate() 
    {

        if (!isHurt && !isInAnimation)
        {
            if (isAirborne)
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

        if (coll.tag == "DeathZone")
        {
            src.PlaySound(src.knockout);

            StartCoroutine(OnDeath());
            return;
        } 
        
        if (coll.tag != "Character")
        {
            return;
        }
        src.PlaySound(src.attacks[Random.Range(0,src.attacks.Length-1)]);

        Instantiate(hitEffect,
            gameObject.transform.Find("DamageBox").transform.position,Quaternion.identity);
        
        Damage dmg = new Damage()
        {
            damageAmount = currentDamage,
            origin = transform.position,
            pushForce = 1.5f
        };
            
        coll.SendMessage("ReceiveDamage",dmg); 
    }

    private IEnumerator OnDeath()
    {
        playerLives -= 1;
        if (playerLives <= 0)
        {
            gameObject.transform.parent.GetComponent<PlayerScript>().PlacementUpdater();
            Destroy(gameObject);
        }
        else
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            spriteRenderer.enabled = false;
            hitpoint = 0;
            transform.position = GameObject.FindWithTag("Respawn").transform.position;
            rb.bodyType = RigidbodyType2D.Static;

            yield return new WaitForSeconds(3);
            
            anim.Play("Spawn");
            
            
            spriteRenderer.enabled = true;
            
            yield return new WaitForSeconds(.3f);
            
            rb.bodyType = RigidbodyType2D.Dynamic;

        }
    }

    protected override void ReceiveDamage(Damage dmg)
    {
        if (isBlocking)
        {
            Instantiate(blockEffect, transform);
            return;
        }
        
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
    public void Jump(InputAction.CallbackContext callbackContext)
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
}

