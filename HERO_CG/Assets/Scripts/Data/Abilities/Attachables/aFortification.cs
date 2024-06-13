using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aFortification : Ability
{
    private void Awake()
    {
        base.Awake();
        myType = Type.Passive;
        Name = "FORTIFICATION";
        Description = "(P) For every hero on the field, this hero may gain +10 defense.";
        myHero.NewAbilityDefModifier(CardDataBase.heroCount * 10);
    }

    public override void PassiveCheck(PassiveType passiveType)
    {
        if(passiveType == PassiveType.CharacterSpawn || passiveType == PassiveType.CharacterDestroyed)
        {
            base.PassiveCheck(passiveType);
            myHero.NewAbilityDefModifier(CardDataBase.heroCount * 10);
        }
    }
}
