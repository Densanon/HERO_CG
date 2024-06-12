//Created by Jordan Ezell
//Last Edited: 6/12/24 Jordan

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aBoost : Ability
{
    private void Awake()
    {
        base.Awake();
        myType = Type.Activate;
        Name = "BOOST";
        Description = "(A) Discard 2 cards from your hand to choose any one player who must draw one card for every hero (regardless of who the heroes belong to) on the field.";
    }

    public override void AbilityAwake()
    {
        base.AbilityAwake();
        OnCharacterAbilityRequest.Invoke("Boost");
    }

    public override void AbilityCompleteCleanup(string abilityName)
    {
        if(abilityName == Name)
        {
            ChangeOncePerTurn(true);
        }
    }
}
