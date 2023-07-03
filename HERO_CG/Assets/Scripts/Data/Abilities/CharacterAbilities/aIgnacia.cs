//Created by Jordan Ezell
//Last Edited: 6/30/23 Jordan

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aIgnacia : Ability
{
    protected override void Awake()
    {
        base.Awake();

        ChangeCanActivate(false);
        myType = Type.Character;
        secondaryType = Type.Activate;
        Name = "IGNACIA";
        Description = "(A) Ignacia may attack once per turn while fatigued (this cannot be combined with any other attack).";
    }

    public override void AbilityAwake()
    {
        base.AbilityAwake();

        if(myHero.Exhausted && !oncePerTurnUsed)
        {
            OnRequestTargeting?.Invoke(Referee.TargetType.OppHero);
            OnTargetedFrom?.Invoke(this);
        }
    }

    public override void PassiveCheck(PassiveType passiveType)
    {
        base.PassiveCheck(passiveType);

        if(passiveType == PassiveType.HeroFatigued && myHero.Exhausted)
        {
            ChangeCanActivate(true);
        }else if(passiveType == PassiveType.HeroFatigued && !myHero.Exhausted)
        {
            ChangeCanActivate(false);
        }
    }

    public override void Target(CardData card)
    {
        base.Target(card);

        if(card.myPlacement == CardData.FieldPlacement.Opp)
        {
            card.DamageCheck(myHero.Attack);
            OnAbilityUsed?.Invoke();
            ChangeOncePerTurn(true);
        }
    }
}
