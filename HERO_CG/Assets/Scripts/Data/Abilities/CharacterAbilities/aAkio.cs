using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aAkio : Ability
{
    private void Awake()
    {
        myType = Type.Character;
        secondaryType = Type.Passive;
        Name = "AKIO";
        Description = "(P) When Akio causes another hero to be fatigued, Akio is not fatigued.";
    }
}
