using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour {


    [Header("Bullet Settings")]
    [Tooltip("Base damage per bullet")]
    public float fireRate = 10f;
    [Tooltip("Bullets per magazine")]
    public int magSize = 25;
    [Tooltip("Bullets in magazine")]
    public int bulletsInMag;
    [Tooltip("Time to fully reload")]
    public float reloadTime = 5;
    private float reloadTimeCounter;
    private Coroutine reloadCoroutine;
    [Tooltip("Is reloading")]
    public bool isReloading = false;
    [Tooltip("Speed that bullets travel")]
    public float bulletSpeed;
    [Tooltip("Things that the bullet can hit")]
    public LayerMask hitLayerMask;

    [Header("Fire Settings")]
    [Tooltip("Base damage per bullet")]
    public float damage = 10f;
    private float lastFired;
    public AudioSource audioSource;
    public AudioClip[] fireSounds;
    public AudioClip emptySound;
    public AudioClip[] reloadSound;

    [Header("Projectile Settings")]
    [Tooltip("How far the projectile ")]
    public float range = 100f;

    [Header("References")]
    public GameObject bulletEmitter;
    public GameObject bullet;


    public ParticleSystem gunFlash;

    private void Start() {
        bulletsInMag = magSize;
    }

    private void Update() {
        if (Input.GetMouseButton(0)) {
            Shoot();
        }
        if(Input.GetKeyDown(KeyCode.R) && reloadCoroutine == null && bulletsInMag < magSize) {
            reloadCoroutine = StartCoroutine("Reload");
        }
    }

    void Shoot() {
        if (Time.time - lastFired > 1 / fireRate) {
            lastFired = Time.time;
            if (isReloading) {
                return;
            }
            if (bulletsInMag <= 0) {
                bulletsInMag = 0;
                audioSource.PlayOneShot(emptySound);
                return;
            }
            bulletsInMag--;
            gunFlash.Play();
            audioSource.PlayOneShot(fireSounds[Random.Range(0, fireSounds.Length - 1)]);

            RaycastHit hit;
            GameObject b = Instantiate(bullet);
            b.transform.position = bulletEmitter.transform.position;
            Vector3 v = Vector3.zero;
            if(Physics.Raycast(GameManager.gm.playerCamera.transform.position, GameManager.gm.playerCamera.transform.forward, out hit, Mathf.Infinity, hitLayerMask)){
                //print(hit.collider.gameObject.name);
                v = hit.point;
            } else {
                v = GameManager.gm.playerCamera.transform.position + (GameManager.gm.playerCamera.transform.forward * 50f);
            }
            b.transform.LookAt(v);
            b.GetComponent<Rigidbody>().AddForce(b.transform.forward * bulletSpeed);

            /*
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, range)) {
                Enemy e = hit.transform.GetComponentInParent<Enemy>();
                if (e != null) {
                    Debug.Log("Hit: " + hit.transform.name + "... Health: " + e.health);
                    e.TakeDamage(damage);
                } else {
                    Debug.Log("Hit: " + hit.transform.name);
                }

                GameObject g = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(g, 2f);

            }*/
        }
    }

    IEnumerator Reload() {
        isReloading = true;
        float startReloadTime = Time.time;
        //start
        audioSource.PlayOneShot(reloadSound[0]);
        while (Time.time < startReloadTime + (reloadTime / 3)) {
            yield return null;
        }
        //middle
        while (Time.time < startReloadTime + ((reloadTime * 2) / 3)) {
            yield return null;
        }
        audioSource.PlayOneShot(reloadSound[1]);

        //end
        while (Time.time < startReloadTime + reloadTime) {
            yield return null;
        }
        audioSource.PlayOneShot(reloadSound[2]);
        bulletsInMag = magSize;
        reloadCoroutine = null;
        isReloading = false;
    }

}
