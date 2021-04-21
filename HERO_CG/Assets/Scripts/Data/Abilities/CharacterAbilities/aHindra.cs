using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aHindra : Ability
{
    protected override void Awake()
    {
        base.Awake();

        myType = Type.Character;
        secondaryType = Type.Activate;
        Name = "HINDRA";
        Description = "(A) Choose one opponent to prevent from playing Ability Cards to the field on their next turn.";
    }

    public override void AbilityAwake()
    {
        base.AbilityAwake();

        OnPreventAbilitiesToFieldForTurn?.Invoke();
    }
}
