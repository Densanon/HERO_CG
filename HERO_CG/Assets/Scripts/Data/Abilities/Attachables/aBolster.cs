using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aBolster : Ability
{
    private void Awake()
    {
        myType = Type.Passive;
        Name = "BOLSTER";
        Description = "(P) For every strengthened hero on the field, this hero may gain +10 attack. It does not matter who the heroes belong to.";
    }
}
