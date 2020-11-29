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
    [Tooltip("How loud the firing should be")]
    [Range(0, 1)]
    public float fireSoundVolume = 1;
    [Tooltip("How loud the reload should be")]
    [Range(0, 1)]
    public float reloadVolume = 1;
    [Tooltip("How loud the empty clicking should be")]
    [Range(0, 1)]
    public float emptyVolume = 1;
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


    /// <summary>
    /// Calculates the max ammo a player can have based on the the initial + power ups
    /// </summary>
    /// <returns>Max magazine size</returns>
    public int GetMagSize() {
        return Mathf.FloorToInt((magSize + GameManager.gm.p.wepAmmoIncrease) * GameManager.gm.p.wepAmmoMulti);
    }

    /// <summary>
    /// Calculates the damage a player does based on the the initial + power ups
    /// </summary>
    /// <returns>Player damage</returns>
    public float GetDamage() {
        return ((damage + GameManager.gm.p.wepDMGIncrease) * GameManager.gm.p.wepDMGMulti);
    }

    /// <summary>
    /// Calculates the fire rate a player has based on the the initial + power ups
    /// </summary>
    /// <returns>Bullet fire speed</returns>
    public float GetFireRate() {
        return ((fireRate + GameManager.gm.p.wepFireIncrease) * GameManager.gm.p.wepFireMulti);
    }

    /// <summary>
    /// Does calculations for eligiblity and power ups, then calls necessary methods
    /// </summary>
    /// <returns></returns>
    IEnumerator Shoot() {
        //Checks if the player is able to fire given the firerate of the weapon and the last time they fired
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
                audioSource.PlayOneShot(emptySound, emptyVolume);
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

    /// <summary>
    /// Fires a bullet out of the gun: removing bullet from mag, playing sound, and creating the object
    /// </summary>
    void FireBullet() {
        bulletsInMag--;
        gunFlash.Play();
        audioSource.PlayOneShot(fireSounds[Random.Range(0, fireSounds.Length - 1)], fireSoundVolume);
        ItemManager.im.gunDelegate?.Invoke();
        CreateBullet();
    }

    /// <summary>
    /// Creats the bullet object and applies the force foward
    /// </summary>
    void CreateBullet() {
        RaycastHit hit;
        GameObject b = Instantiate(bullet);
        b.transform.position = bulletEmitter.transform.position;
        Vector3 v = Vector3.zero;

        //If the player is aiming at something, the bullet will towards that object (not directly straight), unless there is nothing to aim at, then it will go directly straight
        if (Physics.Raycast(GameManager.gm.playerCamera.transform.position, GameManager.gm.playerCamera.transform.forward, out hit, Mathf.Infinity, hitLayerMask)) {
            print(hit.collider.gameObject.name);
            v = hit.point;
        } else {
            v = GameManager.gm.playerCamera.transform.position + (GameManager.gm.playerCamera.transform.forward * 50f);
        }

        //Look at the direction it is going and apply the calculated force
        b.transform.LookAt(v);
        b.GetComponent<Rigidbody>().AddForce(b.transform.forward * bulletSpeed);
    }


    /// <summary>
    /// Runs through the entire reload process
    /// </summary>
    /// <returns></returns>
    IEnumerator Reload() {
        isReloading = true;
        float startReloadTime = Time.time;

        //Triggers any items that occur from starting a reload
        ItemManager.im.beforeReloadDelegate?.Invoke();

        //start
        audioSource.PlayOneShot(reloadSound[0], reloadVolume);
        while (Time.time < startReloadTime + (reloadTime / 3)) {
            yield return null;
        }
        //middle
        while (Time.time < startReloadTime + ((reloadTime * 2) / 3)) {
            yield return null;
        }
        audioSource.PlayOneShot(reloadSound[1], reloadVolume);

        //end
        while (Time.time < startReloadTime + reloadTime) {
            yield return null;
        }

        //Triggers any items that occur from ending a reload
        ItemManager.im.afterReloadDelegate?.Invoke();

        audioSource.PlayOneShot(reloadSound[2], reloadVolume);
        bulletsInMag = GetMagSize();
        reloadCoroutine = null;
        isReloading = false;
    }

}
