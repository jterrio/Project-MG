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

    [Header("Inventory")]
    public float money = 5f;
    public float wepFireIncrease = 0f;
    public float wepDMGIncrease = 0f;
    public int wepAmmoIncrease = 0;
    public float wepFireMulti = 1f;
    public float wepDMGMulti = 1f;
    public float wepAmmoMulti = 1f;


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


    void UpdateAmmoCount() {
        if (ammoCountText != null) {
            if (gun.isReloading) {
                ammoCountText.text = "--/" + gun.GetMagSize().ToString();
            } else {
                ammoCountText.text = gun.bulletsInMag.ToString() + "/" + gun.GetMagSize().ToString();
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
        GameManager.gm.playerMinimapObject.SetActive(false);
        Destroy(RoomManager.rm.transform.parent.gameObject);
        RoomManager.rm = null;
        LevelManager.lm.LoadFarmDeath();
    }

    void UpdateHealth() {
        healthFill.fillAmount = healthCurrent / healthTotal;
    }

    public void LoseMoney(float i) {
        money -= i;
        if(money < 0) {
            money = 0;
        }
        SetMoneyText();
    }

    public void GainMoney(float i) {
        money += i;
        SetMoneyText();
    }

    void SetMoneyText() {
        moneyCountText.text = Mathf.FloorToInt(money).ToString();
    }
}
