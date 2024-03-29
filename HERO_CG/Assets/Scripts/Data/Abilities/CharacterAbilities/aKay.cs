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
        Description = "(A) Play a card to the field from your hand at the beginning or end of your turn.";
    }

    public override void AbilityAwake()
    {
        base.AbilityAwake();
        ChangeCanActivate(false);
        OnActivateKayAbility?.Invoke();
    }

    public override void PassiveCheck(PassiveType passiveType)
    {
        if(passiveType == PassiveType.TurnStart)
        {
            Debug.Log("Kay is ready to play!");
            ChangeCanActivate(true);
        }
    }
}
