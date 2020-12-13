using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockMonster : Enemy {

    [Header("Rock Settings")]
    [Tooltip("The current movement and attack state")]
    public State state = State.IDLE;
    [Tooltip("How hard of force to apply when moving normally")]
    public float movementForce = 10f;
    [Header("Attack and Movement Settings")]
    [Tooltip("Distance to chase")]
    public float distanceToMove = 60f;
    [Tooltip("Distance to attack")]
    public float distanceToCrush = 10f;
    [Tooltip("Delays between attacking and moving")]
    public float movementDelay = 2f;
    [Tooltip("How long to stay in the air")]
    public float airTime = 1.35f;
    [Tooltip("How long to wait in air before slamming")]
    public float jumpTimeDelay = 2.3f;
    private float lastTimeMoved = 0f;

    [Header("Colliders")]
    public SphereCollider sc;
    private bool isGrounded;
    private float jumpTime;



    [Header("Sound Settings")]
    [Tooltip("How loud the jump should be")]
    [Range(0, 1)]
    public float jumpVolume = 1;
    public AudioClip jump;
    [Tooltip("How loud the ground land should be")]
    [Range(0, 1)]
    public float groundLandVolume = 1;
    public AudioClip groundLand;
    [Tooltip("How loud the smash ground should be")]
    [Range(0, 1)]
    public float smashGroundVolume = 1;
    public AudioClip smashGround;


    private Coroutine crushCoroutine;

    public enum State {
        IDLE,
        MOVING,
        CRUSHING
    }



    // Start is called before the first frame update
    new void Start() {
        health = healthTotal;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
    }

    // Update is called once per frame
    new void Update() {
        base.Update();
        Grounded = IsGrounded();
        StateCheck();
        switch (state) {
            case State.IDLE:
                break;
            case State.CRUSHING:
                if (crushCoroutine == null) {
                    Crush();
                }
                break;
            case State.MOVING:
                if(crushCoroutine == null) {
                    Move();
                }
                break;
        }
    }


    /// <summary>
    /// Checks to see the monster's air-to-ground status; Plays sounds and triggers crushing from being in the air too long
    /// </summary>
    /// <param name="old">Last frame's ground value</param>
    /// <param name="n">This frame's ground value</param>
    public bool Grounded {
        get {
            return isGrounded;
        }
        set {
            if(crushCoroutine != null) {
                isGrounded = value;
                return;
            }
            if (!value && isGrounded) { //was grounded, but is not
                jumpTime = Time.time;
                PlaySound(jump, jumpVolume);
            } else if (value && !isGrounded) { //was in air, but grounded
                if (state == State.CRUSHING) {
                    TryDamage();
                    PlaySound(smashGround, smashGroundVolume);
                } else {
                    PlaySound(groundLand, groundLandVolume);
                }
            } else if (!value && !isGrounded && state != State.CRUSHING) { //was in air, and still in air
                if (Time.time >= jumpTime + jumpTimeDelay) {
                    state = State.CRUSHING;
                    crushCoroutine = StartCoroutine(CrushAttack());
                }
            }
            isGrounded = value;
        }
    }

    /// <summary>
    /// Checks through the different conditions to determine which state the monster should be in
    /// </summary>
    void StateCheck() {
        //DO NOT change the state if the monster is in the air
        if (!isGrounded) {
            return;
        }
        float dis = Vector3.Distance(this.gameObject.transform.position, GameManager.gm.player.transform.position);
        if (dis >= distanceToMove) {
            state = State.IDLE;
        } else if (GameManager.gm.HasLineOfSight(this.gameObject, GameManager.gm.player)) {
            if (dis >= distanceToCrush) {
                state = State.MOVING;
            } else {
                state = State.CRUSHING;
            }
        } else {
            state = State.IDLE;
        }
    }

    /// <summary>
    /// Attempts to move the monster if it is grounded and able to.
    /// </summary>
    void Move() {
        if (!isGrounded) {
            return;
        }
        if (CanAct(movementDelay)) {
            lastTimeMoved = Time.time;
            LookAt(GameManager.gm.player);
            Vector3 angle = GetForceAngle();
            rb.AddRelativeForce(angle * movementForce, ForceMode.VelocityChange);
            //audioSource.PlayOneShot(jump, jumpVolume);
            //crushCoroutine = StartCoroutine(MoveAttack());
        }
    }

    /// <summary>
    /// Gives a random angle between 35 degrees and 55 degrees
    /// </summary>
    /// <returns>Random angle</returns>
    Vector3 GetForceAngle() {
        float angle = Random.Range(35f, 55f);
        float xcomponent = Mathf.Cos(angle * Mathf.PI / 180);
        float ycomponent = Mathf.Sin(angle * Mathf.PI / 180);
        return new Vector3(0, ycomponent, xcomponent);
    }

    /// <summary>
    /// Attempts to crush attack, given it is able to move
    /// </summary>
    void Crush() {
        if (CanAct(movementDelay)) {
            lastTimeMoved = Time.time + airTime;
            crushCoroutine = StartCoroutine(CrushAttack());
        }
    }

    /// <summary>
    /// Used for crush attack and crushing down when in air too long
    /// </summary>
    /// <returns></returns>
    IEnumerator CrushAttack() {
        //isGrounded = false;
        if (isGrounded) {
            rb.AddForce(Vector3.up * 25f, ForceMode.VelocityChange);
            PlaySound(jump, jumpVolume);
            yield return new WaitForSeconds(airTime);
        }
        rb.velocity = Vector3.zero;
        rb.AddForce(Vector3.down * 50f, ForceMode.VelocityChange);
        yield return null;
        crushCoroutine = null;
    }

    /// <summary>
    /// Stops attacking and crushing
    /// </summary>
    public void StopAttacking() {
        if (crushCoroutine != null) {
            StopCoroutine(crushCoroutine);
            crushCoroutine = null;
        }
    }

    /// <summary>
    /// Determines if the monster is able to attack or move
    /// </summary>
    /// <param name="delay">Delay for attacking and moving</param>
    /// <returns>Eligibility to attack or move</returns>
    public bool CanAct(float delay) {
        if(Time.time >= lastTimeMoved + delay) {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Attempts damage if the player is in sight and in distance
    /// </summary>
    void TryDamage() {
        float dis = Vector3.Distance(this.gameObject.transform.position, GameManager.gm.player.transform.position);
        if(dis <= distanceToCrush && GameManager.gm.HasLineOfSight(this.gameObject, GameManager.gm.player) && GameManager.gm.playerMovement.IsGrounded) {
            GameManager.gm.p.TakeDamage(1);
        }
    }

    /// <summary>
    /// Determines if the monster is on the ground or not
    /// </summary>
    /// <returns>isGrounded</returns>
    public bool IsGrounded() {
        return Physics.CheckSphere(this.gameObject.transform.position, sc.radius * 1.1f, GameManager.gm.groundMask);
    }

    


}
