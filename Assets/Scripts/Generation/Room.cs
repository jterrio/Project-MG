using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {


    public Node[,] roomArray;

    [Header("Room Settings")]
    public RoomType roomType = RoomType.NORMAL;

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
    public float envPerlinThreshold;
    [Tooltip("Potential environmental spawns during generation")]
    public GameObject envSpawn;
    [Tooltip("Environment that was spawned during generation")]
    public List<GameObject> environments;

    [Header("Blockade Generation")]
    [Tooltip("Min environment that will spawn")]
    public int minBlockade;
    [Tooltip("Max environment that will spawn")]
    public int maxBlockade;
    [Tooltip("Min length of a wall segment")]
    public int minLength;
    [Tooltip("Max length of a wall segment")]
    public int maxLength;
    [Tooltip("Threshold for spawning the walls per node. Higher means less will spawn")]
    public float blockPerlinThreshold;
    [Tooltip("Potential blockade spawns during generation")]
    public GameObject blockadeSpawn;
    [Tooltip("Potential blockade corner spawns during generation")]
    public GameObject blockadeCornerSpawn;
    [Tooltip("Blockades that was spawned during generation")]
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

    [Header("Shop Generation")]
    [Tooltip("Model used for the shopkeeper")]
    public GameObject shopKeeper;
    [Tooltip("Pedestal and trigger for the item where it shall rest")]
    public GameObject itemPlace;

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

    public enum RoomType {
        START,
        SHOP,
        BOSS,
        NORMAL
    }

    private void Start() {
        CreateGrid();
        GenerateRoom();
    }

    /// <summary>
    /// Creates a grid within the room and creates the floors
    /// </summary>
    void CreateGrid() {
        roomArray = new Node[xLength, yLength];
        float roomRadius = floorBase.transform.localScale.x / 2;
        Vector3 firstNode = new Vector3(floorBase.transform.position.x - roomRadius + (nodeLength / 2), floorBase.transform.position.y + 1, floorBase.transform.position.z + roomRadius - (nodeLength / 2));

        //Floor generation
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

    /// <summary>
    /// Adds things inside the room, depending on the type
    /// </summary>
    void GenerateRoom() {
        //SpawnEnvironment();
        switch (roomType) {
            case RoomType.NORMAL:
                SpawnBlockades();
                SpawnMonsters();
                break;
            case RoomType.SHOP:
                SpawnShop();
                break;
            case RoomType.BOSS:
                SpawnBoss();
                break;
            case RoomType.START:
                break;
        }
    }

    /// <summary>
    /// Spawns the shop, the items, and orientates them properly
    /// </summary>
    void SpawnShop() {
        GameObject shopKeep = Instantiate(shopKeeper);
        shopKeep.transform.position = floorBase.transform.position;
        shopKeep.transform.localScale = new Vector3(3f, 3f, 3f);
        int pos = GetDirectionofEntrance(neighbors[0]); //TODO: FIX INDEX WAS OUTSIDE THE BOUNDS OF THE ARRAY

        GameObject item1 = Instantiate(itemPlace);
        GameObject item2 = Instantiate(itemPlace);
        GameObject item3 = Instantiate(itemPlace);

        //Orientation
        switch (pos) {
            case 0:
                shopKeep.transform.position += new Vector3(0f, 1f, floorBase.transform.localScale.z / 4);
                item1.transform.Rotate(new Vector3(0, 90f, 0));
                item1.gameObject.name = "90";
                item2.transform.Rotate(new Vector3(0, 90f, 0));
                item3.transform.Rotate(new Vector3(0, 90f, 0));

                item2.transform.position = floorBase.transform.position + new Vector3(floorBase.transform.localScale.x / 6, 1.5f, 0f);
                item3.transform.position = floorBase.transform.position + new Vector3(-floorBase.transform.localScale.x / 6, 1.5f, 0f);
                break;
            case 1:
                shopKeep.transform.position += new Vector3(floorBase.transform.localScale.x / 4, 1f, 0f);
                item1.transform.Rotate(new Vector3(0, 180f, 0));
                item1.gameObject.name = "180";
                item2.transform.Rotate(new Vector3(0, 180f, 0));
                item3.transform.Rotate(new Vector3(0, 180f, 0));

                item2.transform.position = floorBase.transform.position + new Vector3(0f, 1.5f, floorBase.transform.localScale.z / 6);
                item3.transform.position = floorBase.transform.position + new Vector3(0f, 1.5f, -floorBase.transform.localScale.z / 6);
                break;
            case 2:
                shopKeep.transform.position += new Vector3(0f, 1f, -floorBase.transform.localScale.z / 4);
                item1.transform.Rotate(new Vector3(0, -90f, 0));
                item1.gameObject.name = "-90";
                item2.transform.Rotate(new Vector3(0, -90f, 0));
                item3.transform.Rotate(new Vector3(0, -90f, 0));

                item2.transform.position = floorBase.transform.position + new Vector3(-floorBase.transform.localScale.x / 6, 1.5f, 0f);
                item3.transform.position = floorBase.transform.position + new Vector3(floorBase.transform.localScale.x / 6, 1.5f, 0f);
                break;
            case 3:
                shopKeep.transform.position += new Vector3(-floorBase.transform.localScale.x / 4, 1f, 0f);
                item1.transform.Rotate(new Vector3(0, 0f, 0));
                item1.gameObject.name = "0";
                item2.transform.Rotate(new Vector3(0, 0f, 0));
                item3.transform.Rotate(new Vector3(0, 0f, 0));

                item2.transform.position = floorBase.transform.position + new Vector3(0f, 1.5f, -floorBase.transform.localScale.z / 6);
                item3.transform.position = floorBase.transform.position + new Vector3(0f, 1.5f, floorBase.transform.localScale.z / 6);
                break;
        }

        item1.transform.position = floorBase.transform.position + (Vector3.up * 1.5f);

        float baseCost = 30f;
        SetItemInShop(item1, baseCost, shopKeep);
        SetItemInShop(item2, baseCost, shopKeep);
        SetItemInShop(item3, baseCost, shopKeep);

        shopKeep.transform.parent = this.gameObject.transform;
    }

    /// <summary>
    /// Generates an item for the shop stand
    /// </summary>
    /// <param name="stand">Item stand</param>
    /// <param name="baseCost">Base cost of item</param>
    /// <param name="shopKeep">Parent shopkeep</param>
    void SetItemInShop(GameObject stand, float baseCost, GameObject shopKeep) {
        GameObject i = Instantiate(ItemManager.im.GetRandomShopItemWeighted());
        i.name = i.name.Replace("(Clone)", "").Trim();
        stand.GetComponentInChildren<ShopStand>().SetShop(i, baseCost);
        stand.transform.parent = shopKeep.transform;
    }

    /// <summary>
    /// Get the direction of the entrance of the room (for special rooms)
    /// </summary>
    /// <param name="entrance"></param>
    /// <returns></returns>
    int GetDirectionofEntrance(GameObject entrance) {
        float difX = entrance.transform.position.x - this.gameObject.transform.position.x;
        float difZ = entrance.transform.position.z - this.gameObject.transform.position.z;
        if(difX > 0) {
            //EAST
            return 3;
        }else if(difX < 0) {
            //WEST
            return 1;
        }else if(difZ > 0) {
            //NORTH
            return 2;
        }else if(difZ < 0) {
            //SOUTH
            return 0;
        } else {
            print("Warning! Coordinate math error!");
            return 0;
        }

    }

    /// <summary>
    /// Spawns ambience and environment (like grass)
    /// </summary>
    void SpawnEnvironment() {
        int timeout = 0;
        int envToSpawn = Random.Range(minEnvironment, maxEnvironment);
        HashSet<Vector2> hs = new HashSet<Vector2>();
        while(environments.Count < envToSpawn && timeout < ((xLength - 1) * (yLength - 1))) {
            int x = Random.Range(1, xLength - 1);
            int y = Random.Range(1, yLength - 1);
            if (hs.Contains(new Vector2(x, y))) {
                timeout++;
                continue;
            }
            hs.Add(new Vector2(x, y));
            if (GameManager.gm.seed.ShouldSpawn(x, y, xLength, yLength, envPerlinThreshold)) {
                GameObject grass = new GameObject();
                grass.transform.parent = this.gameObject.transform;

                int r = Random.Range(minPatch, maxPatch);
                for (int i = 0; i < r; i++) {
                    Vector3 spawnPoint = new Vector3(roomArray[x, y].position.x + Random.Range(-(nodeLength / 2), (nodeLength / 2)), 0.5f, roomArray[x, y].position.z + Random.Range(-(nodeLength / 2), (nodeLength / 2)));
                    GameObject env = Instantiate(envSpawn);
                    env.transform.parent = grass.transform;
                    env.transform.position = spawnPoint;
                    environments.Add(env);
                }

            }



        }
    }

    /// <summary>
    /// Spawns walls around the room
    /// </summary>
    void SpawnBlockades() {
        int timeout = 0;
        int blockToSpawn = Random.Range(minBlockade, maxBlockade);
        HashSet<Vector2> hs = new HashSet<Vector2>();
        while (blockades.Count < blockToSpawn && timeout < ((xLength - 1) * (yLength - 1))) {
            int x = Random.Range(1, xLength - 1);
            int y = Random.Range(1, yLength - 1);
            if (hs.Contains(new Vector2(x, y))) {
                timeout++;
                continue;
            }
            hs.Add(new Vector2(x, y));
            if (GameManager.gm.seed.ShouldSpawn(x, y, xLength, yLength, blockPerlinThreshold)) {
                int r = Random.Range(minLength, maxLength);
                int xCoord = x;
                int yCoord = y;
                GameObject wallParent = new GameObject();
                wallParent.transform.parent = this.gameObject.transform;
                wallParent.transform.position = new Vector3(0, 4f, 0f);
                List<Vector2> validPath = new List<Vector2>();
                validPath.Add(new Vector2(x, y));
                for(int i = 0; i < r; i++) {
                    List<Vector2> potPath = new List<Vector2>(GetNeighborNodesPositions(roomArray[xCoord, yCoord]));
                    for(int t = 0; t < 4; t++) {
                        int index = Random.Range(0, potPath.Count);
                        if(IsValidCoordinate((int)potPath[index].x, (int)potPath[index].y) && !hs.Contains(new Vector2((int)potPath[index].x, (int)potPath[index].y)) && (int)potPath[index].x < xLength - 1 && (int)potPath[index].y < yLength - 1 && (int)potPath[index].x >= 1 && (int)potPath[index].y >= 1) {
                            hs.Add(new Vector2((int)potPath[index].x, (int)potPath[index].y));
                            validPath.Add(new Vector2((int)potPath[index].x, (int)potPath[index].y));
                            xCoord = (int)potPath[index].x;
                            yCoord = (int)potPath[index].y;
                            break;
                        }
                    }
                }
                if(validPath.Count < 2) {
                    timeout += 10;
                    continue;
                }

                Vector2[] vPath = validPath.ToArray();
                for(int u = 0; u < vPath.Length; u++) {
                    if (u == 0) {
                        GameObject wall = Instantiate(blockadeSpawn);
                        wall.transform.parent = wallParent.transform;
                        wall.transform.position = wall.transform.parent.transform.position + roomArray[(int)vPath[u].x, (int)vPath[u].y].position;
                        blockades.Add(wall);
                        int direction = GetDirection((int)vPath[u].x, (int)vPath[u].y, (int)vPath[u + 1].x, (int)vPath[u + 1].y);
                        switch (direction) {
                            case 0:
                                wall.transform.Rotate(new Vector3(0f, 90f, 0f));
                                break;
                            case 2:
                                wall.transform.Rotate(new Vector3(0f, 90f, 0f));
                                break;
                            default:
                                break;
                        }
                    } else if (u == vPath.Length - 1) {
                        GameObject wall = Instantiate(blockadeSpawn);
                        wall.transform.parent = wallParent.transform;
                        wall.transform.position = wall.transform.parent.transform.position + roomArray[(int)vPath[u].x, (int)vPath[u].y].position;
                        blockades.Add(wall);
                        int direction = GetDirection((int)vPath[u].x, (int)vPath[u].y, (int)vPath[u - 1].x, (int)vPath[u - 1].y);
                        switch (direction) {
                            case 0:
                                wall.transform.Rotate(new Vector3(0f, 90f, 0f));
                                break;
                            case 2:
                                wall.transform.Rotate(new Vector3(0f, 90f, 0f));
                                break;
                            default:
                                break;
                        }
                    } else {
                        int directionPast = GetDirection((int)vPath[u].x, (int)vPath[u].y, (int)vPath[u - 1].x, (int)vPath[u - 1].y);
                        int directionFuture = GetDirection((int)vPath[u].x, (int)vPath[u].y, (int)vPath[u +1].x, (int)vPath[u+1].y);
                        if ((directionPast + directionFuture) % 2 == 0) {
                            //SameDirection
                            GameObject wall = Instantiate(blockadeSpawn);
                            wall.transform.parent = wallParent.transform;
                            wall.transform.position = wall.transform.parent.transform.position + roomArray[(int)vPath[u].x, (int)vPath[u].y].position;
                            blockades.Add(wall);

                            if (directionFuture == 0 || directionFuture == 2) {
                                wall.transform.Rotate(new Vector3(0f, 90f, 0f));
                            } else {
                                wall.transform.Rotate(new Vector3(0f, 0f, 0f));
                            }
                        } else {
                            //Different Direction
                            GameObject wall = Instantiate(blockadeCornerSpawn);
                            wall.transform.parent = wallParent.transform;
                            wall.transform.position = wall.transform.parent.transform.position + roomArray[(int)vPath[u].x, (int)vPath[u].y].position;
                            blockades.Add(wall);
                            wall.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                            wall.gameObject.name = "Corner";
                            if (directionFuture + directionPast == 1) {
                                //1
                                wall.transform.Rotate(new Vector3(0f, 270f, 0f));
                                wall.transform.position = wall.transform.position + new Vector3(2.4f, 0f, 1.8f);

                            } else if(directionPast + directionFuture == 3) {
                                //3
                                if(Mathf.Abs(directionFuture - directionPast) == 1) {
                                    //3-1
                                    //wall.transform.rotation = wall.transform.rotation * Quaternion.AngleAxis(270f, Vector3.up);
                                    wall.transform.Rotate(new Vector3(0f, 180f, 0f));
                                    wall.transform.position = wall.transform.position + new Vector3(-1.8f, 0f, 2.4f);
                                } else {
                                    //3-3
                                    wall.transform.Rotate(new Vector3(0f, 0f, 0f));
                                    wall.transform.position = wall.transform.position + new Vector3(1.8f, 0f, -2.4f);
                                }
                            } else {
                                //5
                                wall.transform.Rotate(new Vector3(0f, 90f, 0f));
                                wall.transform.position = wall.transform.position + new Vector3(-2.4f, 0f, -1.8f);
                            }

                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Returns a direction for given old and new coordinates where the direction is equal to the position of the new in relation to the old
    /// </summary>
    /// <param name="x">New x</param>
    /// <param name="y">New y</param>
    /// <param name="xx">Old x</param>
    /// <param name="yy">Old y</param>
    /// <returns></returns>
    int GetDirection(int x, int y, int xx, int yy) {
        int difX = xx - x;
        int difY = yy - y;
        //print("Before " + n.xPos.ToString() + ", " + n.yPos.ToString());
        //print("Pair " + difX.ToString() + ", " + difY.ToString());
        switch (difX) {

            case -1:
                return 3;
            case 0:
                switch (difY) {
                    case -1:
                        return 2;
                    case 1:
                        return 0;
                }
                break;
            case 1:
                return 1;
            default:
                return 0;
        }
        return 0;
    }

    bool IsValidCoordinate(int x, int y) {
        if ((x < 0 || x >= xLength) || (y < 0 || y >= yLength)) {
            return false;
        }
        return true;
    }

    Vector2[] GetNeighborNodesPositions(Node n) {
        Vector2[] toCheck = new Vector2[4];
        toCheck[0] = new Vector2(n.xPos, n.yPos + 1);
        //toCheck[1] = new Vector2(n.xPos + 1, n.yPos + 1);
        toCheck[1] = new Vector2(n.xPos + 1, n.yPos);
        //toCheck[3] = new Vector2(n.xPos + 1, n.yPos - 1);
        toCheck[2] = new Vector2(n.xPos, n.yPos - 1);
        //toCheck[5] = new Vector2(n.xPos - 1, n.yPos - 1);
        toCheck[3] = new Vector2(n.xPos - 1, n.yPos);
        //toCheck[7] = new Vector2(n.xPos - 1, n.yPos + 1);
        return toCheck;
    }

    /// <summary>
    /// Spawns monsters within the room at random positions
    /// </summary>
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
            Enemy e = potentialMonsterSpawns[m].GetComponent<Enemy>();
            if(e.maxSpawn > 0) {
                int count = 0;
                foreach(GameObject mon in monsters) {
                    if(mon.GetComponent<Enemy>().enemyID == e.enemyID) {
                        count++;
                    }
                }
                if(count >= e.maxSpawn) {
                    potentialMonsterSpawns.RemoveAt(m);
                    m = Random.Range(0, potentialMonsterSpawns.Count);
                }
            }
            GameObject monster = Instantiate(potentialMonsterSpawns[m]);
            e = monster.GetComponent<Enemy>();
            Vector3 spawnPoint = Vector3.zero;
            if(e.et == Enemy.EnemyType.NORMAL) {
                spawnPoint = new Vector3(roomArray[x, y].position.x + Random.Range(-(nodeLength / 2), (nodeLength / 2)), roomArray[x, y].position.y, roomArray[x, y].position.z + Random.Range(-(nodeLength / 2), (nodeLength / 2)));
            }
            monster.transform.parent = this.gameObject.transform;
            monster.transform.position = spawnPoint;
            monsters.Add(monster);
        }

        if (RoomManager.rm.currentRoom != this) {
            DeactivateMonsters();
        }
    }

    public Vector3 GetRandomNodePosition() {
        int x = Random.Range(0, xLength - 1);
        int y = Random.Range(0, yLength - 1);
        return roomArray[x, y].position;
    }

   public Vector3[] GetTwoRandomNodePosition() {
        Vector3 pos1 = GetRandomNodePosition();
        Vector3 pos2 = GetRandomNodePosition();
        while (true) {
            if (pos1 != pos2) {
                break;
            }
            pos2 = GetRandomNodePosition();
        }
        return new Vector3[] { pos1, pos2 };
    }

    /// <summary>
    /// Spawns a boss
    /// </summary>
    void SpawnBoss() {
        GameObject boss = Instantiate(bosses[0]);
        boss.transform.parent = this.gameObject.transform;
        boss.transform.position = roomArray[xLength / 2, yLength / 2].position;
        monsters.Add(boss);
        DeactivateMonsters();
    }

    public bool DefeatMonster(GameObject monster) {
        monsters.Remove(monster);
        Destroy(monster);
        if(monsters.Count == 0) {
            ClearRoom();
            return true;
        }
        return false;
    }

    public bool DefeatMonster(GameObject monster, float d) {
        monsters.Remove(monster);
        Destroy(monster, d);
        if (monsters.Count == 0) {
            ClearRoom();
            return true;
        }
        return false;
    }

    public void ClearRoom() {
        if (roomType == RoomType.BOSS) {
            ClearFloor();
        }
        if (roomType != RoomType.SHOP) {
            SetMinimapExplored();
        }
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
        f.gameObject.transform.position = GetPositionAcross(GameManager.gm.player.transform.position) + (Vector3.up * 3f);

        GameObject reward = Instantiate(ItemManager.im.GetRandomAnyItem());
        reward.AddComponent<PickUpItem>();
        reward.transform.position = Vector3.Lerp(f.gameObject.transform.position, GameManager.gm.player.transform.position, 0.45f);
        reward.transform.position = new Vector3(reward.transform.position.x, 3f, reward.transform.position.z);
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

    public Vector3 GetPositionAcross(Vector3 pos) {
        Vector3[] corners = new Vector3[] {
            roomArray[0, 0].position,
            roomArray[xLength - 1, 0].position,
            roomArray[0, yLength - 1].position,
            roomArray[xLength - 1, yLength - 1].position
        };
        float d = Vector3.Distance(pos, corners[0]);
        int t = 0;
        for(int i = 1; i < corners.Length; i++) {
            float e = Vector3.Distance(pos, corners[i]);
            if(e < d) {
                d = e;
                t = i;
            }
        }
        switch (t) {
            case 0:
                return roomArray[xLength - 1, yLength - 1].position;
            case 1:
                return roomArray[0, yLength - 1].position;
            case 2:
                return roomArray[xLength - 1, 0].position;
            case 3:
                return roomArray[0, 0].position;
            default:
                print("Warning! Corner math error!");
                return floorBase.transform.position + Vector3.up;
        }
 
    }
}
