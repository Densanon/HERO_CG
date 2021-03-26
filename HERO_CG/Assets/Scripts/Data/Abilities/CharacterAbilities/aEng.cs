using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aEng : Ability
{
    private void Awake()
    {
        myType = Type.Character;
        secondaryType = Type.Activate;
        Name = "ENG";
        Description = "(A) After an Action, heal one hero.";
    }
}
