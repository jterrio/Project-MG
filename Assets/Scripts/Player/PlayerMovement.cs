using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    public CharacterController controller;

    [Header("Speed Settings")]
    [Tooltip("Normal movement speed")]
    public float speed = 10f;
    [Tooltip("Gravity that affects the player")]
    public float gravity = -10f;
    [Tooltip("Height of jump")]
    public float jumpHeight = 5f;

    [Header("Ground Settings")]
    [Tooltip("Point on bottom for detecting ground collision")]
    public Transform groundCheck;
    [Tooltip("Distance from ground to be considered grounded")]
    public float groundDistance = 0.4f;
    [Tooltip("Layer that detects the ground")]
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;

    void Update() {
        CheckGround();
        Move();
        CheckJump();
        ApplyMovement();
    }

    void Move() {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);
    }


    void CheckJump() {
        if (Input.GetButton("Jump") && isGrounded) {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void CheckGround() {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0) {
            velocity.y = -2f;
        }
    }

    void ApplyMovement() {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void Warp(Vector3 newPos) {
        controller.enabled = false;
        transform.position = newPos;
        controller.enabled = true;
    }

}