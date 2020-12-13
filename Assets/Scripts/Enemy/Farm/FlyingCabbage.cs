using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingCabbage : Enemy {

    [Header("Cabbage Settings")]
    public State state = State.HOVERING;
    public float delayBetweenActions = 2.5f;
    private float timeStartedAction = 0f;

    [Header("Hover Settings")]
    public float hoverHeight = 15f;
    private float hoverDelay = 0.35f;
    private float startTime = 0f;
    private float distanceFromTarget = 0f;

    [Header("State Settings")]
    public float distanceToReach = 20f;
    public float flySpeed = 10f;

    [Header("Firing Settings")]
    public GameObject projectile;
    public int damage;
    public float projSpeed;

    [Header("Sound Settings")]
    [Range(0, 1)]
    public float flySoundVolume = 1;
    public AudioClip flySound;
    [Range(0, 1)]
    public float attackSoundVolume = 1;
    public AudioClip attackSound;

    public enum State {
        HOVERING,
        FIRING,
        MOVING
    }

    new void Start() {
        health = healthTotal;
        transform.position += (Vector3.up * hoverHeight);
    }

    new void Update() {
        base.Update();
        Hover();
        SetState();
        StateCheck();
    }

    void StateCheck() {
        switch (state) {
            case State.HOVERING:
                //maybe play sound
                break;
            case State.MOVING:
                Move();
                break;
            case State.FIRING:
                Fire();
                break;
        }
    }


    void SetState() {
        distanceFromTarget = Vector3.Distance(transform.position, GameManager.gm.player.transform.position);
        bool inRange = distanceFromTarget < distanceToReach;
        transform.LookAt(PlayerPositionAtHeight());
        if (!CanAct()) {
            return;
        }
        if (!GameManager.gm.HasLineOfSightToPlayer(gameObject)) {
            state = State.HOVERING;
            return;
        }
        int i = Random.Range(0, 100);
        //print(i);
        if (inRange) {
            if (i <= 10) {
                timeStartedAction = Time.time;
                state = State.MOVING;
            } else if (i <= 30) {
                timeStartedAction = Time.time;
                state = State.HOVERING;
            } else {
                state = State.FIRING;
            }
        } else {
            timeStartedAction = Time.time;
            if (i <= 40) {
                state = State.HOVERING;
            } else {
                state = State.MOVING;
            }
        }
    }

    bool CanAct() {
        if (Time.time < timeStartedAction + delayBetweenActions) {
            return false;
        }
        return true;
    }

    void Hover() {
        if(transform.position.y < hoverHeight && Time.time > startTime + hoverDelay) {
            PlaySound(flySound, flySoundVolume);
            startTime = Time.time;
            rb.AddForce(Vector3.up * 25f, ForceMode.VelocityChange);
        }
    }

    void Fire() {
        if (CanAct()) {
            timeStartedAction = Time.time;
            GameObject p = Instantiate(projectile);
            p.transform.position = this.gameObject.transform.position;
            p.transform.LookAt(GameManager.gm.player.transform);
            p.GetComponent<EnemyProjectile>().SetProjectile(damage, projSpeed);
            PlaySound(attackSound, attackSoundVolume);
        }
    }

    void Move() {
        if(distanceFromTarget <= 1f) {
            return;
        }
        rb.AddForce(transform.forward * flySpeed, ForceMode.Acceleration);
    }

    Vector3 PlayerPositionAtHeight() {
        return new Vector3(GameManager.gm.player.transform.position.x, hoverHeight, GameManager.gm.player.transform.position.z);
    }

}
