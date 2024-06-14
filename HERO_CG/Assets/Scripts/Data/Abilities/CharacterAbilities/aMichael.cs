using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aMichael : Ability
{
    private void Awake()
    {
        base.Awake();

        myType = Type.Character;
        secondaryType = Type.Activate;
        Name = "MICHAEL";
        Description = "(A) Draw a card from your Enhancement Deck to your hand.";
    }

    public override void AbilityAwake()
    {
        base.AbilityAwake();
        OnAbilityRequest?.Invoke("Michael");
    }

    public override void AbilityCompleteCleanup(string abilityName)
    {
        base.AbilityCompleteCleanup(abilityName);
        if (abilityName == Name) ActivateAbility();
    }
}
