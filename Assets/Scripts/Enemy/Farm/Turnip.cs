using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turnip : Enemy {

    [Header("Turnip Settings")]
    public State growState = State.GROW;
    [Tooltip("Range to aggro")]
    public float range = 10f;
    [Tooltip("How much to move on the Y axis from the ground")]
    public float growth = 1.2f;
    [Tooltip("How fast it chases the player")]
    public float moveSpeed = 5f;

    [Header("Explosion Settings")]
    [Tooltip("Distance to explode from the player")]
    public float explodeDistance = 0.5f;
    [Tooltip("Distance from the explosion the player has to be to take damage")]
    public float explosionDmgDistance = 2f;
    [Tooltip("Distance from the explosion the player has to be to get moved")]
    public float explosionTotalDistance = 5f;
    [Tooltip("Time before it explodes after reaching the player")]
    public float timeToExplode = 1f;
    [Tooltip("Delay before engaging in the explosion")]
    public float explosionDelay = 0f;
    [Tooltip("Force of the explosion")]
    public float explosionForce = 3f;
    [Tooltip("Damage to player")]
    [Range(0, 100)]
    public int explosionDMG = 2;
    [Tooltip("Damage to player")]
    [Range(0, 1)]
    public float explosionVolume = 1;

    public ParticleSystem explosionEffect;
    private Coroutine explosionCoroutine;

    public enum State {
        GROW,
        HARVEST,
        FEAST,
        BOOM
    }

    private void Start() {
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
    }

    new void Update() {
        base.Update();
        switch (growState) {
            case State.GROW:
                DetectGrow();
                break;
            case State.HARVEST:
                Harvest();
                break;
            case State.FEAST:
                TryFeast();
                break;
            case State.BOOM:
                if(explosionCoroutine == null) {
                    explosionCoroutine = StartCoroutine(Explosion());
                }
                break;
        }
    }

    public IEnumerator Explosion() {
        yield return new WaitForSeconds(explosionDelay);
        yield return new WaitForSeconds(timeToExplode / 4);
        float t = 0;
        audioSource.PlayOneShot(audioClips[0], explosionVolume);
        yield return new WaitForSeconds(timeToExplode / 4);
        Vector3 start = transform.position;
        Vector3 end = new Vector3(transform.position.x, transform.position.y + 3f, transform.position.z);
        while(t <= 1) {
            t += Time.fixedDeltaTime / (timeToExplode / 2);
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }
        yield return new WaitForFixedUpdate();
        Instantiate(explosionEffect).transform.position = this.transform.position;
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionTotalDistance, GameManager.gm.enemyPlayerLayers);
        foreach(Collider h in colliders) {
            if(h.gameObject == this) {
                continue;
            }
            Rigidbody hRB;
            if (h.gameObject.layer == LayerMask.NameToLayer("Player")) {
                hRB = h.GetComponentInParent<Rigidbody>();
            } else {
                hRB = h.GetComponent<Rigidbody>();
            }
            if(hRB != null && GameManager.gm.HasLineOfSight(this.gameObject, h.gameObject)) {
                hRB.AddExplosionForce(explosionForce, transform.position, explosionTotalDistance, 5f, ForceMode.VelocityChange);
            }
        }
        if(Vector3.Distance(transform.position, GameManager.gm.player.transform.position) <= explosionDmgDistance && GameManager.gm.HasLineOfSight(this.gameObject, GameManager.gm.player)) {
            GameManager.gm.p.TakeDamage(explosionDMG);
        }
        this.gameObject.SetActive(false);
        minMoneyReward = 0f;
        maxMoneyReward = 0f;
        DelayDie(0.5f);
    }

    void DetectGrow() {
        if(Vector3.Distance(GameManager.gm.player.transform.position, this.gameObject.transform.position) <= range && GameManager.gm.HasLineOfSight(this.gameObject, GameManager.gm.player)) {
            growState = State.HARVEST;
        }
    }

    void Harvest() {
        transform.position = new Vector3(transform.position.x, transform.position.y + growth, transform.position.z);
        growState = State.FEAST;
        EnableCollisions();
    }

    void TryFeast() {
        if(!GameManager.gm.HasLineOfSight(this.gameObject, GameManager.gm.player)) {
            growState = State.GROW;
            DisableCollisions();
            transform.position = new Vector3(transform.position.x, transform.position.y - growth, transform.position.z);
            return;
        }
        float d = Vector3.Distance(GameManager.gm.player.transform.position, transform.position);
        if (d > range * 1.5) {
            growState = State.GROW;
            DisableCollisions();
            transform.position = new Vector3(transform.position.x, transform.position.y - growth, transform.position.z);
        }else if (d < explodeDistance) {
            rb.MovePosition(transform.position);
            growState = State.BOOM;
        } else {
            LookAt(GameManager.gm.player);
            rb.MovePosition(transform.position + (new Vector3(transform.forward.x, 0, transform.forward.z) * moveSpeed * Time.fixedDeltaTime));
            //transform.position += new Vector3(transform.forward.x, 0, transform.forward.z) * moveSpeed * Time.deltaTime;
        }
    }



}
