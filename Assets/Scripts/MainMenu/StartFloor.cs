using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartFloor : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        LevelManager.lm.LoadFarmLevel();
    }
}
