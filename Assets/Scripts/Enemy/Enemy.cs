using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : Actor {

    [Header("Spawn Settings")]
    public EnemyType et;
    [Min(0)]
    public int maxSpawn = 0;
    public int enemyID;

    [Header("Health Bar References")]
    public RectTransform healthBar;
    public Image healthBarFill;

    [Header("Health Bar Settings")]
    public float timeToDisplay = 3f;
    public bool alwaysDisplay = false;
    protected float lastTimeTakenDamage = 0f;
    protected bool hasTakenDamageRecently = false;

    [Header("Physics")]
    public Rigidbody rb;
    public bool canFly = false;

    [Header("Rewards")]
    public float minMoneyReward = 0f;
    public float maxMoneyReward = 0f;

    [Header("Audio Source")]
    public AudioSource audioSource;


    public enum EnemyType{
        NORMAL
    }

    protected void Start() {
        health = healthTotal;
        DisableCollisions();
    }


    protected void DisableCollisions() {
        rb.isKinematic = true;
    }

    protected void EnableCollisions() {
        rb.isKinematic = false;
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
        GameManager.gm.SetLastTimeDamaged();
        health -= amount;
        healthBarFill.fillAmount = health / healthTotal;
        if(health <= 0f) {
            Die();
        }
    }

    protected void Die() {
        RoomManager.rm.DefeatMonster(this.gameObject);
        GiveMoneyReward();
    }

    protected void DelayDie(float d) {
        RoomManager.rm.DefeatMonster(this.gameObject, d);
        GiveMoneyReward();
    }

    void GiveMoneyReward() {
        GameManager.gm.p.GainMoney(Random.Range(minMoneyReward, maxMoneyReward));
    }

    protected void LookAt(GameObject g) {
        Vector3 targetPos = new Vector3(g.transform.position.x, this.transform.position.y, g.transform.position.z);
        //transform.LookAt(targetPos);
        rb.rotation = Quaternion.LookRotation(targetPos - transform.position);
    }

    /// <summary>
    /// Plays sound at the monster's location
    /// </summary>
    /// <param name="ac">Clip to play</param>
    /// <param name="vol">Volume</param>
    public void PlaySound(AudioClip ac, float vol) {
        AudioSource.PlayClipAtPoint(ac, transform.position, vol);
    }

    /// <summary>
    /// Plays sound at a given locations, such as for a monster summon
    /// </summary>
    /// <param name="ac">Clip to play</param>
    /// <param name="vol">Volume</param>
    /// <param name="pos">Position</param>
    public void PlaySound(AudioClip ac, float vol, Vector3 pos) {
        AudioSource.PlayClipAtPoint(ac, pos, vol);
    }

}
