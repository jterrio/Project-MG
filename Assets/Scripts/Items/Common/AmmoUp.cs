using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoUp : WepItem {

    

    public override void GetItemEffects() {
        GameManager.gm.p.wepAmmoIncrease += wepAmmoIncrease;
    }

}
