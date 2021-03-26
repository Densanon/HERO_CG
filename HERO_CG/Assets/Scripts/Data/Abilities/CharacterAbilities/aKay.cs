using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aKay : Ability
{
    private void Awake()
    {
        myType = Type.Character;
        secondaryType = Type.Activate;
        Name = "KAY";
        Description = "(A) Play a card to the field from your hand.";
    }
}
