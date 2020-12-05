using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour {

    public static RoomManager rm;
    public CullingRooms cr;
    public Node[,] roomArray;

    [Header("Floor Settings")]
    [Tooltip("Size of each room plus the connectors")]
    public float nodeLength = 120f;
    [Tooltip("Numbers used to generate the size of the floor in X and Z space")]
    public int min, max;
    [Tooltip("Numbers used for min and max generation of rooms")]
    public int minMin, maxMin;
    [Space(10)]
    [HideInInspector]
    public int xLength, yLength;

    [Header("Room Settings")]
    [Tooltip("Rooms that can be used during generation in place of a node")]
    public List<GameObject> potentialRoomSpawns;
    [Tooltip("Boss rooms that can be used during generation in place of a node")]
    public List<GameObject> potentialBossRoomSpawns;
    [Tooltip("Shop rooms that can be used during generation in place of a node")]
    public List<GameObject> potentialShopRoomSpawns;
    [Tooltip("Rooms that have already been generated")]
    public List<Node> createdRooms;
    [Tooltip("Debug spawn object")]
    public GameObject testObject;

    [Header("Current Room")]
    [Tooltip("The current room the player is in")]
    public Room currentRoom;
    public GameObject nextFloor;

    private List<Node> endRoomNodes;
    private bool debugMode = false;


    [System.Serializable]
    public class Node {
        public int xPos;
        public int yPos;
        public bool isTaken = false;
        public bool isValid = true;
        public GameObject room;
    }

    private void Awake() {
        //Singleton behavior
        if (rm == null) {
            rm = this;

            //The gamemanager should exist (UNLESS SOMETHING TERRIBLE WENT WRONG)
            //If the gamemanager exists, like it should, rename the gameobject to the floor numbers, else debug mode
            if (GameManager.gm != null) {
                rm.gameObject.name = "RoomManager-" + GameManager.gm.currentFloor.ToString();
            } else {
                rm.gameObject.name = "Debug Mode";
                debugMode = true;
            }
        } else if (rm != this) {
            Destroy(this);
        }
    }

    void Start() {
        if (debugMode) {
            CreateFloorLayout();
            DebugValidNodes();
        }
    }


    void DebugValidNodes() {
        //generate objects on nodes that are invalid
        for (int u = 0; u < xLength; u++) {
            for (int y = 0; y < yLength; y++) {
                if(roomArray[u, y].isValid) {
                    continue;
                }
                GameObject g = Instantiate(testObject);
                g.transform.position = new Vector3(u * nodeLength, 0, y * nodeLength);
            }
        }
    }

    /// <summary>
    /// Gets the average positions of the rooms to place the minimap camera in the center
    /// </summary>
    /// <returns>Center of the floor</returns>
    Vector3 GetAveragePosition() {
        Vector3 v = new Vector3();
        foreach (Node n in createdRooms) {
            v += n.room.transform.position;
        }
        v /= createdRooms.Count;
        return v;
    }

    /// <summary>
    /// Changes the current room to the one just entered
    /// </summary>
    /// <param name="r">Entered room</param>
    public void ChangeRoom(Room r) {
        if(r.roomType == Room.RoomType.BOSS) {
            MusicManager.mm.am.SetTrigger("FadeOut");
        }
        currentRoom = r;
        currentRoom.AddGates();
        currentRoom.ActivateMonsters();
        SetEnemiesRemainingText();
        GameManager.gm.SetLastTimeDamaged();
    }

    /// <summary>
    /// Sets the text for how many enemies are remaining
    /// </summary>
    public void SetEnemiesRemainingText() {
        if (currentRoom.roomType == Room.RoomType.NORMAL) {
            UIManager.ui.enemyCount.gameObject.SetActive(true);
            UIManager.ui.SetEnemiesRemaining(currentRoom.monsters.Count);
        }
    }

    /// <summary>
    /// Removes the monster and determines if the room is cleared
    /// </summary>
    /// <param name="monster"></param>
    public void DefeatMonster(GameObject monster) {
        bool a = currentRoom.DefeatMonster(monster);
        if (!a) {
            SetEnemiesRemainingText();
        } else {
            UIManager.ui.enemyCount.gameObject.SetActive(false);
        }

    }

    /// <summary>
    /// Removes the monster (after a set amount of time), but runs through the checks to determine if the room is cleared
    /// </summary>
    /// <param name="monster"></param>
    /// <param name="d">Time until deleted</param>
    public void DefeatMonster(GameObject monster, float d) {
        bool a = currentRoom.DefeatMonster(monster, d);
        if (!a) {
            SetEnemiesRemainingText();
        } else {
            UIManager.ui.enemyCount.gameObject.SetActive(false);
        }
    }


    /// <summary>
    /// Generates the floor and all the rooms inside
    /// </summary>
    public void CreateFloorLayout() {

        //generate size
        xLength = Random.Range(min, max);
        yLength = Random.Range(min, max);
        roomArray = new Node[xLength, yLength];
        createdRooms = new List<Node>();

        //generate initial room / min
        int minNumberOfRooms = Random.Range(minMin, maxMin);
        //print(minNumberOfRooms + " have been chosen to be created");
        int initialX = Random.Range(0, xLength - 1);
        int initialY = Random.Range(0, yLength - 1);


        //generate nodes
        for (int u = 0; u < xLength; u++) {
            for (int y = 0; y < yLength; y++) {
                Node temp = new Node {
                    xPos = u,
                    yPos = y
                };
                roomArray[u, y] = temp;
            }
        }

        //Initial
        GenerateSpecialRoom(initialX, initialY, Room.RoomType.START);

        //generate rooms
        Node currentNode = roomArray[initialX, initialY];
        currentNode.isTaken = true;

        createdRooms.Add(currentNode);
        endRoomNodes = new List<Node>();
        for (int i = 1; i < minNumberOfRooms;) {
            i += GenerateDoors(currentNode);
            currentNode = GetRandomEndNode();
            if (currentNode == null) {
                break;
            }
        }

        //Validate starting room
        currentRoom.DeactivateMonsters();
        currentRoom.ClearRoom();
        currentRoom.roomType = Room.RoomType.START;


        //generate boss
        //GenerateSpecialRoom(initialX, initialY, Room.RoomType.BOSS);

        //generate other special rooms
        //GenerateSpecialRoom(initialX, initialY, Room.RoomType.SHOP);


        //Camera
        GameManager.gm.mm.c.gameObject.transform.position = GetAveragePosition() + new Vector3(0, 50f, 0);
        GameManager.gm.mm.c.orthographicSize = 100 * (createdRooms.Count / 2);

        //Validation
        ValidateNodeNeighbors();
        cr.ValidateStartRoom();
        print(createdRooms.Count + " rooms have been created!");

    }

    void ValidateNodeNeighbors() {
        foreach (Node n in createdRooms) {
            SetAllNeighborsClosed(n);
        }
    }


    /// <summary>
    /// Will generate special rooms, such as the boss room, and handle the spawning behavior
    /// </summary>
    /// <param name="intX">Initial X of floor</param>
    /// <param name="intY">Initial Y of floor</param>
    /// <param name="rt">Room Type to spawn</param>
    void GenerateSpecialRoom(int intX, int intY, Room.RoomType rt) {
        GameObject room;
        if(rt == Room.RoomType.SHOP || rt == Room.RoomType.BOSS) {
            List<Vector2> potentialLoc= GetPotentialEndRoom(intX, intY);
            int pickedSpawn = Random.Range(0, potentialLoc.Count);

            if (rt == Room.RoomType.SHOP) {
                room = Instantiate(potentialShopRoomSpawns[0]);
                room.gameObject.name = "SHOP";
                room.GetComponent<Room>().ClearRoom();
            } else {
                room = Instantiate(potentialBossRoomSpawns[0]);
                room.gameObject.name = "BOSS";
            }

            room.transform.position = new Vector3(potentialLoc[pickedSpawn].x * nodeLength, 0, potentialLoc[pickedSpawn].y * nodeLength);
            roomArray[(int)potentialLoc[pickedSpawn].x, (int)potentialLoc[pickedSpawn].y].isTaken = true;
            createdRooms.Add(roomArray[(int)potentialLoc[pickedSpawn].x, (int)potentialLoc[pickedSpawn].y]);
            roomArray[(int)potentialLoc[pickedSpawn].x, (int)potentialLoc[pickedSpawn].y].room = room;
            room.GetComponent<Room>().roomType = rt;
            room.gameObject.SetActive(false);
        } else if(rt == Room.RoomType.START) {
            room = Instantiate(potentialRoomSpawns[0]);
            room.transform.position = new Vector3(intX * nodeLength, 0, intY * nodeLength);
            GameManager.gm.playerMovement.Warp(new Vector3(room.transform.position.x, 2, room.transform.position.z));
            currentRoom = room.GetComponent<Room>();
            createdRooms.Add(roomArray[intX, intY]);
            roomArray[intX, intY].room = room;

            room.GetComponent<Room>().roomType = rt;
        }

    }


    /// <summary>
    /// Navigates through the floor to find coordinates that have open neighbors that dont connect to more than 1 room. Returns the neighbors
    /// </summary>
    /// <param name="x">Initial X of start</param>
    /// <param name="y">Initial Y of start</param>
    /// <returns></returns>
    List<Vector2> GetPotentialEndRoom(int x, int y) {
        List<Vector2> toReturn = new List<Vector2>();
        foreach (Node n in createdRooms) {
            List<Vector2> temp = EndRoomsFromNode(n);
            foreach(Vector2 v in temp) {
                toReturn.Add(v);
            }
        }
        return toReturn;
    }

    List<Vector2> EndRoomsFromNode(Node n) {
        Room nRoom = n.room.GetComponent<Room>();
        List<Vector2> toReturn = new List<Vector2>();
        if (nRoom.roomType != Room.RoomType.NORMAL || nRoom.neighbors.Length >= nRoom.maxNeighbors) {
            return toReturn;
        }
        Vector2[] neighbors = GetNeighborNodesPositions(n);
        for (int i = 0; i < neighbors.Length; i++) {
            if (!IsValidCoordinate((int)neighbors[i].x, (int)neighbors[i].y)) {
                continue;
            }
            if (roomArray[(int)neighbors[i].x, (int)neighbors[i].y].isTaken) {
                continue;
            }
            int nCount = 0;
            Vector2[] nextNeighbors = GetNeighborNodesPositions(roomArray[(int)neighbors[i].x, (int)neighbors[i].y]);
            for (int t = 0; t < nextNeighbors.Length; t++) {
                if (IsValidCoordinate((int)nextNeighbors[t].x, (int)nextNeighbors[t].y)) {
                    if (roomArray[(int)nextNeighbors[t].x, (int)nextNeighbors[t].y].isTaken) {
                        Room r = roomArray[(int)nextNeighbors[t].x, (int)nextNeighbors[t].y].room.GetComponent<Room>();
                        if (r.roomType != Room.RoomType.NORMAL) {
                            nCount += 4;
                            break;
                        }
                        nCount++;
                    }
                }
            }
            if (nCount == 1) {
                toReturn.Add(new Vector2(neighbors[i].x, neighbors[i].y));
            }
        }
        return toReturn;
    }



    /// <summary>
    /// Sets neighbors closed where paths do not lead anywhere, blocking them
    /// </summary>
    /// <param name="n">Room node</param>
    void SetAllNeighborsClosed(Node n) {
        Vector2[] toCheck = GetNeighborNodesPositions(n);
        Room room = n.room.GetComponent<Room>();
        for (int i = 0; i < 4; i++) {
            if (!IsValidCoordinate((int)toCheck[i].x, (int)toCheck[i].y)) {
                SetClosedNeighbor(n, room, (int)toCheck[i].x, (int)toCheck[i].y, true);
            }else if (roomArray[(int)toCheck[i].x, (int)toCheck[i].y].isTaken) {
                List<GameObject> newNeighbors = new List<GameObject>(room.neighbors);
                if (!newNeighbors.Contains(roomArray[(int)toCheck[i].x, (int)toCheck[i].y].room)) {
                    newNeighbors.Add(roomArray[(int)toCheck[i].x, (int)toCheck[i].y].room);
                    room.neighbors = newNeighbors.ToArray();
                }
                SetClosedNeighbor(n, room, (int)toCheck[i].x, (int)toCheck[i].y, false);
            }else {
                SetClosedNeighbor(n, room, (int)toCheck[i].x, (int)toCheck[i].y, true);
            }
        }
    }

    /// <summary>
    /// Expands the floor from the given node
    /// </summary>
    /// <param name="n">Room node to expand from</param>
    /// <returns></returns>
    int GenerateDoors(Node n) {
        Vector2[] toCheck = GetNeighborNodesPositions(n);
        List<Vector2> validRoomPositions = new List<Vector2>();
        List<Vector2> invalidRoomPositions = new List<Vector2>();

        Room room = n.room.GetComponent<Room>();


        //create random number of doors and their rooms
        int doors = Mathf.FloorToInt((Random.Range(2, room.maxNeighbors + 1) + Random.Range(2, room.maxNeighbors + 1) + Random.Range(2, room.maxNeighbors + 1)) / 3);
        List<GameObject> newRoomList = new List<GameObject>();
        //print("Goal neighbors: " + doors);
        //Split valid and invalid rooms and count existing neighbors
        for (int i = 0; i < 4; i++) {
            if(!IsValidCoordinate((int)toCheck[i].x, (int)toCheck[i].y)) {
                SetClosedNeighbor(n, room, (int)toCheck[i].x, (int)toCheck[i].y, true);
                continue;
            }
            if(roomArray[(int)toCheck[i].x, (int)toCheck[i].y].isValid) {
                if(!roomArray[(int)toCheck[i].x, (int)toCheck[i].y].isTaken) {
                    if (!CheckIfCreationWouldCauseInvalid(roomArray[(int)toCheck[i].x, (int)toCheck[i].y])) {
                        validRoomPositions.Add(toCheck[i]);
                    } else {
                        doors--;
                        newRoomList.Add(roomArray[(int)toCheck[i].x, (int)toCheck[i].y].room);
                        invalidRoomPositions.Add(toCheck[i]);
                    }
                } else {
                    doors--;
                    newRoomList.Add(roomArray[(int)toCheck[i].x, (int)toCheck[i].y].room);
                    invalidRoomPositions.Add(toCheck[i]);
                }
            } else {
                invalidRoomPositions.Add(toCheck[i]);
            }
        }

        int newRoomsCreatedCount = 0;

        //print(n.xPos + ", " + n.yPos + " has " + doors + " doors before generation with " + validRoomPositions.Count + " possible chocies, plus " + invalidRoomPositions.Count + " invalids");
        while (validRoomPositions.Count > 0 && doors > 0) {
            newRoomsCreatedCount++;
            int randomRoomInt = Random.Range(0, validRoomPositions.Count);
            GameObject newRoom = GenerateRoom((int)validRoomPositions[randomRoomInt].x, (int)validRoomPositions[randomRoomInt].y);
            newRoomList.Add(newRoom);
            createdRooms.Add(roomArray[(int)validRoomPositions[randomRoomInt].x, (int)validRoomPositions[randomRoomInt].y]);
            roomArray[(int)validRoomPositions[randomRoomInt].x, (int)validRoomPositions[randomRoomInt].y].isTaken = true;
            roomArray[(int)validRoomPositions[randomRoomInt].x, (int)validRoomPositions[randomRoomInt].y].room = newRoom;
            CheckEndNode(roomArray[(int)validRoomPositions[randomRoomInt].x, (int)validRoomPositions[randomRoomInt].y]);
            validRoomPositions.RemoveAt(randomRoomInt);

            doors--;
        }


        //add rooms to node neighbors
        room.neighbors = newRoomList.ToArray();
        //print(n.xPos + ", " + n.yPos + " has " + room.neighbors.Length + " neighbors after generation!");

        //add remaining potential spots to invalids
        foreach(Vector2 v in validRoomPositions) {
            roomArray[(int)v.x, (int)v.y].isValid = false;
        }

        /*
        //set node to not have doors to invalids
        //set node to not have doors to invalids
        foreach(Vector2 v in invalidRoomPositions) {
            SetClosedNeighbor(n, room, (int)v.x, (int)v.y, true);
            roomArray[(int)v.x, (int)v.y].isValid = false;
        }*/

        //Disable rooms for culling purposes
        cr.CullOutRoom(room);
        //print("End nodes before re-checking: " + endRoomNodes.Count);
        RecheckAllEndNodes();
        //print("End nodes after re-checking: " + endRoomNodes.Count);

        return newRoomsCreatedCount;
    }

    /// <summary>
    /// Invalidates rooms diagonally from the nodes (creates more "spacing" in room and feels less cramped)
    /// </summary>
    /// <param name="n">Room node</param>
    void InvalidateCorners(Node n) {
        Vector2[] corners = GetCornerNodesPositions(n);
        for(int i = 0; i < corners.Length; i++) {
            if (IsValidCoordinate((int)corners[i].x, (int)corners[i].y)) {
                roomArray[(int)corners[i].x, (int)corners[i].y].isValid = false;
            }
        }
    }

    /// <summary>
    /// Checks if the coordinate given exists within the coordinate space
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    bool IsValidCoordinate(int x, int y) {
        if ((x < 0 || x >= xLength) || (y < 0 || y >= yLength)) {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Verifies the nodes in the end node list are, in fact, end nodes
    /// </summary>
    void RecheckAllEndNodes() {
        foreach (Node n in new List<Node>(endRoomNodes)) {
            CheckEndNode(n);
        }
    }

    /// <summary>
    /// Get a random end node from the list
    /// </summary>
    /// <returns>End Room node</returns>
    Node GetRandomEndNode() {
        if (endRoomNodes.Count == 0) {
            return null;
        }
        return endRoomNodes[Random.Range(0, endRoomNodes.Count - 1)];
    }
    /// <summary>
    /// Determines if a node has room to expand with 1 empty spot
    /// </summary>
    /// <param name="n">Empty node</param>
    /// <returns></returns>
    bool CheckEndNode(Node n) {
        List<Vector2> temp = EndRoomsFromNode(n);
        //print(temp.Count + " end rooms found from the node of: " + n.xPos + ", " + n.yPos);
        if (temp.Count > 0) {
            AddNodeEnd(n);
            return true;
            }
        RemoveNodeEnd(n);
        return false;
    }

    /// <summary>
    /// Adds node to the list of end nodes
    /// </summary>
    /// <param name="n">Room node</param>
    void AddNodeEnd(Node n) {
        if (!endRoomNodes.Contains(n)) {
            endRoomNodes.Add(n);
        }
    }

    /// <summary>
    /// Removes node from the list of end nodes
    /// </summary>
    /// <param name="n">Room node</param>
    /// <returns></returns>
    bool RemoveNodeEnd(Node n) {
        if (endRoomNodes.Contains(n)) {
            endRoomNodes.Remove(n);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Changes the state of a neighbor (if they should be open or closed) and sets the connectors/blockers appropriately
    /// </summary>
    /// <param name="n">Room node</param>
    /// <param name="r">Room room</param>
    /// <param name="x">Coordinate X to set</param>
    /// <param name="y">Coordinate Y to set</param>
    /// <param name="closed">True == closed; False == open</param>
    void SetClosedNeighbor(Node n, Room r, int x, int y, bool closed) {
        int difX = x - n.xPos;
        int difY = y - n.yPos;
        //print("Before " + n.xPos.ToString() + ", " + n.yPos.ToString());
        //print("Pair " + difX.ToString() + ", " + difY.ToString());
        switch (difX) {

            case -1:
                r.connectors[3].SetActive(!closed);
                r.blockers[3].SetActive(closed);
                r.blockers[3].name = "West";
                break;
            case 0:
                switch (difY) {
                    case -1:
                        r.connectors[2].SetActive(!closed);
                        r.blockers[2].SetActive(closed);
                        r.blockers[2].name = "South";
                        break;
                    case 1:
                        r.connectors[0].SetActive(!closed);
                        r.blockers[0].SetActive(closed);
                        r.blockers[0].name = "North";
                        break;
                    default:
                        print("Help me! 2");
                        break;
                }
                break;
            case 1:
                r.connectors[1].SetActive(!closed);
                r.blockers[1].SetActive(closed);
                r.blockers[1].name = "East";
                break;
            default:
                print("Help me!");
                break;
        }
    }

    /// <summary>
    /// Creates a room at the given position
    /// </summary>
    /// <param name="x">Position to create X</param>
    /// <param name="y">Position to create Y</param>
    /// <returns>Gameobject for the Room</returns>
    GameObject GenerateRoom(int x, int y) {
        GameObject r = Instantiate(potentialRoomSpawns[Random.Range(0, potentialRoomSpawns.Count)]);
        r.transform.position = new Vector3(x * nodeLength, 0, y * nodeLength);
        r.gameObject.name = "X: " + (x).ToString() + ", Y: " + (y).ToString();
        return r;
    }

    bool CheckIfCreationWouldCauseInvalid(Node n) {
        Vector2[] v = GetNeighborNodesPositions(n);
        for (int i = 0; i < 4; i++) {
            if (!IsValidCoordinate((int)v[i].x, (int)v[i].y)) {
                continue;
            }
            if(roomArray[(int)v[i].x, (int)v[i].y].isTaken) {
                Room r = roomArray[(int)v[i].x, (int)v[i].y].room.GetComponent<Room>();
                if(r.neighbors.Length + 1 > r.maxNeighbors) {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Returns coordinates for all possible neighbors
    /// </summary>
    /// <param name="n">Room node</param>
    /// <returns>Neighbor coordinates</returns>
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
    /// Returns coordinates of corner neighbors (diagonally)
    /// </summary>
    /// <param name="n">Room node</param>
    /// <returns>Corner Neighbor coordinates</returns>
    Vector2[] GetCornerNodesPositions(Node n) {
        Vector2[] toCheck = new Vector2[4];
        toCheck[0] = new Vector2(n.xPos + 1, n.yPos + 1);
        toCheck[1] = new Vector2(n.xPos + 1, n.yPos - 1);
        toCheck[2] = new Vector2(n.xPos - 1, n.yPos - 1);
        toCheck[3] = new Vector2(n.xPos - 1, n.yPos + 1);
        return toCheck;
    }


}
