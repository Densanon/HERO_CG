using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aHardened : Ability
{
    private void Awake()
    {
        myType = Type.Passive;
        Name = "HARDENED";
        Description = "(P) For every opposing card on the field, this hero may gain +10 defense.";
    }
}
