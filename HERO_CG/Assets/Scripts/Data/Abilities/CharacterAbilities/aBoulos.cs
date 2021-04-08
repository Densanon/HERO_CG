﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aBoulos : Ability
{
    private void Awake()
    {
        myType = Type.Character;
        secondaryType = Type.Passive;
        Name = "BOULOS";
        Description = "(P) For each card in your hand, Boulos gains +10 defense.";
    }

    public override void PassiveCheck(PassiveType passiveType)
    {
        base.PassiveCheck(passiveType);

        if(passiveType == PassiveType.HandCardAdjustment)
        {
            myHero.NewAbilityDefModifier(CardDataBase.handSize);
        }
    }
}
