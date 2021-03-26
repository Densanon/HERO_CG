using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aAccelerate : Ability
{
    private void Awake()
    {
        myType = Type.Activate;
        Name = "ACCELERATE";
        Description = "(A) Target player draws 3 cards from their Enhancement Deck then discards 2 from their hand.";
    }
}
