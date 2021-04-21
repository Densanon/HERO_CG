using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aGrit : Ability
{
    protected override void Awake()
    {
        base.Awake();

        myType = Type.Character;
        secondaryType = Type.Passive;
        Name = "GRIT";
        Description = " (P) Grit may gain +20 attack for every fatigued hero.";
    }

    public override void PassiveCheck(PassiveType passiveType)
    {
        base.PassiveCheck(passiveType);

        if(passiveType == PassiveType.HeroFatigued)
        {
            myHero.NewAbilityAttModifier(CardDataBase.herosFatigued * 20);
        }
    }
}
