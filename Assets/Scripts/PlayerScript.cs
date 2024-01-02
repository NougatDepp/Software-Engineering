using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    public float moveSpeed = 5f; 
    public float jumpForce = 10f; 

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private SpriteRenderer spriteRen;
    private BoxCollider2D boxCollider;


    private PlayerControls controls;


    private void Awake()
    {
        controls = new PlayerControls();
        controls.Gameplay.Jump.performed += ctx => Jump();
        controls.Gameplay.Attack1.performed += ctx => Attack1();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRen = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(transform.position, 0.5f, LayerMask.GetMask("Ground"));
    }

    void FixedUpdate()
    {

        float horizontalInput = Input.GetAxis("Horizontal");
        Move(horizontalInput);
        
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

    void Move(float horizontal)
    {
        Vector2 movement = new Vector2(horizontal * moveSpeed, rb.velocity.y);
        rb.velocity = movement;
    }

    void Jump()
    {
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            anim.SetTrigger("Jump");
        }
    }

    void Attack1()
    {
        anim.SetInteger("AttackState",1);
        anim.SetTrigger("Attack1");

    }

    void AnimationUpdate()
    {
        anim.SetFloat("AirSpeedY",rb.velocity.y);
        if (Mathf.Floor(rb.velocity.x) > 0)
        {
            spriteRen.flipX = false;
            anim.SetInteger("AnimState", 1);
        }
        else if (Mathf.Floor(rb.velocity.x) < 0)
        {
            spriteRen.flipX = true;
            anim.SetInteger("AnimState", 1);
        }
        else
        {
            anim.SetInteger("AnimState", 2);
        }

    }

    void UpdateOnIdle()
    {
        anim.SetInteger("AttackState",0);
    }
}

