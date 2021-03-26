using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aChristoph : Ability
{
    private void Awake()
    {
        myType = Type.Character;
        secondaryType = Type.Passive;
        Name = "CHRISTOPH";
        Description = "(P) When an attack against Christoph is resolved, you may choose one of the attacking heroes to be defeated. This occurs even if Christoph is defeated.";
    }
}
