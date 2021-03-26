using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aCollateralDamage : Ability
{
    private void Awake()
    {
        myType = Type.Passive;
        Name = "COLLATERAL DAMAGE";
        Description = "(P) If this hero is used to defeat a hero, you may determine one other strengthened hero to be fatigued.";
    }
}
