using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : Fighter
{ 
    private int ID;

    private InputActionAsset inputAsset;
    private InputActionMap player;
    private InputAction move;
    private InputAction sprint;

    private Rigidbody2D rb;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float maxSpeed = 5f;

    private Animator anim;
    private bool isGrounded;

    private int currentDamage = 0;

    [SerializeField]
    private TMP_Text text;

    private TMP_Text health;
    

    private void Awake()
    {
        ID = GameObject.FindGameObjectsWithTag("Player").Length;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        inputAsset = this.GetComponent<PlayerInput>().actions;
        player = inputAsset.FindActionMap("Player");

        health = Instantiate(text);
        Debug.Log(health);
        

        health.transform.SetParent(GameObject.FindWithTag("PlayerDamage").transform);


    }

    private void OnEnable()
    {
        player.FindAction("Jump").started += Jump;
        player.FindAction("Attack").started += Attack;
        player.FindAction("A + Right").started += AttackRight;
        
        move = inputAsset.FindAction("Move");
        sprint = inputAsset.FindAction("Sprint");

        player.Enable();

    }
    

    private void OnDisable()
    {
        player.FindAction("Jump").started -= Jump;
        inputAsset.FindAction("Attack").started -= Attack;
        inputAsset.FindAction("A + Right").started -= AttackRight;

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
            rb.velocity += new Vector2(move.ReadValue<Vector2>().x,0);
        }
        
        if (Math.Abs(rb.velocity.x) < maxSpeed*2)
        {
            rb.velocity += new Vector2(sprint.ReadValue<Vector2>().x,0);
        }
        
        if (rb.velocity.y < 0f )
        {
            rb.velocity += Vector2.down*Physics.gravity.y *Time.fixedDeltaTime;
        }

        


        
        AnimationUpdate();
        DamageTextChange();

    }

    private void DamageTextChange()
    {
        health.GetComponent<TextScript>().GetAnim().SetTrigger("OnHit");
        health.GetComponent<TextScript>().GetAnim().ResetTrigger("OnHit");
        Color newColor = new Color(1, 1 - hitpoint / 999, 1 - hitpoint / 333, 1);
        health.CrossFadeColor(newColor, 0.1f, true, false);
        health.text = hitpoint.ToString() + "%";
    }

    void Jump(InputAction.CallbackContext callbackContext)
    {
        if (IsGrounded())
        {
            rb.velocity += Vector2.up * jumpForce;
        }
    }
    
    private void Attack(InputAction.CallbackContext obj)
    {
        anim.SetTrigger("Attack1");
    }
    
    private void AttackRight(InputAction.CallbackContext obj)
    {
        anim.SetTrigger("Side A");

    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(transform.position, 0.5f, LayerMask.GetMask("Ground"));
    }
    
    void AnimationUpdate()
    {
        if (isGrounded)
        {
            anim.SetTrigger("Grounded");
        }
        else
        {
            anim.ResetTrigger("Grounded");
        }
        
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
        if (coll.tag != "Player")
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

    void UpdateDamage(int dmg)
    {
        currentDamage = dmg;
    }
}

