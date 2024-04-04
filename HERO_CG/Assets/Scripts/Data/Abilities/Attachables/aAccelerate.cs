using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aAccelerate : Ability
{
    private void Awake()
    {
        base.Awake();

        myType = Type.Activate;
        Name = "ACCELERATE";
        Description = "(A) Target player draws 3 cards from their Enhancement Deck then discards 2 from their hand.";
    }

    public override void AbilityAwake()
    {
        base.AbilityAwake();
        OnCharacterAbilityRequest.Invoke("Accelerate");
    }

    public override void AbilityCompleteCleanup(string abilityName)
    {
        base.AbilityCompleteCleanup(abilityName);
        if (abilityName == Name) ActivateAbility();
    }
}
