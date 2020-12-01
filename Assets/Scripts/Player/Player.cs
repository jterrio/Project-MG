﻿using System.Collections;
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

    [Header("Inventory")]
    public float money = 5f;
    public float wepFireIncrease = 0f;
    public float wepDMGIncrease = 0f;
    public int wepAmmoIncrease = 0;
    public float wepFireMulti = 1f;
    public float wepDMGMulti = 1f;
    public float wepAmmoMulti = 1f;
    public int numberOfBulletBounces = 0;
    [Tooltip("Used to determine if the bullets pierce enemies")]
    public bool piercingShots = false;


    [Header("References")]
    public TMPro.TextMeshProUGUI ammoCountText;
    public TMPro.TextMeshProUGUI moneyCountText;
    public PlayerMovement pm;
    public GameObject weaponHolder;
    public Gun gun;
    public GameObject bulletSpawn;
    public Image healthFill;
    public AudioSource audioSource;

    void Start() {
        //gun = GetComponentInChildren<Gun>();
        ammoCountText = UIManager.ui.ammoCount.GetComponent<TMPro.TextMeshProUGUI>();
        moneyCountText = UIManager.ui.moneyCount.GetComponent<TMPro.TextMeshProUGUI>();
        healthFill = UIManager.ui.HealthUIFill.GetComponent<Image>();
        UIManager.ui.TurnOnHUD();
        SetMoneyText();
        UpdateHealth();
    }

    // Update is called once per frame
    void Update() {
        UpdateAmmoCount();
    }

    /// <summary>
    /// Updates the UI for displaying bullet count and when reloading
    /// </summary>
    void UpdateAmmoCount() {
        if (ammoCountText != null) {
            if (gun.isReloading) {
                ammoCountText.text = "--/" + gun.GetMagSize().ToString();
            } else {
                ammoCountText.text = gun.bulletsInMag.ToString() + "/" + gun.GetMagSize().ToString();
            }

        }
    }


    /// <summary>
    /// Deals damage to the player and determines if they should die
    /// </summary>
    /// <param name="d">Damage</param>
    public void TakeDamage(int d) {
        //Checks to see if the player still has invicibility frames
        if (lastTimeTakenDMG + invDMGSecs >= Time.time) {
            return;
        }
        lastTimeTakenDMG = Time.time;
        healthCurrent -= d;
        UpdateHealth();
        if (healthCurrent <= 0) {
            Die();
        } else {
            audioSource.PlayOneShot(damageSound);
        }
    }

    /// <summary>
    /// Kills the player and handles the post-death process
    /// </summary>
    void Die() {
        GameManager.gm.playerMinimapObject.SetActive(false);
        ItemManager.im.ResetPlayerItems();
        Destroy(RoomManager.rm.transform.parent.gameObject);
        RoomManager.rm = null;
        LevelManager.lm.LoadFarmDeath();
    }

    /// <summary>
    /// Updates the health bar
    /// </summary>
    void UpdateHealth() {
        healthFill.fillAmount = healthCurrent / healthTotal;
    }

    /// <summary>
    /// Removes money from player wallet
    /// </summary>
    /// <param name="i">Ammount to lose</param>
    public void LoseMoney(float i) {
        money -= i;
        if(money < 0) {
            money = 0;
        }
        SetMoneyText();
    }

    /// <summary>
    /// Adds money from player wallet
    /// </summary>
    /// <param name="i">Ammount to gain</param>
    public void GainMoney(float i) {
        money += i;
        SetMoneyText();
    }

    /// <summary>
    /// Updates money on the UI to reflect wallet
    /// </summary>
    void SetMoneyText() {
        moneyCountText.text = Mathf.FloorToInt(money).ToString();
    }
}
