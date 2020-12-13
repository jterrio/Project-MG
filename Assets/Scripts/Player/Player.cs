using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Actor {

    [Header("Damage")]
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

    private bool uplifting = false;
    private float timeStartedUplifting = 0f;




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
        CheckIfLift();
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


    public void SetUplift(bool v) {
        uplifting = v;
        if (v) {
            timeStartedUplifting = Time.time;
        }
    }

    void CheckIfLift() {
        if(Time.time >= timeStartedUplifting + 0.5f) {
            SetUplift(false);
        }
    }

    public bool IsUplifting() {
        return uplifting;
    }

    /// <summary>
    /// Deals damage to the player and determines if they should die
    /// </summary>
    /// <param name="d">Damage</param>
    public bool TakeDamage(float d) {
        //Checks to see if the player still has invicibility frames
        if (lastTimeTakenDMG + invDMGSecs >= Time.time) {
            return false;
        }
        lastTimeTakenDMG = Time.time;
        health -= d;
        UpdateHealth();
        if (health <= 0) {
            Die();
        } else {
            audioSource.PlayOneShot(damageSound);
        }
        return true;
    }

    public void TakeStatusDamage(float d) {
        health -= d;
        UpdateHealth();
        if (health <= 0) {
            Die();
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
        healthFill.fillAmount = health / healthTotal;
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
