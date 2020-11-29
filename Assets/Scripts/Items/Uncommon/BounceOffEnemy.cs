using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceOffEnemy : WepItem {

    public override void GetItemEffects() {
        GameManager.gm.p.numberOfBulletBounces += 1;
        if (GameManager.gm.p.numberOfBulletBounces == 1) {
            ItemManager.im.bulletHitDelegate += ItemManager.im.BounceOffEnemy;
        }
    }

}
