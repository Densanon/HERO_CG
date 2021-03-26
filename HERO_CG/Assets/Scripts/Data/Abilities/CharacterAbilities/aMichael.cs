using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aMichael : Ability
{
    private void Awake()
    {
        myType = Type.Character;
        secondaryType = Type.Activate;
        Name = "MICHAEL";
        Description = "(A) Draw a card from your Enhancement Deck to your hand.";
    }
}
