using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aKay : Ability
{
    protected override void Awake()
    {
        base.Awake();

        myType = Type.Character;
        secondaryType = Type.Activate;
        Name = "KAY";
        Description = "(A) Play a card to the field from your hand.";
    }

    public override void AbilityAwake()
    {
        base.AbilityAwake();

        //OnCanPlayAcard
    }
}
