using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turnip : Enemy {

    private State growState = State.GROW;
    public float range = 10f;
    public float growth = 1.2f;
    public float moveSpeed = 5f;

    public enum State {
        GROW,
        HARVEST,
        FEAST
    }

    new void Update() {
        base.Update();
        switch (growState) {
            case State.GROW:
                DetectGrow();
                break;
            case State.HARVEST:
                Harvest();
                break;
            case State.FEAST:
                TryFeast();
                break;
        }
    }

    void DetectGrow() {
        if(Vector3.Distance(GameManager.gm.player.transform.position, transform.position) <= range) {
            growState = State.HARVEST;
        }
    }

    void Harvest() {
        transform.position = new Vector3(transform.position.x, transform.position.y + growth, transform.position.z);
        growState = State.FEAST;
    }

    void TryFeast() {
        if (Vector3.Distance(GameManager.gm.player.transform.position, transform.position) > range * 1.5) {
            growState = State.GROW;
            transform.position = new Vector3(transform.position.x, transform.position.y - growth, transform.position.z);
        } else {
            LookAt(GameManager.gm.player);
            transform.position += new Vector3(transform.forward.x, 0, transform.forward.z) * moveSpeed * Time.deltaTime;
        }
    }



}
