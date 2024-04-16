using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aBolster : Ability
{
    private void Awake()
    {
        base.Awake();

        myType = Type.Passive;
        Name = "BOLSTER";
        Description = "(P) For every strengthened hero on the field, this hero may gain +10 attack.";
    }

    public override void PassiveCheck(PassiveType passiveType)
    {
        if (!passiveCheckable)
            return;

        base.PassiveCheck(passiveType);
        if(passiveType == PassiveType.ValueSet)
        {
            myHero.NewAbilityAttModifier(CardDataBase.herosModified * 10);
            Debug.Log("Bolstered hero is getting adjustment.");
        }
    }
}
