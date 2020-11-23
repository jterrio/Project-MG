using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceOffEnemy : WepItem {

    public override void GetItemEffects() {
        GameManager.gm.p.gun.timesToBounceOffEnemies += 1;
        if (GameManager.gm.p.gun.timesToBounceOffEnemies == 1) {
            ItemManager.im.bulletHitDelegate += ItemManager.im.BounceOffEnemy;
        }
    }

}
