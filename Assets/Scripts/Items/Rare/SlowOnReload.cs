using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowOnReload : WepItem {

    public override void GetItemEffects() {
        GameManager.gm.p.gun.reloadTime *= 0.35f;
        ItemManager.im.beforeReloadDelegate += ItemManager.im.SlowOnReloadStart;
        ItemManager.im.afterReloadDelegate += ItemManager.im.SlowOnReloadFinish;
    }

}
