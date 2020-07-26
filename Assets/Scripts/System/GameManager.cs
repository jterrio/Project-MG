using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Floor")]
    [Tooltip("The number of the current floor the player is on")]
    public int currentFloor = 0;
    [Tooltip("Minimap camera for the floor")]
    public Minimap mm;
    public GameObject playerMinimapObject;

    private void Awake() {
        if(gm == null) {
            gm = this;
        }else if(gm != this) {
            Destroy(this);
        }
        DontDestroyOnLoad(this);
    }

    public void FindPlayer() {
        player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
        playerCamera = player.GetComponentInChildren<Camera>();

        //playerMinimapObject = Instantiate(playerMinimapObject);
        mm = GameObject.FindGameObjectWithTag("Minimap").GetComponent<Minimap>();
    }

}
