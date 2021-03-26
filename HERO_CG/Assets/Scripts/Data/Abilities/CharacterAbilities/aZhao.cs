using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aZhao : Ability
{
    private void Awake()
    {
        myType = Type.Character;
        secondaryType = Type.Passive;
        Name = "ZHAO";
        Description = "(P) Allied heroes may gain +10 attack while Zhao is in play. When Zhao is defeated/removed play a card to the field.";
    }
}
