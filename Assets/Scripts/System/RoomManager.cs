using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour {

    public static RoomManager rm;
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
    [Tooltip("Rooms that have already been generated")]
    public List<Node> createdRooms;
    [Tooltip("Debug spawn object")]
    public GameObject testObject;

    [Header("Current Room")]
    [Tooltip("The current room the player is in")]
    public Room currentRoom;
    public GameObject nextFloor;
    [Tooltip("Text for number of enemies remaining")]
    public TMPro.TextMeshProUGUI text;
    public TMPro.TextMeshProUGUI slideText;

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
        if(rm == null) {
            rm = this;
            if (GameManager.gm != null) {
                rm.gameObject.name = "RoomManager-" + GameManager.gm.currentFloor.ToString();
            } else {
                rm.gameObject.name = "Debug Mode";
                debugMode = true;
            }
        }else if(rm != this) {
            Destroy(this);
        }
    }

    void Start() {
        if (debugMode) {
            CreateFloorLayout();
        }   
    }

    Vector3 GetAveragePosition() {
        Vector3 v = new Vector3();
        foreach(Node n in createdRooms) {
            v += n.room.transform.position;
        }
        v /= createdRooms.Count;
        return v;
    }

    public void ChangeRoom(Room r) {
        currentRoom = r;
        currentRoom.AddGates();
        currentRoom.ActivateMonsters();
        if (currentRoom.roomType == Room.RoomType.NORMAL) {
            SetEnemiesRemainingText();
        }
    }

    public void SetEnemiesRemainingText() {
        text.gameObject.SetActive(true);
        text.text = currentRoom.monsters.Count.ToString() + " Enemies Remaining";
    }

    public void DefeatMonster(GameObject monster) {
        bool a = currentRoom.DefeatMonster(monster);
        if (!a) {
            SetEnemiesRemainingText();
        } else {
            text.gameObject.SetActive(false);
        }

    }

    public void DefeatMonster(GameObject monster, float d) {
        bool a = currentRoom.DefeatMonster(monster, d);
        if (!a) {
            SetEnemiesRemainingText();
        } else {
            text.gameObject.SetActive(false);
        }
    }

    public void CreateFloorLayout() {

        //generate size
        xLength = Random.Range(min, max);
        yLength = Random.Range(min, max);
        roomArray = new Node[xLength, yLength];
        createdRooms = new List<Node>();

        //generate initial room / min
        int minNumberOfRooms = Random.Range(minMin, maxMin);
        int initialX = Random.Range(0, xLength - 1);
        int initialY = Random.Range(0, yLength - 1);


        //generate nodes
        for(int u = 0; u < xLength; u++) {
            for(int y = 0; y < yLength; y++) {
                Node temp = new Node {
                    xPos = u,
                    yPos = y
                };
                roomArray[u, y] = temp;
            }
        }

        //initial room
        GameObject r = Instantiate(potentialRoomSpawns[0]);
        r.transform.position = new Vector3(initialX * nodeLength, 0, initialY * nodeLength);
        GameManager.gm.playerMovement.Warp(new Vector3(r.transform.position.x, 2, r.transform.position.z));
        currentRoom = r.GetComponent<Room>();
        createdRooms.Add(roomArray[initialX, initialY]);
        roomArray[initialX, initialY].room = r;

        //generate rooms
        Node currentNode = roomArray[initialX, initialY];
        currentNode.isTaken = true;
        for (int i = 1; i < minNumberOfRooms;) {
            i += GenerateDoors(currentNode);
            currentNode = GetRandomEndNode();
            if(currentNode == null) {
                break;
            }
        }

        //generate special rooms

        //generate boss
        GetBossEndRoom(initialX, initialY);

        //Camera
        GameManager.gm.mm.c.gameObject.transform.position = GetAveragePosition() + new Vector3(0, 50f, 0);
        GameManager.gm.mm.c.orthographicSize = 100 * (createdRooms.Count / 2);

        //Validation
        ValidateNodeNeighbors();
        currentRoom.DeactivateMonsters();
        currentRoom.ClearRoom();
        currentRoom.roomType = Room.RoomType.START;
    }

    void ValidateNodeNeighbors() {
        foreach (Node n in createdRooms) {
            SetAllNeighborsClosed(n);
        }
    }


    void GetBossEndRoom(int initialX, int initialY) {
        List<Vector2> potentialBossLoc = new List<Vector2>();
        foreach (Node n in createdRooms) {
            if (roomArray[initialX, initialY] == n) {
                continue;
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
                            nCount++;
                        }
                    }
                }
                if (nCount == 1) {
                    potentialBossLoc.Add(new Vector2(neighbors[i].x, neighbors[i].y));
                }
            }
        }
        int bossSpawn = Random.Range(0, potentialBossLoc.Count - 1);
        GameObject boss = Instantiate(potentialBossRoomSpawns[0]);
        boss.gameObject.name = "BOSS";
        boss.transform.position = new Vector3(potentialBossLoc[bossSpawn].x * nodeLength, 0, potentialBossLoc[bossSpawn].y * nodeLength);
        roomArray[(int)potentialBossLoc[bossSpawn].x, (int)potentialBossLoc[bossSpawn].y].isTaken = true;
        boss.GetComponent<Room>().roomType = Room.RoomType.BOSS;
        createdRooms.Add(roomArray[(int)potentialBossLoc[bossSpawn].x, (int)potentialBossLoc[bossSpawn].y]);
        roomArray[(int)potentialBossLoc[bossSpawn].x, (int)potentialBossLoc[bossSpawn].y].room = boss;
    }

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


    int GenerateDoors(Node n) {
        Vector2[] toCheck = GetNeighborNodesPositions(n);
        List<Vector2> validRoomPositions = new List<Vector2>();
        List<Vector2> invalidRoomPositions = new List<Vector2>();
        endRoomNodes = new List<Node>();
        Room room = n.room.GetComponent<Room>();

        //Split valid and invalid rooms
        for (int i = 0; i < 4; i++) {
            if(!IsValidCoordinate((int)toCheck[i].x, (int)toCheck[i].y)) {
                SetClosedNeighbor(n, room, (int)toCheck[i].x, (int)toCheck[i].y, true);
                continue;
            }
            if(roomArray[(int)toCheck[i].x, (int)toCheck[i].y].isValid && !roomArray[(int)toCheck[i].x, (int)toCheck[i].y].isTaken) {
                validRoomPositions.Add(toCheck[i]);
                
            } else {
                invalidRoomPositions.Add(toCheck[i]);
            }
        }

        //create random number of doors and their rooms
        int doors = Mathf.FloorToInt((Random.Range(1, room.maxNeighbors) + Random.Range(1, room.maxNeighbors) + Random.Range(1, room.maxNeighbors)) / 3);
        //int doors = 1;
        List<GameObject> newRoomList = new List<GameObject>();
        while(validRoomPositions.Count != 0) {
            int randomRoomInt = Random.Range(0, validRoomPositions.Count - 1);
            GameObject newRoom = GenerateRoom((int)validRoomPositions[randomRoomInt].x, (int)validRoomPositions[randomRoomInt].y);
            newRoomList.Add(newRoom);
            createdRooms.Add(roomArray[(int)validRoomPositions[randomRoomInt].x, (int)validRoomPositions[randomRoomInt].y]);
            roomArray[(int)validRoomPositions[randomRoomInt].x, (int)validRoomPositions[randomRoomInt].y].isTaken = true;
            roomArray[(int)validRoomPositions[randomRoomInt].x, (int)validRoomPositions[randomRoomInt].y].room = newRoom;
            CheckEndNode(roomArray[(int)validRoomPositions[randomRoomInt].x, (int)validRoomPositions[randomRoomInt].y]);
            validRoomPositions.RemoveAt(randomRoomInt);
            doors--;
            if(doors <= 0) {
                break;
            }
        }


        //add rooms to node neighbors
        room.neighbors = newRoomList.ToArray();

        //add remaining potential spots to invalids
        foreach(Vector2 v in validRoomPositions) {
            invalidRoomPositions.Add(v);
            roomArray[(int)v.x, (int)v.y].isValid = false;
        }

        //set node to not have doors to invalids
        foreach(Vector2 v in invalidRoomPositions) {
            SetClosedNeighbor(n, room, (int)v.x, (int)v.y, true);
            roomArray[(int)v.x, (int)v.y].isValid = false;
        }

        //Recheck end nodes
        RecheckAllEndNodes();
        return newRoomList.Count;
    }

    void InvalidateCorners(Node n) {
        Vector2[] corners = GetCornerNodesPositions(n);
        for(int i = 0; i < corners.Length; i++) {
            if (IsValidCoordinate((int)corners[i].x, (int)corners[i].y)) {
                roomArray[(int)corners[i].x, (int)corners[i].y].isValid = false;
            }
        }
    }


    bool IsValidCoordinate(int x, int y) {
        if ((x < 0 || x >= xLength) || (y < 0 || y >= yLength)) {
            return false;
        }
        return true;
    }

    void RecheckAllEndNodes() {
        foreach(Node n in new List<Node>(endRoomNodes)) {
            CheckEndNode(n);
        }
    }

    Node GetRandomEndNode() {
        if(endRoomNodes.Count == 0) {
            return null;
        }
        return endRoomNodes[Random.Range(0, endRoomNodes.Count - 1)];
    }

    bool CheckEndNode(Node n) {
        Vector2[] neighbors = GetNeighborNodesPositions(n);
        for(int i = 0; i < 4; i++) {
            if (IsValidCoordinate((int)neighbors[i].x, (int)neighbors[i].y)) {
                //print("Pair " + neighbors[i].x.ToString() + " " + neighbors[i].y.ToString());
                Node b = roomArray[(int)neighbors[i].x, (int)neighbors[i].y];
                if(!b.isTaken && b.isValid) {
                    AddNodeEnd(n);
                    return true;
                }
            }
        }
        RemoveNodeEnd(n);
        return false;
    }

    bool RemoveNodeEnd(Node n) {
        if (endRoomNodes.Contains(n)) {
            endRoomNodes.Remove(n);
            return true;
        }
        return false;
    }

    void AddNodeEnd(Node n) {
        if (!endRoomNodes.Contains(n)) {
            endRoomNodes.Add(n);
        }
    }


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


    GameObject GenerateRoom(int x, int y) {
        GameObject r = Instantiate(potentialRoomSpawns[0]);
        r.transform.position = new Vector3(x * nodeLength, 0, y * nodeLength);
        r.gameObject.name = "X: " + (x).ToString() + ", Y: " + (y).ToString();
        return r;
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

    Vector2[] GetCornerNodesPositions(Node n) {
        Vector2[] toCheck = new Vector2[4];
        toCheck[0] = new Vector2(n.xPos + 1, n.yPos + 1);
        toCheck[1] = new Vector2(n.xPos + 1, n.yPos - 1);
        toCheck[2] = new Vector2(n.xPos - 1, n.yPos - 1);
        toCheck[3] = new Vector2(n.xPos - 1, n.yPos + 1);
        return toCheck;
    }


}
