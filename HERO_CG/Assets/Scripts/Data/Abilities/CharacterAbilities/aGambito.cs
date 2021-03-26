using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aGambito : Ability
{
    private void Awake()
    {
        myType = Type.Character;
        secondaryType = Type.Passive;
        Name = "GAMBITO";
        Description = "(P) When any hero is fatigued, all players must discard a random card from their hand.";
    }
}
