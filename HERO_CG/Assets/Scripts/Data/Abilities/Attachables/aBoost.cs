using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aBoost : Ability
{
    private void Awake()
    {
        myType = Type.Activate;
        Name = "BOOST";
        Description = "(A) Discard 2 cards from your hand to choose any one player who must draw one card for every hero (regardless of who the heroes belong to) on the field.";
    }
}
