using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aBoulos : Ability
{
    protected override void Awake()
    {
        base.Awake();

        myType = Type.Character;
        secondaryType = Type.Passive;
        Name = "BOULOS";
        Description = "(P) For each card in your hand, Boulos gains +10 defense.";
    }

    public override void PassiveCheck(PassiveType passiveType)
    {
        if (!passiveCheckable)
            return;

        base.PassiveCheck(passiveType);

        if( passiveType == PassiveType.HandCardAdjustment)
        {
            Debug.Log($"{myHero.Name} should be activating the defence pasive.");
            myHero.NewAbilityDefModifier(CardDataBase.handSize*10);
        }
    }
}
