using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aGoingNuclear : Ability
{
    private void Awake()
    {
        base.Awake();
        myType = Type.Passive;
        Name = "GOING NUCLEAR";
        Description = "(P) When this hero is attacked, all cards on the field, including this one must be removed, except for SkyBases.";
    }

    public override void PassiveCheck(PassiveType passiveType)
    {
        if(passiveType == PassiveType.BattleCalculation && myHero.myPlacement == CardData.FieldPlacement.Opp && myHero == Referee.DefendingHero)
        {
            base.PassiveCheck(passiveType);
            OnGoingNuclear.Invoke();
        }
    }
}
