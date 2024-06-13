using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aCounterMeasures : Ability
{
    private void Awake()
    {
        base.Awake();
        myType = Type.Passive;
        Name = "COUNTER-MEASURES";
        Description = "(P) When attacked, this hero may combine its own Total Defense with the defense of the attacking hero(es), excluding their enhancements.";
    }

    public override void PassiveCheck(PassiveType passiveType)
    {
        if(passiveType == PassiveType.BattleCalculation && Referee.myTurn && myHero.myPlacement == CardData.FieldPlacement.Opp)
        {
            base.PassiveCheck(passiveType);
            myHero.ResetValues();
            OnCheckNeedResponse.Invoke("Counter-Measures");
        }else if(passiveType == PassiveType.ActionComplete && Referee.myTurn && myHero.myPlacement == CardData.FieldPlacement.Opp)
        {
            myHero.ResetValues();
        }
    }

    public override void AbilityCompleteCleanup(string abilityName)
    {
        base.AbilityCompleteCleanup(abilityName);
        myHero.UseCounterMeasures();
    }
}
