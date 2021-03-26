using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aGrit : Ability
{
    private void Awake()
    {
        myType = Type.Character;
        secondaryType = Type.Passive;
        Name = "GRIT";
        Description = " (P) Grit may gain +20 attack for every fatigued hero.";
    }
}
