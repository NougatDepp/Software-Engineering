using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : Fighter
{
    private InputActionAsset inputAsset;
    private InputActionMap player;
    private InputAction move;

    private Rigidbody2D rb;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float maxSpeed = 5f;
    private Vector2 forceDirection = Vector2.zero;


    private Animator anim;
    private bool isGrounded;
    private SpriteRenderer spriteRen;

    private int currentDamage = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRen = GetComponent<SpriteRenderer>();

        inputAsset = this.GetComponent<PlayerInput>().actions;
        player = inputAsset.FindActionMap("Player");

    }

    private void OnEnable()
    {
        player.FindAction("Jump").started += Jump;
        inputAsset.FindAction("Attack").started += Attack;
        move = inputAsset.FindAction("Move");
        player.Enable();

    }

    private void OnDisable()
    {
        player.FindAction("Jump").started -= Jump;
        inputAsset.FindAction("Attack").started -= Attack;
        player.Disable();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(transform.position, 0.5f, LayerMask.GetMask("Ground"));
    }

    void FixedUpdate()
    {

        forceDirection += new Vector2(move.ReadValue<Vector2>().x,0);
        rb.AddForce(forceDirection,ForceMode2D.Impulse);
        
        forceDirection = Vector2.zero;

        if (rb.velocity.y < 0f)
        {
            rb.velocity += Vector2.down*Physics.gravity.y *Time.fixedDeltaTime;
        }

        Vector2 horizontalVelocity = rb.velocity;
        horizontalVelocity.y = 0;
        if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed)
            rb.velocity = horizontalVelocity.normalized * maxSpeed + Vector2.up * rb.velocity.y;

        if (isGrounded)
        {
            anim.SetTrigger("Grounded");
        }
        else
        {
            anim.ResetTrigger("Grounded");
        }
        
        AnimationUpdate();
    }

    void Jump(InputAction.CallbackContext callbackContext)
    {
        if (IsGrounded())
        {
            forceDirection += Vector2.up * jumpForce;
        }
    }
    
    private void Attack(InputAction.CallbackContext obj)
    {
        anim.SetTrigger("Attack1");
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(transform.position, 0.5f, LayerMask.GetMask("Ground"));
    }

    void Attack1()
    {
        

    }

    void AnimationUpdate()
    {
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

    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.tag == "DamageBox")
        {
            return;
        }


        Damage dmg = new Damage()
        {
            damageAmount = currentDamage,
            origin = transform.position,
            pushForce = 1.5f
        };
            
        coll.SendMessage("ReceiveDamage",dmg); 
    }

    void UpdateOnIdle()
    {
        anim.SetInteger("AttackState",0);
    }

    void UpdateDamage(int dmg)
    {
        currentDamage = dmg;
    }
}

