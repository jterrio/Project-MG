using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {


    [Header("Health")]
    public float healthTotal = 50f;
    [HideInInspector]
    public float health = 50f;

    [Header("Health Bar References")]
    public RectTransform healthBar;
    public Image healthBarFill;

    [Header("Health Bar Settings")]
    public float timeToDisplay = 3f;
    public bool alwaysDisplay = false;
    private float lastTimeTakenDamage = 0f;
    private bool hasTakenDamageRecently = false;

    [Header("Sounds")]
    public AudioSource audioSource;
    public AudioClip[] audioClips;

    private void Start() {
        health = healthTotal;
    }

    protected void Update() {
        if (lastTimeTakenDamage + timeToDisplay > Time.time && hasTakenDamageRecently) {
            healthBar.gameObject.SetActive(true);
            healthBar.transform.LookAt(GameManager.gm.player.transform);
        } else {
            healthBar.gameObject.SetActive(false);
            hasTakenDamageRecently = false;
        }
    }

    public virtual void TakeDamage(float amount) {
        lastTimeTakenDamage = Time.time;
        hasTakenDamageRecently = true;
        health -= amount;
        healthBarFill.fillAmount = health / healthTotal;
        if(health <= 0f) {
            Die();
        }
    }

    protected void Die() {
        RoomManager.rm.DefeatMonster(this.gameObject);
    }

}
