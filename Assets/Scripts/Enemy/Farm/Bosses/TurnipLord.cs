using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnipLord : Enemy {

    //-7 + 7
    [Header("Boss Settings")]
    public State state = State.GROW;
    public float shakeBeforeRiseTime = 3f;
    public float riseSpeed = 3f;
    private Coroutine harvestCoroutine;
    private List<Coroutine> vineCoroutines;
    private List<GameObject> spawnedVines;
    public float vineAttackSpeed = 2f;
    public float lastTimeAttacked = 0f;
    public float timeBetweenAttacks = 5f;

    [Header("Boss References")]
    public GameObject vineObject;
    public GameObject minion;

    [Header("Camera Shake")]
    public float magnitude;

    public enum State {
        GROW,
        HARVEST,
        READY,
        VINE,
        SPAWN,
        BOTH,
        ISATTACKING
    }

    private void Start() {
        transform.position = new Vector3(transform.position.x, transform.position.y - 7, transform.position.z);
        vineCoroutines = new List<Coroutine>();
        spawnedVines = new List<GameObject>();
    }

    // Update is called once per frame
    new void Update() {
        if(state == State.GROW) {
            state = State.HARVEST;
        }
        if(state == State.HARVEST && harvestCoroutine == null) {
            harvestCoroutine = StartCoroutine(Harvest());
        }
        if(state == State.READY) {
            transform.LookAt(GameManager.gm.player.transform.position);

            if(lastTimeAttacked + timeBetweenAttacks < Time.time) {
                state = State.VINE;
            }
        }
        if(state == State.VINE) {
            state = State.ISATTACKING;
            VineAttack();
        }
        if(state == State.SPAWN) {

        }
        if(state == State.ISATTACKING) {
            transform.LookAt(GameManager.gm.player.transform.position);
            return;
        }
    }

    void VineAttack() {
        Coroutine c = StartCoroutine(VineAttackSpawn(Random.Range(3, 10)));
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
        state = State.READY;
        yield return null;
    }

    IEnumerator SpawnVine() {
        GameObject v = Instantiate(vineObject);
        spawnedVines.Add(v);
        GameObject vChild = v.GetComponentInChildren<Vine>().vine;
        v.transform.position = new Vector3(GameManager.gm.player.transform.position.x, 0, GameManager.gm.player.transform.position.z); ;
        vChild.transform.localPosition = new Vector3(0, -0.35f, 0);
        yield return new WaitForSeconds(vineAttackSpeed);
        while (vChild.transform.position.y < 0.1f) {
            vChild.transform.localPosition = new Vector3(0, vChild.transform.localPosition.y + (vineAttackSpeed * Time.deltaTime), 0);
            yield return null;
        }
        yield return new WaitForSeconds(vineAttackSpeed);
        spawnedVines.Remove(v);
        Destroy(v);
    }


    IEnumerator Harvest() {
        yield return new WaitForSeconds(shakeBeforeRiseTime);
        Vector3 originalCamPos = GameManager.gm.playerCamera.transform.localPosition;
        float elapsed = 0f;
        bool hasPlayedSound = false;
        audioSource.PlayOneShot(audioClips[0]);
        while (transform.position.y < 2f) {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            GameManager.gm.playerCamera.transform.localPosition = new Vector3(x, y, originalCamPos.z);
            if (elapsed < shakeBeforeRiseTime) {
                elapsed += Time.deltaTime;
            } else {
                if (!hasPlayedSound) {
                    hasPlayedSound = true;
                    audioSource.PlayOneShot(audioClips[1]);
                }
                transform.position = new Vector3(transform.position.x, transform.position.y + (riseSpeed * Time.deltaTime), transform.position.z);
            }
            yield return null;
        }
        GameManager.gm.playerCamera.transform.localPosition = originalCamPos;
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
            Die();
        }
    }
}
