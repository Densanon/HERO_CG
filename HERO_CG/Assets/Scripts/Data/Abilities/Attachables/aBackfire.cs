using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aBackfire : Ability
{
    private void Awake()
    {
        base.Awake();

        myType = Type.Activate;
        Name = "BACKFIRE";
        Description = "(A) Target opponent reveals all of their Abilities on the field; you may use one of the Active Abilities as if it was your own, even if it is one that is found on a hero’s card, regardless of timing.";
    }

    public override void AbilityAwake()
    {
        base.AbilityAwake();
        OnAbilityRequest?.Invoke("Backfire");
    }

    public override void AbilityCompleteCleanup(string abilityName)
    {
        base.AbilityCompleteCleanup(abilityName);
        if (abilityName == Name) ActivateAbility();
    }
}
