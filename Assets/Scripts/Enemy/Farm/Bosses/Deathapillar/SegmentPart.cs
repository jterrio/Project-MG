using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentPart : Enemy {

    private Coroutine shootCoroutine;
    public Material headMat;
    public Material bodyMat;

    public Renderer r;

    private new void Start() {
        r = GetComponent<Renderer>();
    }
    
    private new void Update() {
        FireLaser();
    }

    public override void TakeDamage(float amount) {
        r.material.color += new Color((amount/healthTotal) / 2, 0f, 0f);
        if (amount > health) {
            Deathapillar.originalBody.currentHealth -= health;
        } else {
            Deathapillar.originalBody.currentHealth -= amount;
        }
        Deathapillar.originalBody.UpdateHealth();
        health -= amount;
        if (health <= 0f) {
            Die();
        }
    }


    public void FireLaser() {
        if(Deathapillar.originalBody != null && shootCoroutine == null) {
            if(Deathapillar.originalBody.state != Deathapillar.State.DIVING) {
                return;
            }
            shootCoroutine = StartCoroutine(Shoot());
        }
    }

    IEnumerator Shoot() {
        float x = Random.Range(Deathapillar.originalBody.laserFiringMinDelay, Deathapillar.originalBody.laserFiringMaxDelay);
        yield return new WaitForSeconds(x);
        if (!GameManager.gm.IsBlockedByGroundToPlayer(gameObject) && Deathapillar.originalBody != null) {
            GameObject laser = Instantiate(Deathapillar.originalBody.laserPrefab);
            laser.transform.position = transform.position;

            int random = Random.Range(0, 100);
            if(random <= 70) {
                laser.transform.LookAt(GameManager.gm.player.transform.position + new Vector3(0, 0.65f, 0));
            } else {
                laser.transform.LookAt(GameManager.gm.player.transform.position + new Vector3(Random.Range(-2f, 2f), Random.Range(0f, 1.2f), Random.Range(-2f, 2f)));
            }

            SegmentLaser sl = laser.GetComponent<SegmentLaser>();
            sl.speed = Deathapillar.originalBody.laserSpeed;
            AudioSource.PlayClipAtPoint(Deathapillar.originalBody.deathShoot, transform.position, Deathapillar.originalBody.deathShootVolume);
            Destroy(laser, 3f);
        }


        shootCoroutine = null;
    }

    protected new void Die() {
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Bullet") || collision.gameObject.layer == LayerMask.NameToLayer("Bounds")) {
            return;
        }
        if (gameObject == null) {
            return;
        }
        Collider myCollider = collision.contacts[0].otherCollider;
        if (myCollider.gameObject.layer == LayerMask.NameToLayer("Player")) {
            if(Deathapillar.originalBody == null) {
                GameManager.gm.p.TakeDamage(1);
            } else {
                GameManager.gm.p.TakeDamage(Deathapillar.originalBody.collisionDamage);
            }
        }
    }


}
