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
    public GameObject walls;
    [Tooltip("Debug Object")]
    public GameObject floorGrass;
    public GameObject minimapObject;

    [Header("Environment Generation")]
    [Tooltip("Min environment that will spawn")]
    public int minEnvironment;
    [Tooltip("Max environment that will spawn")]
    public int maxEnvironment;
    [Tooltip("Min environment that will spawn inside individual node")]
    public int minPatch;
    [Tooltip("Max environment that will spawn inside individual node")]
    public int maxPatch;
    [Range(0, 1)]
    [Tooltip("Threshold for spawning the environment per node. Higher means less will spawn")]
    public float perlinThreshold;
    [Tooltip("Potential environmental spawns during generation")]
    public GameObject envSpawn;
    [Tooltip("Environment that was spawned during generation")]
    public List<GameObject> environments;

    [Header("Blockade Generation")]
    [Tooltip("Min environment that will spawn")]
    public int minBlockade;
    [Tooltip("Max environment that will spawn")]
    public int maxBlockade;
    [Tooltip("Potential environmental spawns during generation")]
    public GameObject blockadeSpawn;
    [Tooltip("Environment that was spawned during generation")]
    public List<GameObject> blockades;



    [Header("Enemy Generation")]
    [Tooltip("Min monsters that will spawn")]
    public int minMonsterSpawn;
    [Tooltip("Max monsters that will spawn")]
    public int maxMonsterSpawn;
    [Tooltip("Potential enemy spawns during generation")]
    public List<GameObject> potentialMonsterSpawns;
    [Tooltip("Enemies that were spawned during generation")]
    public List<GameObject> monsters;
    [Tooltip("Bosses that can be spawned in a boss room")]
    public List<GameObject> bosses;
    [Tooltip("Determines if enemies spawn in this room, like a shop or spawn room")]
    public bool specialRoom;
    [Tooltip("Determines if this is a boss room")]
    public bool bossRoom;

    [Header("Connectors")]
    [Tooltip("The entire connector")]
    public GameObject[] connectors;
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
                grass.transform.parent = walls.transform;
                grass.transform.position = new Vector3(firstNode.x + (nodeLength * i), 0.3f, firstNode.z - (nodeLength * t));
                grass.transform.localScale = new Vector3(0.7f, 1f, 0.7f);
            }
        }
    }

    void GenerateRoom() {
        SpawnEnvironment();
        if (!specialRoom && !bossRoom) {
            SpawnMonsters();
        }else if (specialRoom) {
            //spawn or shop
        } else {
            SpawnBoss();
        }
    }



    void SpawnEnvironment() {
        int timeout = 0;
        int envToSpawn = Random.Range(minEnvironment, maxEnvironment);
        HashSet<Vector2> hs = new HashSet<Vector2>();
        while(environments.Count < envToSpawn && timeout < ((xLength - 1) * (yLength - 1))) {
            int x = Random.Range(0, xLength - 1);
            int y = Random.Range(0, yLength - 1);
            if (hs.Contains(new Vector2(x, y))) {
                timeout++;
                continue;
            }
            hs.Add(new Vector2(x, y));
            if (GameManager.gm.seed.ShouldSpawn(x, y, xLength, yLength, perlinThreshold)) {
                int r = Random.Range(minPatch, maxPatch);
                for (int i = 0; i < r; i++) {
                    Vector3 spawnPoint = new Vector3(roomArray[x, y].position.x + Random.Range(-(nodeLength / 2), (nodeLength / 2)), 0.5f, roomArray[x, y].position.z + Random.Range(-(nodeLength / 2), (nodeLength / 2)));
                    GameObject env = Instantiate(envSpawn);
                    env.transform.parent = this.gameObject.transform;
                    env.transform.position = spawnPoint;
                    environments.Add(env);
                }
            }



        }
    }

    void SpawnMonsters() {

        int timeout = 0;
        int monstersToSpawn = Random.Range(minMonsterSpawn, maxMonsterSpawn);
        while (monsters.Count < monstersToSpawn || timeout >= ((xLength - 1) * (yLength - 1))) {
            int x = Random.Range(0, xLength - 1);
            int y = Random.Range(0, yLength - 1);
            if (roomArray[x, y].isTaken) {
                timeout++;
                continue;
            }

            roomArray[x, y].isTaken = true;
            timeout = 0;
            int m = Random.Range(0, potentialMonsterSpawns.Count);
            Vector3 spawnPoint = new Vector3(roomArray[x, y].position.x + Random.Range(-(nodeLength / 2), (nodeLength / 2)), roomArray[x, y].position.y, roomArray[x, y].position.z + Random.Range(-(nodeLength / 2), (nodeLength / 2)));
            GameObject monster = Instantiate(potentialMonsterSpawns[m]);
            monster.transform.parent = this.gameObject.transform;
            monster.transform.position = spawnPoint;
            monsters.Add(monster);
        }

        if (RoomManager.rm.currentRoom != this) {
            DeactivateMonsters();
        }
    }

    void SpawnBoss() {
        GameObject boss = Instantiate(bosses[0]);
        boss.transform.parent = this.gameObject.transform;
        boss.transform.position = roomArray[xLength / 2, yLength / 2].position;
        monsters.Add(boss);
        DeactivateMonsters();
    }

    public void DefeatMonster(GameObject monster) {
        monsters.Remove(monster);
        Destroy(monster);
        if(monsters.Count == 0) {
            ClearRoom();
        }
    }

    public void DefeatMonster(GameObject monster, float d) {
        monsters.Remove(monster);
        Destroy(monster, d);
        if (monsters.Count == 0) {
            ClearRoom();
        }
    }

    public void ClearRoom() {
        if (bossRoom) {
            ClearFloor();
        }
        SetMinimapExplored();
        ClearTriggers();
        RemoveTriggers();
        RemoveGates();
        AccessNeighbors();
    }

    public void SetMinimapExplored() {
        GameManager.gm.mm.SetExplored(minimapObject);
        for (int i = 0; i < neighbors.Length; i++) {
            GameManager.gm.mm.SetUnexplored(neighbors[i].GetComponent<Room>().minimapObject);
        }
    }

    public void ClearFloor() {
        GameObject f = Instantiate(RoomManager.rm.nextFloor);
        f.gameObject.transform.position = new Vector3(floorBase.transform.position.x, 3f, floorBase.transform.position.z);
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
