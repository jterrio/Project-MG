using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomVariantHoleCenter : Room {

    public List<Vector2> holes;

    public new void Start() {
        base.Start();
        NullHoles();
    }

    public void NullHoles() {
        foreach (Vector2 v in holes) {
            roomArray[(int)v.x, (int)v.y].isHole = true;
            Destroy(roomArray[(int)v.x, (int)v.y].grass);
            roomArray[(int)v.x, (int)v.y].grass = null;
        }
    }
}
