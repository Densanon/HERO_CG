using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aImpede : Ability
{
    public static bool impedeNeedResponse = false;
    public static bool isImpede = false;
    private void Awake()
    {
        base.Awake();
        myType = Type.Passive;
        Name = "IMPEDE";
        Description = "(P) You may prevent the effects of one Active Ability per opponent turn.";
    }

    protected override void ResetOncePerTurn()
    {
        base.ResetOncePerTurn();
        if (Referee.myTurn && myHero.myPlacement == CardData.FieldPlacement.Opp) impedeNeedResponse = true;
    }

    public override void PassiveCheck(PassiveType passiveType)
    {
        if(!oncePerTurnUsed && passiveType == PassiveType.AbilityActivated && Referee.myTurn && myHero.myPlacement == CardData.FieldPlacement.Opp)
        {
            OnCheckNeedResponse.Invoke("Impede");
            base.PassiveCheck(passiveType);
        }
    }

    public override void AbilityCompleteCleanup(string abilityName)
    {
        if(abilityName == Name)
        {
            base.AbilityCompleteCleanup(abilityName);
            ChangeOncePerTurn(true);
            isImpede = false;
        }
    }
}
