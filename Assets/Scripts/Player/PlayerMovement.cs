using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public Rigidbody rb;

    [Header("Speed Settings")]
    [Tooltip("Normal movement speed")]
    public float speed = 10f;
    [Tooltip("Gravity that affects the player")]
    public float fallMultiplier = -2.5f;
    [Tooltip("Height of jump")]
    public float jumpHeight = 5f;
    [Tooltip("How long to hang into the air")]
    public float airTime = 0.1f;

    [Header("Ground Settings")]
    [Tooltip("Point on bottom for detecting ground collision")]
    public Transform groundCheck;
    [Tooltip("Distance from ground to be considered grounded")]
    public float groundDistance = 0.4f;

    [Header("Forces")]
    [Tooltip("Type of force applied to the jump")]
    public ForceMode jumpForce;
    [Tooltip("Type of force applied to the jump")]
    public ForceMode fallForce;

    private float x, z; //movement input vars

    private Vector3 move;
    bool isGrounded;
    private float lastTimeJumped = 0f;

    private void Start() {
        rb.freezeRotation = true; //Freeze rotation to be controlled by the player
    }

    void Update() {
        CheckGround();
        Move();
        CheckJump();
    }

    /// <summary>
    /// This is where movement of the player happens via keyboard or controller
    /// </summary>
    void Move() {
        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");
        move = new Vector3(x, 0, z) * speed;
        //controller.Move(move * speed * Time.deltaTime);
    }

    /// <summary>
    /// This is where the physics of rotating the rigid body come into play, along with gravity
    /// </summary>
    private void FixedUpdate() {
        rb.AddForce(rb.rotation * move, ForceMode.Impulse);
        if (!isGrounded && rb.velocity.y <= airTime) {
            Vector3 newGravity = new Vector3(0, -1 * Physics.gravity.y * (fallMultiplier - 1), 0);
            rb.AddForce(newGravity, fallForce);
        }
    }

    /// <summary>
    /// Will check to see if the player is grounded, then check to see if they are eligible to jump
    /// </summary>
    void CheckJump() {
        if (isGrounded) {
            //Check if they are eligible via the time
            if (Input.GetButton("Jump") && Time.time >= lastTimeJumped + 0.2f) {
                lastTimeJumped = Time.time;
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.AddForce(Vector3.up * jumpHeight, jumpForce);
                isGrounded = false;
            }
        }
    }
        
    /// <summary>
    /// Checks to see if they are on the ground, which can be slightly above the ground
    /// </summary>
    void CheckGround() {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, GameManager.gm.groundMask);
    }

    /// <summary>
    /// Warps the user to a location
    /// </summary>
    /// <param name="newPos"></param>
    public void Warp(Vector3 newPos) {
        transform.position = newPos;
    }

    private void OnDrawGizmos() {
        Gizmos.DrawSphere(groundCheck.position, groundDistance);
    }

}