using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aIgnacia : Ability
{
    protected override void Awake()
    {
        base.Awake();

        myType = Type.Character;
        secondaryType = Type.Activate;
        Name = "IGNACIA";
        Description = "(A) Ignacia may attack once per turn while fatigued (this cannot be combined with any other attack).";
    }

    public override void AbilityAwake()
    {
        base.AbilityAwake();

        if(myHero.Exhausted)
            OnSetActive?.Invoke(this);
    }

    public override void Target(CardData card)
    {
        base.Target(card);

        if(card.myPlacement == CardData.FieldPlacement.Opp && !oncePerTurnUsed)
        {
            oncePerTurnUsed = true;
            card.DamageCheck(myHero.Attack);
        }
    }
}
