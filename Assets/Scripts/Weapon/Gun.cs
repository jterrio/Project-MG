using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour {


    [Header("Bullet Settings")]
    [Tooltip("Base damage per bullet")]
    public float fireRate = 10f;
    [Tooltip("Bullets per magazine")]
    public int magSize = 25;
    public int bulletsInMag;
    public float reloadTime = 5;
    private float reloadTimeCounter;
    private Coroutine reloadCoroutine;
    public bool isReloading = false;

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


    public ParticleSystem gunFlash;
    public GameObject hitEffect;

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

            }
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
