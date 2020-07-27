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
    [Tooltip("Layer that detects the ground")]
    public LayerMask groundMask;

    public ForceMode jumpForce;
    public ForceMode fallForce;

    private Vector3 move;
    bool isGrounded;

    private void Start() {
        rb.freezeRotation = true;
    }

    void Update() {
        CheckGround();
        Move();

    }

    void Move() {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        move = new Vector3(x, 0, z) * speed;
        //controller.Move(move * speed * Time.deltaTime);
    }

    private void FixedUpdate() {
        CheckJump();
        rb.AddForce(rb.rotation * move, ForceMode.Impulse);
    }


    void CheckJump() {
        if (Input.GetButton("Jump") && isGrounded) {
            Debug.Log("JUMP " + Time.time);
            rb.AddForce(Vector3.up * jumpHeight, jumpForce);
            isGrounded = false;
        } else if(!isGrounded && rb.velocity.y <= airTime) {
            Vector3 newGravity = new Vector3(0, -1 * Physics.gravity.y * (fallMultiplier - 1), 0);
            rb.AddForce(newGravity, fallForce);
        }
    }

    void CheckGround() {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    public void Warp(Vector3 newPos) {
        transform.position = newPos;
    }

    private void OnDrawGizmos() {
        Gizmos.DrawSphere(groundCheck.position, groundDistance);
    }

}