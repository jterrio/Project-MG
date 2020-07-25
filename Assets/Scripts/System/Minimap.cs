using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour {

    public Camera c;
    public Material explored;

    private void Update() {
        if (GameManager.gm.player != null) {
            GameManager.gm.playerMinimapObject.transform.position = RoomManager.rm.GetRoomForMinimap() + new Vector3(0, 10f, 0);
        }
    }

    public void SetExplored(GameObject g) {
        g.GetComponent<MeshRenderer>().enabled = true;
        g.GetComponent<MeshRenderer>().sharedMaterial = explored;
    }

    public void SetUnexplored(GameObject g) {
        g.GetComponent<MeshRenderer>().enabled = true;
    }

}
