using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float moveSpeed = 5f; // Geschwindigkeit des Spielers
    public float jumpForce = 10f; // Sprungkraft des Spielers

    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Überprüfen, ob der Spieler auf dem Boden steht
        isGrounded = Physics2D.OverlapCircle(transform.position, 2f, LayerMask.GetMask("Ground"));

        // Spieler springt, wenn die Leertaste gedrückt wird und er sich auf dem Boden befindet
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        // Spielerbewegung
        float horizontalInput = Input.GetAxis("Horizontal");
        Move(horizontalInput);

        // Bodenprüfung für Animationen oder andere Funktionen
        // Du könntest hier beispielsweise eine Animation auslösen, wenn der Spieler sich bewegt, etc.
        if (isGrounded)
        {
            // Hier könntest du weitere Funktionen auslösen, die nur dann aktiv sein sollen, wenn der Spieler am Boden ist.
        }
    }

    void Move(float horizontal)
    {
        // Spielerbewegung
        Vector2 movement = new Vector2(horizontal * moveSpeed, rb.velocity.y);
        rb.velocity = movement;
    }

    void Jump()
    {
        // Spieler springt
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }
}

