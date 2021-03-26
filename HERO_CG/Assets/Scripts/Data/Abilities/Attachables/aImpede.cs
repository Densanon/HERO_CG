using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aImpede : Ability
{
    private void Awake()
    {
        myType = Type.Passive;
        Name = "IMPEDE";
        Description = "(P) You may prevent the effects of one Active Ability per opponent turn.";
    }
}
