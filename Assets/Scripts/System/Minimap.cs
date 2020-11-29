using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour {

    public Camera c;
    public Material explored;

    private void LateUpdate() {
        transform.rotation = Quaternion.Euler(90f, GameManager.gm.player.transform.eulerAngles.y, 0f);
    }

    public void SetExplored(GameObject g) {
        g.GetComponent<MeshRenderer>().enabled = true;
        g.GetComponent<MeshRenderer>().sharedMaterial = explored;
    }

    public void SetUnexplored(GameObject g) {
        g.transform.parent = null;
        g.GetComponent<MeshRenderer>().enabled = true;
    }

}
