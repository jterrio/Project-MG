using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentPart : Enemy {


    private new void Start() {

    }

    protected new void Update() {

    }


    public override void TakeDamage(float amount) {

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

    protected new void Die() {
        Destroy(this.gameObject);
    }


}
