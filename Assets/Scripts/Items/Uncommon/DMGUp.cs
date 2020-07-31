using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMGUp : WepItem {


    public override void GetItemEffects() {
        GameManager.gm.p.wepDMGIncrease += wepDMGIncrease;
    }
}

