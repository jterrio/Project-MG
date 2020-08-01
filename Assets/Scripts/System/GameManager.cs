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
    public LayerMask minimap;

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

}
