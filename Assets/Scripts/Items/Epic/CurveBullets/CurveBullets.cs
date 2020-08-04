using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveBullets : WepItem {

    public override void GetItemEffects() {
        ItemManager.im.bulletVelocityDelegate += ItemManager.im.CurveBullet;
    }
}
