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
    [Tooltip("Speed that bullets travel")]
    public float bulletSpeed;
    [Tooltip("Things that the bullet can hit")]
    public LayerMask hitLayerMask;
    [Tooltip("How many bullets fire per click")]
    public int shotsPerTrigger = 1;
    [Tooltip("Used for delay in burst fire between bullets inside a burst")]
    public float burstDelay = 0.2f;

    [Header("Reload Settings")]
    [Tooltip("Time to fully reload")]
    public float reloadTime = 5;
    private float reloadTimeCounter;
    private Coroutine reloadCoroutine;
    [Tooltip("Is reloading")]
    public bool isReloading = false;

    [Header("Damage Settings")]
    [Tooltip("Base damage per bullet")]
    public float damage = 10f;
    private float lastFired;

    [Header("Sound Settings")]
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
        bulletsInMag = GetMagSize();
    }

    private void Update() {
        if (Input.GetMouseButton(0)) {
            StartCoroutine(Shoot());
        }
        if(Input.GetKeyDown(KeyCode.R) && reloadCoroutine == null && bulletsInMag < GetMagSize()) {
            reloadCoroutine = StartCoroutine("Reload");
        }
    }

    public int GetMagSize() {
        return Mathf.FloorToInt((magSize + GameManager.gm.p.wepAmmoIncrease) * GameManager.gm.p.wepAmmoMulti);
    }

    public float GetDamage() {
        return ((damage + GameManager.gm.p.wepDMGIncrease) * GameManager.gm.p.wepDMGMulti);
    }

    public float GetFireRate() {
        return ((fireRate + GameManager.gm.p.wepFireIncrease) * GameManager.gm.p.wepFireMulti);
    }


    IEnumerator Shoot() {
        if (Time.time - lastFired > 1 / GetFireRate()) {
            if (shotsPerTrigger == 1) {
                lastFired = Time.time;
            } else {
                lastFired = Time.time + (burstDelay * shotsPerTrigger);
            }

            //Cant shoot if reloading
            if (isReloading) {
                yield break;
            }

            //Cant shoot if you have no bullets
            if (bulletsInMag <= 0) {
                bulletsInMag = 0;
                audioSource.PlayOneShot(emptySound);
                yield break;
            }

            //fire bullets
            if (shotsPerTrigger == 1) {
                FireBullet();
            } else {
                for (int i = 0; i < shotsPerTrigger; i++) {
                    FireBullet();
                    if (bulletsInMag <= 0) {
                        yield break;
                    }
                    yield return new WaitForSeconds(burstDelay);
                }
            }

        }
    }

    void FireBullet() {
        bulletsInMag--;
        gunFlash.Play();
        audioSource.PlayOneShot(fireSounds[Random.Range(0, fireSounds.Length - 1)]);
        ItemManager.im.gunDelegate?.Invoke();
        CreateBullet();
    }

    void CreateBullet() {
        RaycastHit hit;
        GameObject b = Instantiate(bullet);
        b.transform.position = bulletEmitter.transform.position;
        Vector3 v = Vector3.zero;
        if (Physics.Raycast(GameManager.gm.playerCamera.transform.position, GameManager.gm.playerCamera.transform.forward, out hit, Mathf.Infinity, hitLayerMask)) {
            print(hit.collider.gameObject.name);
            v = hit.point;
        } else {
            v = GameManager.gm.playerCamera.transform.position + (GameManager.gm.playerCamera.transform.forward * 50f);
        }
        b.transform.LookAt(v);
        b.GetComponent<Rigidbody>().AddForce(b.transform.forward * bulletSpeed);
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
        bulletsInMag = GetMagSize();
        reloadCoroutine = null;
        isReloading = false;
    }

}
