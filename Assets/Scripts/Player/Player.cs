using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    [Header("Health")]
    public float healthTotal = 8f;
    public float healthCurrent = 8f;
    public AudioClip damageSound;
    public float lastTimeTakenDMG;
    public float invDMGSecs = 1f;

    [Header("References")]
    public TMPro.TextMeshProUGUI ammoCountText;
    public PlayerMovement pm;
    public GameObject weaponHolder;
    public Gun gun;
    public Image healthFill;
    public AudioSource audioSource;

    void Start() {
        //gun = GetComponentInChildren<Gun>();
    }

    // Update is called once per frame
    void Update() {
        UpdateAmmoCount();
    }


    void UpdateAmmoCount() {
        if (ammoCountText != null) {
            if (gun.isReloading) {
                ammoCountText.text = "--/" + gun.magSize.ToString();
            } else {
                ammoCountText.text = gun.bulletsInMag.ToString() + "/" + gun.magSize.ToString();
            }

        }
    }

    public void TakeDamage(int d) {
        if (lastTimeTakenDMG + invDMGSecs >= Time.time) {
            return;
        }
        lastTimeTakenDMG = Time.time;
        audioSource.PlayOneShot(damageSound);
        healthCurrent -= d;
        UpdateHealth();
        if (healthCurrent <= 0) {
            Die();
        }
    }

    void Die() {
        LevelManager.lm.LoadFarmLevel();
    }

    void UpdateHealth() {
        healthFill.fillAmount = healthCurrent / healthTotal;
    }
}
