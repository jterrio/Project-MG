using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentPart : Enemy {


    private new void Start() {

    }

    protected new void Update() {

    }


    public override void TakeDamage(float amount) {
        health -= amount;
        Deathapillar.originalBody.currentHealth -= amount;
        Deathapillar.originalBody.UpdateHealth();
        //healthBarFill.fillAmount = health / healthTotal;
        if (health <= 0f) {
            Die();
        }
    }

    protected new void Die() {
        Destroy(this.gameObject);
    }


}
