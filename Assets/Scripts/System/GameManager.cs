using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Seed))]
public class GameManager : MonoBehaviour {

    public static GameManager gm;

    [Header("Player")]
    [Tooltip("Gameobject of the player")]
    public GameObject player;
    [Tooltip("Reference to the players player")]
    public Player p;
    [Tooltip("Reference to the players movement")]
    public PlayerMovement playerMovement;
    [Tooltip("Reference to the players camera")]
    public Camera playerCamera;
    [Tooltip("Reference to the players rigidbody")]
    public Rigidbody playerRB;

    [Header("Persistence")]
    public int wepID;
    public GameObject[] weapons;
    public Seed seed;

    [Header("Floor")]
    [Tooltip("The number of the current floor the player is on")]
    public int currentFloor = 0;
    [Tooltip("Minimap camera for the floor")]
    public Minimap mm;
    public GameObject playerMinimapObject;

    [Header("Rooms")]
    public GameObject compass;
    [Tooltip("In seconds, how long until pity system kicks in and shows where enemies are")]
    public float timeoutBeforePity = 15f;
    public bool pityEnabled;
    private float lastTimeKilledEnemy;

    [Header("Layer Mask")]
    public LayerMask enemyPlayerLayers;
    public LayerMask enemyLayers;
    public LayerMask LOS;
    public LayerMask minimap;
    public LayerMask groundMask;
    public LayerMask groundEnemy;

    public enum State {
        MAINMENU,
        GAMEPLAY
    }


    private void Awake() {
        if(gm == null) {
            gm = this;
        }else if(gm != this) {
            Destroy(this);
        }
        DontDestroyOnLoad(this);
        AudioSource a;
    }

    private void Update() {
        if (RoomManager.rm != null) {
            CheckPity();
        }
    }




    public void SetLastTimeDamaged() {
        lastTimeKilledEnemy = Time.time;
    }

    public void EnablePity() {
        pityEnabled = true;
        compass.gameObject.SetActive(true);
    }

    public void DisablePity() {
        pityEnabled = false;
        compass.gameObject.SetActive(false);
    }

    public void CheckPity() {
        if(RoomManager.rm.currentRoom.roomType == Room.RoomType.BOSS) {
            return;
        }
        if (pityEnabled && RoomManager.rm.currentRoom.monsters.Count == 0) {
            DisablePity();
        } else if(!pityEnabled && RoomManager.rm.currentRoom.monsters.Count > 0) {
            if(Time.time >= lastTimeKilledEnemy + timeoutBeforePity) {
                EnablePity();
            }
        }
    }


    /// <summary>
    /// Set the Player object if it null and destroy an existing one if it exists (from changing levels)
    /// </summary>
    public void FindPlayer() {
        if(player == null) {
            player = GameObject.FindGameObjectWithTag("Player");
            playerMovement = player.GetComponent<PlayerMovement>();
            playerCamera = player.GetComponentInChildren<Camera>();
            p = player.GetComponent<Player>();
            playerRB = player.GetComponent<Rigidbody>();
        } else {
            foreach (GameObject c in GameObject.FindGameObjectsWithTag("Player")) {
                if(c != player) {
                    Destroy(c);
                }
            }
        }
        //player = GameObject.FindGameObjectWithTag("Player");

        //playerMinimapObject = Instantiate(playerMinimapObject);
        mm = GameObject.FindGameObjectWithTag("Minimap").GetComponent<Minimap>();
    }

    /// <summary>
    /// Give the player their weapon of choice
    /// </summary>
    /// <param name="id">Weapon id</param>
    public void LoadWeapon(int id) {
        GameObject g = Instantiate(weapons[wepID]);
        if (p.gun != null) {
            Destroy(p.gun.gameObject);
        }
        g.transform.parent = p.weaponHolder.transform;
        p.gun = g.GetComponent<Gun>();
        p.gun.bulletEmitter = p.bulletSpawn;
        g.transform.localPosition = Vector3.zero;
        g.transform.localRotation = Quaternion.Euler(Vector3.zero);
        g.transform.localScale = new Vector3(1, 1, 1);
    }

    
    /// <summary>
    /// TEMP--Cycle different weapon options available on the menu level--TEMP
    /// </summary>
    public void CycleWeapon() {
        wepID = (wepID + 1) % (weapons.Length);
        LoadWeapon(wepID);
    }

    /// <summary>
    /// Determine if there is line of sight between the two objects (monster -> player)
    /// </summary>
    /// <param name="host">Monster</param>
    /// <param name="target">Player</param>
    /// <returns></returns>
    public bool HasLineOfSight(GameObject host, GameObject target) {
        RaycastHit hit;
        Vector3 hostPos = host.transform.position;
        Vector3 targetPos = target.transform.position;

        Vector3 direction = targetPos - hostPos;
        if (Physics.Raycast(hostPos, direction, out hit, Vector3.Distance(hostPos, targetPos), LOS)) {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player")) {
                //Debug.DrawRay(hostPos, direction, Color.green);
                return true;
            } else {
                Debug.DrawRay(hostPos, direction, Color.red, 10f);
                return false;
            }
        }
        Debug.DrawRay(hostPos, direction, Color.blue, 10f);
        return true;
    }

    public bool IsBlockedByGroundToPlayer(GameObject monster) {
        RaycastHit hit;
        Vector3 hostPos = monster.transform.position;
        Vector3 targetPos = player.transform.position;

        Vector3 direction = targetPos - hostPos;
        if (Physics.Raycast(hostPos, direction, out hit, Vector3.Distance(hostPos, targetPos), groundMask)) {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player")) {
                //Debug.DrawRay(hostPos, direction, Color.green);
                return false;
            } else {
                Debug.DrawRay(hostPos, direction, Color.red, 10f);
                return true;
            }
        }
        Debug.DrawRay(hostPos, direction, Color.blue, 10f);
        return false;
    }

    public bool HasLineOfSightToPlayer(GameObject monster) {
        return HasLineOfSight(monster, player);
    }

}
