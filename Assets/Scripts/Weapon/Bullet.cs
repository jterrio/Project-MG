using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float bulletLifetime;
    public GameObject hitEffect;

    // Start is called before the first frame update
    void Start() {
        Destroy(this.gameObject, bulletLifetime);
    }

    private void Update() {
        ItemManager.im.bulletVelocityDelegate?.Invoke(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Bullet")) {
            return;
        }
        Collider myCollider = collision.contacts[0].otherCollider;
        GameObject g = Instantiate(hitEffect, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
        Destroy(g, 4f);
        if (myCollider.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            Enemy e = myCollider.gameObject.GetComponent<Enemy>();
            if(e != null) {
                Debug.Log("Hit: " + myCollider.gameObject.name + "... Health: " + e.health);
                e.TakeDamage(GameManager.gm.p.gun.GetDamage());
            } else {
                e = myCollider.gameObject.GetComponentInParent<Enemy>();
                if(e != null) {
                    Debug.Log("Hit: " + myCollider.gameObject.name + "... Health: " + e.health);
                    e.TakeDamage(GameManager.gm.p.gun.GetDamage());
                }
            }
        }
        Destroy(this.gameObject);
    }

}
