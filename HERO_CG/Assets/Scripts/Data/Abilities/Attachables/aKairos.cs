﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aKairos : Ability
{
    private void Awake()
    {
        base.Awake();
        myType = Type.Activate;
        Name = "KAIROS";
        Description = "(A) Discard this hero to recruit up to 2 heroes from the top of the Reserves straight to your play area.";
    }
 
    public override void AbilityAwake()
    {
        base.AbilityAwake();
        OnAbilityRequest.Invoke("Kairos");
    }
}
