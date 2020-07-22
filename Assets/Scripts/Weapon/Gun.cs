using UnityEngine;

public class Gun : MonoBehaviour {

    public float damage = 10f;
    public float range = 100f;

    public ParticleSystem gunFlash;
    public GameObject hitEffect;

    private void Update() {
        if (Input.GetButtonDown("Fire1")) {
            Shoot();
        }
    }

    void Shoot() {

        gunFlash.Play();

        RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, range)) {
            Enemy e = hit.transform.GetComponentInParent<Enemy>();
            if(e != null) {
                Debug.Log("Hit: " + hit.transform.name + "... Health: " + e.health);
                e.TakeDamage(damage);
            } else {
                Debug.Log("Hit: " + hit.transform.name);
            }

            GameObject g = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(g, 2f);


        }
    }

}
