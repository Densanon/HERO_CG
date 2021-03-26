using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aShielding : Ability
{
    private void Awake()
    {
        myType = Type.Passive;
        Name = "SHEILDING";
        Description = "(P) If fatigued, this hero may gain +20 defense for every strengthened hero. This bonus is added after the Total Defense is halved.";
    }
}
