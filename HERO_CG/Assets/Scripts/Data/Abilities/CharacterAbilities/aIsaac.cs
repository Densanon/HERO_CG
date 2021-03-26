using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aIsaac : Ability
{
    private void Awake()
    {
        myType = Type.Character;
        secondaryType = Type.Passive;
        Name = "ISAAC";
        Description = "(P) When another hero is defeated, you may draw a card from your Discard Pile that was there before the hero was defeated.";
    }
}
