using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnipLord : Enemy {

    //-7 + 7
    [Header("Boss Settings")]
    [Tooltip("Boss Attack Stage")]
    public State state = State.GROW;
    [Tooltip("Time the screen shakes before anything actually happens")]
    public float shakeBeforeRiseTime = 3f;
    [Tooltip("How fast the boss rises and the battle begins")]
    public float riseSpeed = 3f;
    private Coroutine harvestCoroutine;
    private List<Coroutine> vineCoroutines;
    private List<GameObject> spawnedVines;
    [Header("Attack Settings")]
    [Tooltip("Delay between stages of vine attack. Lower value means faster attacks")]
    public float vineAttackSpeed = 2f;
    private float lastTimeAttacked = 0f;
    [Tooltip("Delay between stages between attack. Higher value means slower attacks")]
    public float timeBetweenAttacks = 5f;
    [Tooltip("Delay between stages between spawning. Higher value means slower spawns")]
    public float minionAttackSpeed = 1.5f;
    private bool vineAttackDone = true;
    private bool minionAttackDone = true;

    [Header("Boss References")]
    public GameObject vineObject;
    public GameObject minion;
    public GameObject minionSpawnPoint;

    [Header("Camera Shake")]
    public float magnitude;

    [Header("Sound Settings")]
    [Tooltip("How loud the ground crack should be")]
    [Range(0, 1)]
    public float groundCrackVolume = 1;
    public AudioClip groundCrack;
    [Tooltip("How loud the rumble should be")]
    [Range(0, 1)]
    public float rumbleVolume = 1;
    public AudioClip rumble;
    [Tooltip("How loud the spawn minion should be")]
    [Range(0, 1)]
    public float spawnMinionVolume = 1;
    public AudioClip spawnMinion;
    [Tooltip("How loud the spawn vine should be")]
    [Range(0, 1)]
    public float spawnVineVolume = 1;
    public AudioClip spawnVine;
    [Tooltip("How loud the vine ground break should be")]
    [Range(0, 1)]
    public float vineGroundBreakVolume = 1;
    public AudioClip vineGroundBreak;

    public enum State {
        GROW,
        HARVEST,
        READY,
        VINE,
        SPAWN,
        BOTH,
        ISATTACKING
    }

    private new void Start() {
        transform.position = new Vector3(transform.position.x, transform.position.y - 7, transform.position.z);
        vineCoroutines = new List<Coroutine>();
        spawnedVines = new List<GameObject>();
    }

    // Update is called once per frame
    new void Update() {
        switch (state) {
            case State.GROW:
                state = State.HARVEST;
                break;
            case State.HARVEST:
                if(harvestCoroutine == null) {
                    harvestCoroutine = StartCoroutine(Harvest());
                }
                break;
            case State.READY:
                //transform.LookAt(GameManager.gm.player.transform.position);
                float h = GetHealthPercentage();
                if (h > 0.6f) {
                    HighHPAttack();
                } else if (h > 0.05f) {
                    LowHPAttack();
                } else {
                    NearDeathAttack();
                }
                break;
            case State.VINE:
                state = State.ISATTACKING;
                VineAttack();
                break;
            case State.SPAWN:
                state = State.ISATTACKING;
                SpawnAttack();
                break;
            case State.BOTH:
                state = State.ISATTACKING;
                VineAttack();
                SpawnAttack();
                break;
            case State.ISATTACKING:
                LookAt(GameManager.gm.player);
                if (minionAttackDone && vineAttackDone) {
                    state = State.READY;
                }
                break;
        }
    }


    void HighHPAttack() {
        if (lastTimeAttacked + (timeBetweenAttacks) < Time.time) {
            int r = Random.Range(0, 10);
            if (r < 4) {
                state = State.VINE;
            } else if (r < 10) {
                state = State.SPAWN;
            } else {
                state = State.BOTH;
            }
        }
    }

    void LowHPAttack() {
        if (lastTimeAttacked + (timeBetweenAttacks) < Time.time) {
            int r = Random.Range(0, 20);
            if(r < 2) {
                state = State.VINE;
            }else if(r < 4) {
                state = State.SPAWN;
            } else {
                state = State.BOTH;
            }
        }
    }

    void NearDeathAttack() {
        if (lastTimeAttacked + (timeBetweenAttacks) < Time.time) {
            state = State.SPAWN;
        }
    }

    void SpawnAttack() {
        minionAttackDone = false;
        Coroutine m = StartCoroutine(SpawnAttackSpawn(Mathf.FloorToInt(Random.Range(3, 5) * (1/(GetHealthPercentage() + 0.01f)))));
        vineCoroutines.Add(m);
    }

    float GetHealthPercentage() {
        return health / healthTotal;
    }

    IEnumerator SpawnAttackSpawn(int n) {
        int nCount = 0;
        while (nCount < n) {
            PlaySound(spawnMinion, spawnMinionVolume);
            nCount++;
            float randomAngle = Random.Range(0f, Mathf.PI * 2f);
            Vector3 spawnPoint = minionSpawnPoint.transform.position + (new Vector3(Mathf.Sin(randomAngle), 0f, Mathf.Cos(randomAngle)).normalized * 10f);
            GameObject m = Instantiate(minion);
            m.transform.position = new Vector3(spawnPoint.x, m.GetComponent<Turnip>().growth, spawnPoint.z);
            RoomManager.rm.currentRoom.monsters.Add(m);
            yield return new WaitForSeconds(Mathf.Max(0.2f, minionAttackSpeed * GetHealthPercentage()));
        }
        lastTimeAttacked = Time.time;
        minionAttackDone = true;
    }

    void VineAttack() {
        vineAttackDone = false;
        Coroutine c = StartCoroutine(VineAttackSpawn(Random.Range(5, 20)));
        vineCoroutines.Add(c);
    }

    IEnumerator VineAttackSpawn(int n) {
        int nCount = 0;
        while (nCount < n) {
            nCount++;
            Coroutine v = StartCoroutine(SpawnVine());
            yield return new WaitForSeconds(vineAttackSpeed / 3);
        }
        lastTimeAttacked = Time.time;
        vineAttackDone = true;
    }

    IEnumerator SpawnVine() {
        GameObject v = Instantiate(vineObject);
        Vine vine = v.GetComponentInChildren<Vine>();
        spawnedVines.Add(v);
        GameObject vChild = vine.vine;
        int r = Random.Range(0, 2);
        if (r == 0) {
            v.transform.position = new Vector3(GameManager.gm.player.transform.position.x + (GameManager.gm.playerRB.velocity.normalized.x * 15), 0f, GameManager.gm.player.transform.position.z + (GameManager.gm.playerRB.velocity.normalized.z * 15));
        } else {
            v.transform.position = new Vector3(GameManager.gm.player.transform.position.x, 0, GameManager.gm.player.transform.position.z);
        }
        vChild.transform.localPosition = new Vector3(0, -0.4f, 0);
        PlaySound(spawnVine, spawnVineVolume, vine.transform.position);
        yield return new WaitForSeconds(Mathf.Max(0.5f, vineAttackSpeed * ((GetHealthPercentage() + 0.01f))));
        PlaySound(vineGroundBreak, vineGroundBreakVolume, vine.transform.position);
        while (vChild.transform.position.y < 0f) {
            vChild.transform.localPosition = new Vector3(0, vChild.transform.localPosition.y + Mathf.Max(0.5f, vineAttackSpeed * ((GetHealthPercentage() + 0.01f))), 0);
            yield return null;
        }
        vChild.transform.localPosition = new Vector3(0, 0f, 0);
        yield return new WaitForSeconds(vineAttackSpeed * 0.1f);
        vine.hasLift = false;
        yield return new WaitForSeconds(vineAttackSpeed * 1.9f);
        spawnedVines.Remove(v);
        Destroy(v);
    }


    IEnumerator Harvest() {
        yield return new WaitForSeconds(shakeBeforeRiseTime);
        Vector3 originalCamPos = GameManager.gm.playerCamera.transform.localPosition;
        float elapsed = 0f;
        bool hasPlayedSound = false;
        GameManager.gm.p.audioSource.PlayOneShot(rumble, rumbleVolume);
        while (transform.position.y < 2f) {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            GameManager.gm.playerCamera.transform.localPosition = new Vector3(x, y, originalCamPos.z);
            if (elapsed < shakeBeforeRiseTime) {
                elapsed += Time.deltaTime;
            } else {
                if (!hasPlayedSound) {
                    hasPlayedSound = true;
                    GameManager.gm.p.audioSource.PlayOneShot(groundCrack, groundCrackVolume);
                }
                transform.position = new Vector3(transform.position.x, transform.position.y + (riseSpeed * Time.deltaTime), transform.position.z);
            }
            yield return null;
        }
        GameManager.gm.playerCamera.transform.localPosition = originalCamPos;
        yield return new WaitForSeconds(shakeBeforeRiseTime / 2);
        //play music
        healthBar.gameObject.SetActive(true);
        state = State.READY;
        yield return null;
    }

    public void BeginGrowing() {
        state = State.HARVEST;
    }

    void StopAllVineCoroutines() {
        foreach(Coroutine c in vineCoroutines) {
            StopCoroutine(c);
        }
        foreach(GameObject v in spawnedVines) {
            Destroy(v);
        }
    }

    public override void TakeDamage(float amount) {
        if(state == State.GROW || state == State.HARVEST) {
            return;
        }
        health -= amount;
        healthBarFill.fillAmount = health / healthTotal;
        if (health <= 0f) {
            StopAllVineCoroutines();
            ExplodeMinions();
            Die();
        }
    }

    void ExplodeMinions() {
        foreach(GameObject g in RoomManager.rm.currentRoom.monsters) {
            if(g.gameObject == this.gameObject) {
                continue;
            }
            Turnip t = g.GetComponent<Turnip>();
            t.explosionDMG = 0;
            t.growState = Turnip.State.BOOM;
            t.explosionDelay = Random.Range(0.0f, 2f);
            t.explosionVolume = 0.1f;
        }
    }
}
