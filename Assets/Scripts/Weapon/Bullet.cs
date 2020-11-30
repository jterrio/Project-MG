using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float bulletLifetime;
    public int numberOfBounces = 0;
    public GameObject hitEffect;
    public GameObject monsterToIgnore;

    // Start is called before the first frame update
    void Start() {
        Destroy(this.gameObject, bulletLifetime);
    }

    private void Update() {
        ItemManager.im.bulletVelocityDelegate?.Invoke(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Bullet") || collision.gameObject.layer == LayerMask.NameToLayer("Bounds")) {
            return;
        }

        Collider myCollider = collision.contacts[0].otherCollider;
 

        if (myCollider.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
            Enemy e = myCollider.gameObject.GetComponent<Enemy>();
            if(e != null) {
                if (myCollider.gameObject == monsterToIgnore) {
                    return;
                }
                //Debug.Log("Hit: " + myCollider.gameObject.name + "... Health: " + e.health);
                e.TakeDamage(GameManager.gm.p.gun.GetDamage());
                ItemManager.im.bulletHitDelegate?.Invoke(this.gameObject, this, e.gameObject);
            } else {
                if (myCollider.gameObject.transform.parent.gameObject == monsterToIgnore) {
                    return;
                }
                e = myCollider.gameObject.GetComponentInParent<Enemy>();
                if(e != null) {
                    //Debug.Log("Hit: " + myCollider.gameObject.name + "... Health: " + e.health);
                    e.TakeDamage(GameManager.gm.p.gun.GetDamage());
                    ItemManager.im.bulletHitDelegate?.Invoke(this.gameObject, this, e.gameObject);
                }
            }
        }


        GameObject g = Instantiate(hitEffect, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
        Destroy(g, 4f);
        Destroy(this.gameObject);
    }

}
