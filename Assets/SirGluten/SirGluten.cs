using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

public class SirGluten : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private float verticalInput, horizontalInput;
    private float speed = 5f;
    private Rigidbody2D body;
    

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

    }


    void Update() {

        // Movement
        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalInput = Input.GetAxisRaw("Horizontal");


        MovePlayer();    

        // ANimations
    
        animator.SetFloat("horizontal", Mathf.Abs(body.linearVelocity.x));
        animator.SetFloat("vertical", body.linearVelocity.y);
        animator.SetBool("isMoving", body.linearVelocity.x != 0 || body.linearVelocity.y != 0);

    }

    void MovePlayer(){
        body.linearVelocity = new Vector2(horizontalInput,verticalInput).normalized * speed;
        
        if (horizontalInput < 0) {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        } else if (horizontalInput > 0) {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        if (verticalInput != 0 && horizontalInput == 0) {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

    }
}