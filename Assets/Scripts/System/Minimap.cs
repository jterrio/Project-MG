using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour {

    public Camera c;
    public Material explored;

    public void SetExplored(GameObject g) {
        g.GetComponent<MeshRenderer>().enabled = true;
        g.GetComponent<MeshRenderer>().sharedMaterial = explored;
    }

    public void SetUnexplored(GameObject g) {
        g.GetComponent<MeshRenderer>().enabled = true;
    }

}
