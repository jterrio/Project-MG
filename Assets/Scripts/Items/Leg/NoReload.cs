using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoReload : WepItem {

    public override void GetItemEffects() {
        ItemManager.im.gunDelegate += ItemManager.im.NoReload;
    }
}
