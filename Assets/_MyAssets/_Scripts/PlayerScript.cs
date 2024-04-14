using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum CharacterState
{
    Idling,
    Running,
    Rolling,
    Jumping,
    Dead
}
public class PlayerScript : MonoBehaviour
{
    [SerializeField] private Transform groundDetect;
    [SerializeField] private bool isGrounded; // Just so we can see in Editor.
    [SerializeField] private float moveForce;
    [SerializeField] private float jumpForce;
    public LayerMask groundLayer;
    private float groundCheckWidth = 2f;
    private float groundCheckHeight = 0.25f;
    private Animator an;
    private Rigidbody2D rb;
    private CapsuleCollider2D cc;

    [SerializeField] GameObject[] health;
    public int hits = 0;
    public bool isAlive = true;

    private CharacterState currentState;
    private bool jumpStarted;

    private bool invulnerability = false;

    void Start()
    {
        an = GetComponentInChildren<Animator>();
        isGrounded = false; // Always start in air.
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CapsuleCollider2D>();
        StartJump();
    }

    void Update()
    {
        if (!jumpStarted)
        {
            GroundedCheck();
        }
        
        switch (currentState)
        {
            case CharacterState.Idling:
                HandleIdlingState(); 
                break;
            case CharacterState.Running:
                HandleRunningState();
                break;
            case CharacterState.Rolling:
                HandleRollingState();
                break;
            case CharacterState.Jumping:
                HandleJumpingState();
                break;
            case CharacterState.Dead:
                HandleDeathState();
                break;
        }
        if (hits == 3)
        {
            isAlive = false;
        }
        if (invulnerability == true)
        {
            SetObstacleColliders(true);
            SetPlayerVisibility(0.2f);
        }
        else if (invulnerability == false)
        {
            SetObstacleColliders(false);
            SetPlayerVisibility(1f);
        }
    }

    void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
        if (isAlive == false) return;
        if (invulnerability == false && collision.gameObject.tag == "Obstacle")
        {
            Game.Instance.SOMA.PlaySound("Thump");
            health[hits].SetActive(false);
            hits++;
            invulnerability = true;
            Invoke("Vulnerable", 10);
        }
        Debug.Log(hits);
    }

    void Vulnerable()
    {
        invulnerability = false;
    }

    void SetPlayerVisibility(float invis)
    {
        Color color = GetComponentInChildren<SpriteRenderer>().color;
        color.a = invis;
        GetComponentInChildren<SpriteRenderer>().color = color;
    }

    void SetObstacleColliders(bool si)
    {
        GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (GameObject obstacle in obstacles)
        {
            PolygonCollider2D obstacleCollider = obstacle.GetComponent<PolygonCollider2D>();
            if (obstacleCollider != null)
            {
                obstacleCollider.isTrigger = si;
                obstacleCollider.enabled = !si;
            }
        }
    }

    private void HandleIdlingState()
    {

        // State logic here.
        transform.Translate(new Vector3(-4f * Time.deltaTime, 0f, 0f));
        if (isGrounded && isAlive == false)
        {
            an.SetBool("isDead", true);
            currentState = CharacterState.Dead;
            Invoke("GameOver", 1.5f);

        }
        else if (isGrounded && (Input.GetAxis("Horizontal") != 0)) // To Running.
        {
            an.SetBool("isMoving", true);
            currentState = CharacterState.Running;
        }
        else if (isGrounded && Input.GetKeyDown(KeyCode.S)) // To Rolling.
        {
            cc.offset = new Vector2(0.33f, -1f);
            cc.size = new Vector2(2f, 2f);
            Game.Instance.SOMA.PlayLoopedSound("Roll");
            an.SetBool("isRolling", true);
            currentState = CharacterState.Rolling;
        }
        else if (isGrounded && Input.GetButtonDown("Jump")) // To Jumping.
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce * Time.fixedDeltaTime);
            Game.Instance.SOMA.PlaySound("Jump");
            StartJump();
        }
    }

    private void HandleRunningState()
    {
        // State logic here.
        MoveCharacter();
        // Transistions.
        if(isGrounded && (Input.GetAxis("Horizontal") == 0)) // To Idling.
        {
            an.SetBool("isMoving", false);
            currentState = CharacterState.Idling;
        }
        else if (isGrounded && Input.GetKeyDown(KeyCode.S)) // To Rolling.
        {
            cc.offset = new Vector2(0.33f, -1f);
            cc.size = new Vector2(2f, 2f);
            Game.Instance.SOMA.PlayLoopedSound("Roll");
            an.SetBool("isRolling", true);
            currentState = CharacterState.Rolling;
        }
        else if (isGrounded && Input.GetButtonDown("Jump")) // To Jumping.
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce * Time.fixedDeltaTime);
            Game.Instance.SOMA.PlaySound("Jump");
            StartJump();
        }
    }
    private void HandleRollingState()
    {
        // State logic here.
        MoveCharacter();
        // Transistions.
        if (Input.GetKeyUp(KeyCode.S))
        {
            cc.offset = new Vector2(0.33f, -0.25f);
            cc.size = new Vector2(2f, 3.5f);
            Game.Instance.SOMA.StopLoopedSound();
            an.SetBool("isRolling", false);
            currentState = CharacterState.Idling;
        }

    }
    private void HandleJumpingState()
    {
        // Jumping state logic here.
        MoveCharacter();
        // Transitions.
        if (isGrounded) // First frame where isGrounded is true, we transition to idling.
        {
            an.SetBool("isJumping", false);
            currentState = CharacterState.Idling;
        }
    }
    private void HandleDeathState()
    {

    }
    private void MoveCharacter()
    {
        // Horizontal movement.
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveForce * Time.fixedDeltaTime, rb.velocity.y);
    }

    private void GroundedCheck()
    {
        isGrounded = Physics2D.OverlapBox(groundDetect.position, 
            new Vector2(groundCheckWidth, groundCheckHeight), 0f, groundLayer);
        an.SetBool("isJumping", !isGrounded);
    }

    private void StartJump()
    {
        jumpStarted = true;
        isGrounded = false;

        an.SetBool("isJumping", true);
        currentState = CharacterState.Jumping;
        Invoke("ResetJumpStarted", 0.5f);
    }
    private void ResetJumpStarted()
    {
        jumpStarted = false;
    }
    void GameOver()
    {
        SceneLoader.LoadSceneByIndex(2);
    }

}
