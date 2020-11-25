/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CobbleStoneGuardian : Enemy {

    [Header("Guardian Settings")]
    public State state;
    public float range = 20f;

    [Header("Attack Settings")]
    private float lastTimeAttacked = 0f;
    [Tooltip("Delay between stages between attack. Higher value means slower attacks")]
    public float timeBetweenAttacks = 5f;
    [Tooltip("The distance of the smash")]
    public float smashDistanceDMG = 15f;
    [Tooltip("The force of the smash")]
    public float explosiveForce = 10f;
    [Tooltip("Damage of the smash")]
    public int smashDMG = 2;
    private Coroutine attackCoroutine;

    [Header("Guardian References")]
    public GameObject arms;
    public GameObject groundCheck;

    public enum State {
        BLEND,
        READY,
        SMASH,
        JUMP
    }

    // Update is called once per frame
    new void Update() {
        base.Update();
        switch (state) {
            case State.BLEND:
                Blend();
                break;
            case State.READY:
                if (Vector3.Distance(GameManager.gm.player.transform.position, this.gameObject.transform.position) <= range) {
                    //ATTACK
                    TrySmash();
                } else {
                    //GET CLOSER
                    TryJump();
                }
                break;
            case State.SMASH:
                if (attackCoroutine == null) {
                    attackCoroutine = StartCoroutine(Smash());
                }
                break;
            case State.JUMP:
                if(attackCoroutine == null) {
                    attackCoroutine = StartCoroutine(Jump());
                }
                break;
        }
    }

    void Blend() {
        if(Vector3.Distance(GameManager.gm.player.transform.position, this.gameObject.transform.position) <= range && GameManager.gm.HasLineOfSight(GameManager.gm.player, this.gameObject)) {
            state = State.READY;
            lastTimeAttacked = Time.time;
            arms.SetActive(true);
            EnableCollisions();
            audioSource.PlayOneShot(audioClips[0]);
        }
    }

    void TrySmash() {
        if(lastTimeAttacked + timeBetweenAttacks < Time.time) {
            state = State.SMASH;
        }
    }

    IEnumerator Smash() {
        //audioSource.PlayOneShot(audioClips[0]);
        rb.AddForce(Vector3.up * 10f, ForceMode.VelocityChange);
        yield return new WaitForSeconds(1f);
        rb.velocity = Vector3.zero;
        rb.AddForce(Vector3.up * -30f, ForceMode.VelocityChange);
        float timeout = Time.time;
        while (!Physics.CheckSphere(groundCheck.transform.position, 0.5f, GameManager.gm.groundMask) && timeout + 2f > Time.time) {
            yield return null;
        }
        rb.velocity = Vector3.zero;
        Collider[] colliders = Physics.OverlapSphere(transform.position, smashDistanceDMG, GameManager.gm.enemyPlayerLayers);
        foreach (Collider h in colliders) {
            if (h.gameObject == this) {
                continue;
            }
            Rigidbody hRB;
            if (h.gameObject.layer == LayerMask.NameToLayer("Player")) {
                hRB = h.GetComponentInParent<Rigidbody>();
            } else {
                hRB = h.GetComponent<Rigidbody>();
            }
            if (hRB != null && GameManager.gm.HasLineOfSight(this.gameObject, h.gameObject)) {
                hRB.AddExplosionForce(explosiveForce / 4, transform.position, smashDistanceDMG, 5f, ForceMode.VelocityChange);
            }
        }
        if (Vector3.Distance(transform.position, GameManager.gm.player.transform.position) <= smashDistanceDMG && GameManager.gm.HasLineOfSight(this.gameObject, GameManager.gm.player)) {
            GameManager.gm.p.TakeDamage(smashDMG);
        }
        audioSource.PlayOneShot(audioClips[0]);
        lastTimeAttacked = Time.time;
        attackCoroutine = null;
        state = State.READY;
    }

    void TryJump() {
        if (lastTimeAttacked + timeBetweenAttacks < Time.time) {
            state = State.JUMP;
        }
    }

    IEnumerator Jump() {
        audioSource.PlayOneShot(audioClips[0]);
        rb.AddForce(Vector3.up * 50f, ForceMode.VelocityChange);
        yield return new WaitForSeconds(2f);
        rb.velocity = Vector3.zero;
        Vector3 direction = (GameManager.gm.player.transform.position - this.gameObject.transform.position).normalized;
        yield return new WaitForSeconds(0.5f);
        rb.AddForce(direction * 175f, ForceMode.VelocityChange);
        float timeout = Time.time;
        while (!Physics.CheckSphere(groundCheck.transform.position, 1f, GameManager.gm.groundMask) && timeout + 5f > Time.time) {
            yield return null;
        }
        rb.velocity = Vector3.zero;
        Collider[] colliders = Physics.OverlapSphere(transform.position, smashDistanceDMG, GameManager.gm.enemyPlayerLayers);
        foreach (Collider h in colliders) {
            if (h.gameObject == this) {
                continue;
            }
            Rigidbody hRB;
            if (h.gameObject.layer == LayerMask.NameToLayer("Player")) {
                hRB = h.GetComponentInParent<Rigidbody>();
            } else {
                hRB = h.GetComponent<Rigidbody>();
            }
            if (hRB != null && GameManager.gm.HasLineOfSight(this.gameObject, h.gameObject)) {
                hRB.AddExplosionForce(explosiveForce, transform.position, smashDistanceDMG, 5f, ForceMode.VelocityChange);
            }
        }
        if (Vector3.Distance(transform.position, GameManager.gm.player.transform.position) <= smashDistanceDMG && GameManager.gm.HasLineOfSight(this.gameObject, GameManager.gm.player)) {
            GameManager.gm.p.TakeDamage(smashDMG);
        }
        audioSource.PlayOneShot(audioClips[1]);
        lastTimeAttacked = Time.time;
        attackCoroutine = null;
        state = State.READY;
    }
}
*/