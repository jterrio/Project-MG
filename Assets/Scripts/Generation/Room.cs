using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {


    public Node[,] roomArray;

    [Header("Node Settings")]
    [Tooltip("Number of nodes within the room in X and Z space")]
    public int xLength, yLength;
    [Tooltip("Size of node in world space")]
    public float nodeLength;

    [Header("References")]
    [Tooltip("Reference to size of the floor space for calculations and spawning")]
    public GameObject floorBase;
    [Tooltip("Debug Object")]
    public GameObject testObject;
    public GameObject floorGrass;

    [Header("Enemy Generation")]
    [Tooltip("Potential enemy spawns during generation")]
    public List<GameObject> potentialMonsterSpawns;
    [Tooltip("Enemies that were spawned during generation")]
    public List<GameObject> monsters;
    [Tooltip("Determines if enemies spawn in this room, like a shop or spawn room")]
    public bool specialRoom;
    [Tooltip("Determines if this is a boss room")]
    public bool bossRoom;

    [Header("Connectors")]
    [Tooltip("Doors that lead to other rooms")]
    public GameObject[] gates;
    [Tooltip("Entrance triggers for entering a new room")]
    public GameObject[] triggers;
    [Tooltip("Walls for connectors that have no neighbor")]
    public GameObject[] blockers;
    [Tooltip("Neighbor rooms")]
    public GameObject[] neighbors;
    [Tooltip("Max number of neighbors")]
    public int maxNeighbors;

    [System.Serializable]
    public class Node {
        public int xPos;
        public int yPos;
        public Vector3 position;
        public bool isTaken = false;
    }

    private void Start() {
        CreateGrid();
        GenerateRoom();
    }

    void CreateGrid() {
        roomArray = new Node[xLength, yLength];
        float roomRadius = floorBase.transform.localScale.x / 2;
        Vector3 firstNode = new Vector3(floorBase.transform.position.x - roomRadius + (nodeLength / 2), floorBase.transform.position.y + 1, floorBase.transform.position.z + roomRadius - (nodeLength / 2));
        for(int i = 0; i < xLength; i++) {
            for(int t = 0; t < yLength; t++) {
                Node n = new Node {
                    xPos = i,
                    yPos = t,
                    position = new Vector3(firstNode.x + (nodeLength * i), firstNode.y, firstNode.z - (nodeLength * t))
                };
                roomArray[i, t] = n;
                GameObject grass = Instantiate(floorGrass);
                grass.transform.position = new Vector3(firstNode.x + (nodeLength * i), 0.3f, firstNode.z - (nodeLength * t));
                grass.transform.localScale = new Vector3(0.7f, 1f, 0.7f);
            }
        }
    }

    void GenerateRoom() {
        if(!specialRoom && !bossRoom) {
            SpawnMonsters();
        }else if (specialRoom) {
            //spawn or shop
        } else {
            //boss
        }
    }

    void SpawnMonsters() {
        int x = Random.Range(0, xLength - 1);
        int y = Random.Range(0, yLength - 1);
        roomArray[x, y].isTaken = true;
        int m = Random.Range(0, potentialMonsterSpawns.Count - 1);
        int c = Random.Range(3, 7);
        for (int i = 0; i < c; i++) {
            Vector3 spawnPoint = new Vector3(roomArray[x, y].position.x + Random.Range(-(nodeLength / 2), (nodeLength / 2)), roomArray[x, y].position.y, roomArray[x, y].position.z + Random.Range(-(nodeLength / 2), (nodeLength / 2)));
            GameObject monster = Instantiate(potentialMonsterSpawns[m]);
            monster.transform.position = spawnPoint;
            monsters.Add(monster);
        }

        if (RoomManager.rm.currentRoom != this) {
            DeactivateMonsters();
        }
    }

    public void DefeatMonster(GameObject monster) {
        monsters.Remove(monster);
        Destroy(monster);
        if(monsters.Count == 0) {
            ClearRoom();
        }
    }

    public void ClearRoom() {
        ClearTriggers();
        RemoveTriggers();
        RemoveGates();
        AccessNeighbors();
    }

    public void AccessNeighbors() {
        for(int i = 0; i < neighbors.Length; i++) {
            Room r = neighbors[i].GetComponent<Room>();
            r.RemoveGates();
            r.AddTriggers();
        }
    }

    public void DeactivateMonsters() {
        foreach(GameObject m in monsters) {
            m.SetActive(false);
        }
    }

    public void ActivateMonsters() {
        foreach (GameObject m in monsters) {
            m.SetActive(true);
        }
    }

    public void RemoveGates() {
        for (int i = 0; i < gates.Length; i++) {
            gates[i].SetActive(false);
        }
    }

    public void AddGates() {
        for (int i = 0; i < gates.Length; i++) {
            gates[i].SetActive(true);
        }
    }

    public void ClearTriggers() {
        for (int i = 0; i < triggers.Length; i++) {
            triggers[i].GetComponent<Gate>().hasCleared = true;
        }
    }

    public void RemoveTriggers() {
        for (int i = 0; i < triggers.Length; i++) {
            triggers[i].SetActive(false);
        }
    }

    public void AddTriggers() {
        for (int i = 0; i < triggers.Length; i++) {
            triggers[i].SetActive(true);
        }
    }

    public void RemoveGate(int p) {
        gates[p].SetActive(false);
    }

    public void RemoveTrigger(int p) {
        triggers[p].SetActive(false);
    }

    public void AddGate(int p) {
        gates[p].SetActive(true);
    }

    public void AddTrigger(int p) {
        triggers[p].SetActive(true);
    }
}
