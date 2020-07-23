﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager gm;

    [Header("Player")]
    [Tooltip("Gameobject of the player")]
    public GameObject player;
    [Tooltip("Reference to the players movement")]
    public PlayerMovement playerMovement;
    [Tooltip("Reference to the players camera")]
    public Camera playerCamera;

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
    }

}
