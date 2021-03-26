using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aZoe : Ability
{
    private void Awake()
    {
        myType = Type.Character;
        secondaryType = Type.Activate;
        Name = "ZOE";
        Description = "(A) Before performing an Action, heal any hero(es) of your choice.";
    }
}
