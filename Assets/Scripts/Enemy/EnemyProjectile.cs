using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour {

    public float bulletLifetime;
    public HazardManager.StatusEffect statusEffect;
    private int damage = 1;
    private float speed  = 1f;


    private void Start() {
        Destroy(this.gameObject, bulletLifetime);
    }

    public void SetProjectile(int d, float s) {
        damage = d;
        speed = s;
    }

    void Update() {
        transform.position += (transform.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Bullet") || collider.gameObject.layer == LayerMask.NameToLayer("Bounds") || collider.gameObject.layer == LayerMask.NameToLayer("InvisibleLOSWall")) {
            return;
        }
        if (gameObject == null) {
            return;
        }
        //Collider myCollider = collision.contacts[0].otherCollider;
        if (collider.gameObject.layer == LayerMask.NameToLayer("Player")) {
            if(GameManager.gm.p.TakeDamage(damage)){
                HazardManager.hm.ApplyStatusEffect(statusEffect, GameManager.gm.player);
            }
        }
        
    }
}
