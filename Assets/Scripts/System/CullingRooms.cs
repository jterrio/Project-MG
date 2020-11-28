using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CullingRooms : MonoBehaviour {

    public void SetCull(Room oldRoom, Room newRoom) {
        foreach(GameObject neighbor in oldRoom.neighbors) {
            if(neighbor != newRoom.gameObject) {
                neighbor.gameObject.SetActive(false);
            }
        }
        foreach(GameObject neighbor in newRoom.neighbors) {
            neighbor.gameObject.SetActive(true);
        }
    }

    public void ValidateStartRoom() {
        Room start = RoomManager.rm.currentRoom;
        start.gameObject.SetActive(true);
        foreach(GameObject neighbor in start.neighbors) {
            neighbor.gameObject.SetActive(true);
        }
    }

    public void CullOutRoom(Room r) {
        r.gameObject.SetActive(false);
        foreach (GameObject neighbor in r.neighbors) {
            neighbor.gameObject.SetActive(false);
        }
    }
}
