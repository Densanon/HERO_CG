using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aRohan : Ability
{
    private void Awake()
    {
        base.Awake();

        myType = Type.Character;
        secondaryType = Type.Activate;
        Name = "ROHAN";
        Description = "(A) For every 2 fatigued heroes, recruit one hero from Hero HQ or Reserves to your hand.";
    }

    public override void AbilityAwake()
    {
        base.AbilityAwake();
        OnCharacterAbilityRequest?.Invoke("Rohan");
    }

    public override void AbilityCompleteCleanup(string abilityName)
    {
        base.AbilityCompleteCleanup(abilityName);
        if (abilityName == Name) ActivateAbility();
    }
}
