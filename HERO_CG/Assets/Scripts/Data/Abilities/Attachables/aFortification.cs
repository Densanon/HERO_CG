using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aFortification : Ability
{
    private void Awake()
    {
        myType = Type.Passive;
        Name = "FORTIFICATION";
        Description = "(P) For every hero on the field, this hero may gain +10 defense.";
    }
}
