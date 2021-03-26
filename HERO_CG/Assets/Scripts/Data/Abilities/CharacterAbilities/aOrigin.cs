using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aOrigin : Ability
{
    private void Awake()
    {
        myType = Type.Character;
        secondaryType = Type.Passive;
        Name = "ORIGIN";
        Description = "(P) During each opponent’s turn, you may block one attack against Origin.";
    }
}
