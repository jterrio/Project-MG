using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRateUp : WepItem {

    

    public override void GetItemEffects() {
        GameManager.gm.p.wepFireIncrease += wepFireIncrease;
    }

}
