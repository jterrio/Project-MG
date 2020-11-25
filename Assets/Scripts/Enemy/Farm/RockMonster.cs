using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockMonster : Enemy {

    public State state = State.IDLE;
    public Microstate microstate = Microstate.MOVING;
    public float movementForce = 10f;
    public float gravityCoefficent = 10f;
    public float distanceToMove = 60f;
    public float distanceToCrush = 10f;
    public float lastTimeMoved = 0f;
    public float movementDelay = 2f;
    public float airTime = 1.35f;
    public SphereCollider sc;


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

    public enum Microstate {
        ATTACKING,
        MOVING
    }



    // Start is called before the first frame update
    void Start() {
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
    }

    // Update is called once per frame
    new void Update() {
        base.Update();
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
    /// Checks through the different conditions to determine which state the monster should be in
    /// </summary>
    void StateCheck() {
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

    void Move() {
        if (CanAct(movementDelay)) {
            lastTimeMoved = Time.time;
            crushCoroutine = StartCoroutine(MoveAttack());
        }
    }


    Vector3 GetForceAngle() {
        float angle = Random.Range(35f, 55f);
        float xcomponent = Mathf.Cos(angle * Mathf.PI / 180);
        float ycomponent = Mathf.Sin(angle * Mathf.PI / 180);
        return new Vector3(0, ycomponent, xcomponent);
    }

    void Crush() {
        if (CanAct(movementDelay + airTime)) {
            lastTimeMoved = Time.time + airTime;
            crushCoroutine = StartCoroutine(CrushAttack());
        }
    }

    IEnumerator CrushAttack() {
        rb.AddForce(Vector3.up * 25f, ForceMode.VelocityChange);
        audioSource.PlayOneShot(jump, jumpVolume);
        yield return new WaitForSeconds(airTime);
        rb.velocity = Vector3.zero;
        rb.AddForce(Vector3.down * 50f, ForceMode.VelocityChange);
        audioSource.PlayOneShot(smashGround, smashGroundVolume);
        TryDamage();

        crushCoroutine = null;
        yield return null;
    }

    IEnumerator MoveAttack() {
        LookAt(GameManager.gm.player);
        Vector3 angle = GetForceAngle();
        rb.AddRelativeForce(angle * movementForce, ForceMode.VelocityChange);
        audioSource.PlayOneShot(jump, jumpVolume);
        yield return new WaitForSeconds(movementDelay * 0.95f);

        if (!IsGrounded()) {
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector3.down * 200f, ForceMode.VelocityChange);
            lastTimeMoved = Time.time;
            yield return new WaitForSeconds(0.75f);
            audioSource.PlayOneShot(smashGround, smashGroundVolume);
            TryDamage();
        } else {
            audioSource.PlayOneShot(groundLand, groundLandVolume);
        }

        crushCoroutine = null;
        yield return null;
    }


    public void StopAttacking() {
        if (crushCoroutine != null) {
            StopCoroutine(crushCoroutine);
            crushCoroutine = null;
        }
    }

    public bool CanAct(float delay) {
        if(Time.time >= lastTimeMoved + delay) {
            return true;
        }
        return false;
    }


    void TryDamage() {
        float dis = Vector3.Distance(this.gameObject.transform.position, GameManager.gm.player.transform.position);
        if(dis <= distanceToCrush && GameManager.gm.HasLineOfSight(this.gameObject, GameManager.gm.player)) {
            GameManager.gm.p.TakeDamage(1);
        }
    }

    public bool IsGrounded() {
        return Physics.CheckSphere(this.gameObject.transform.position, sc.radius * 1.1f, GameManager.gm.groundMask);
    }

    


}
