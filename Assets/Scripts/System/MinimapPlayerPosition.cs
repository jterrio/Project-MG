using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapPlayerPosition : MonoBehaviour {

    GameObject old;

    private void Update() {
        RaycastHit hit;
        if (Physics.Raycast(this.gameObject.transform.position, Vector3.down, out hit, Mathf.Infinity, GameManager.gm.minimap)) {
            //start room
            if(old == null) {
                GameManager.gm.playerMinimapObject.transform.position = hit.collider.gameObject.transform.position + new Vector3(0, 10f, 0);
                GameManager.gm.playerMinimapObject.SetActive(true);
                old = hit.collider.gameObject;
                return;
            }
            if(hit.collider.gameObject != old) {
                GameManager.gm.playerMinimapObject.transform.position = hit.collider.gameObject.transform.position + new Vector3(0, 10f, 0);
                GameManager.gm.playerMinimapObject.SetActive(true);
                RoomManager.rm.cr.SetCull(old.transform.parent.parent.gameObject, hit.collider.gameObject.transform.parent.parent.gameObject);
                old = hit.collider.gameObject;
                return;
            }

        }
    }

}
