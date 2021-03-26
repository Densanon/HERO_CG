using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aIzumi : Ability
{
    private void Awake()
    {
        myType = Type.Character;
        secondaryType = Type.Passive;
        Name = "IZUMI";
        Description = "(P) Allied heroes may gain +20 defense while Izumi is in play. Fatigued heroes gain this after their defense is halved.";
    }
}
