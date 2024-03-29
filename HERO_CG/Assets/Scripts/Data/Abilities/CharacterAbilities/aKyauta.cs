using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aKyauta : Ability
{
    protected override void Awake()
    {
        base.Awake();
    
        myType = Type.Character;
        secondaryType = Type.Activate;
        Name = "KYAUTA";
        Description = "(A) After an Action, fatigue one of your heros to recruit one hero to your play area from the Reserves.";
    }


    public override void AbilityAwake()
    {
        base.AbilityAwake();

        OnRequestTargeting?.Invoke(Referee.TargetType.MyHero);
        OnTargetedFrom?.Invoke(this);
    }

    public override void Target(CardData card)
    {
        base.Target(card);
        card.Exhaust(false);
        OnNeedPlayFromReserve?.Invoke();
        OnAbilityUsed?.Invoke();
    }

    public override void PassiveCheck(PassiveType passiveType)
    {
        if(passiveType == PassiveType.TurnStart)
        {
            ChangeOncePerTurn(true);
            ChangeCanActivate(false);
        }
        if(passiveType == PassiveType.ActionComplete)
        {
            ChangeOncePerTurn(false);
            ChangeCanActivate(true);
            //need to have heros to fatigue
            //recruit a hero to play area
                //target hero
                //don't draw but play to field
        }
    }
}
