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

    [Header("References")]
    public LayerMask enemyPlayerLayers;
    public LayerMask LOS;

    private void Awake() {
        if(gm == null) {
            gm = this;
        }else if(gm != this) {
            Destroy(this);
        }
        DontDestroyOnLoad(this);
    }

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

    public void LoadWeapon(int id) {
        GameObject g = Instantiate(weapons[wepID]);
        if (p.gun != null) {
            Destroy(p.gun.gameObject);
        }
        g.transform.parent = p.weaponHolder.transform;
        p.gun = g.GetComponent<Gun>();
        g.transform.localPosition = Vector3.zero;
        g.transform.localRotation = Quaternion.Euler(Vector3.zero);
        g.transform.localScale = new Vector3(1, 1, 1);
    }

    public void CycleWeapon() {
        wepID = (wepID + 1) % (weapons.Length);
        LoadWeapon(wepID);
    }

    public bool HasLineOfSight(GameObject host, GameObject target) {
        RaycastHit hit;
        Vector3 direction = target.transform.position - host.transform.position;
        if (Physics.Raycast(host.transform.position, direction, out hit, Mathf.Infinity, LOS)) {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player")) {
                Debug.DrawRay(GameManager.gm.player.transform.position, direction, Color.green);
                return true;
            } else {
                Debug.DrawRay(GameManager.gm.player.transform.position, direction, Color.red);
                return false;
            }
        }
        return false;
    }

}
