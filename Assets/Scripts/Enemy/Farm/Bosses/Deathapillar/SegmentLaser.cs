using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentLaser : MonoBehaviour {

    public float speed;

    void Update() {
        transform.position += (transform.forward * speed * Time.deltaTime);
    }


    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Bullet") || collider.gameObject.layer == LayerMask.NameToLayer("Bounds")) {
            return;
        }
        if (gameObject == null) {
            return;
        }
        //Collider myCollider = collision.contacts[0].otherCollider;
        if (collider.gameObject.layer == LayerMask.NameToLayer("Player")) {
            if (Deathapillar.originalBody == null) {
                GameManager.gm.p.TakeDamage(1);
            } else {
                GameManager.gm.p.TakeDamage(Deathapillar.originalBody.laserDamage);
            }
        }
    }


}
