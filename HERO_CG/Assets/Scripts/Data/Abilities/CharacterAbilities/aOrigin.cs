using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aOrigin : Ability
{
    public static bool blockActive = false;

    private void Awake()
    {
        base.Awake();

        myType = Type.Character;
        secondaryType = Type.Passive;
        Name = "ORIGIN";
        Description = "(P) During each opponent’s turn, you may block one attack against Origin.";
    }

    public override void PassiveCheck(PassiveType passiveType)
    {
        base.PassiveCheck(passiveType);
        if (passiveType == PassiveType.BattleCalculation && myHero.myPlacement == CardData.FieldPlacement.Opp && !oncePerTurnUsed) OnCheckNeedResponse?.Invoke("Origin");
    }

    protected override void ResetOncePerTurn()
    {
        base.ResetOncePerTurn();
        blockActive = false;
    }

    public override void AbilityCompleteCleanup(string abilityName)
    {
        base.AbilityCompleteCleanup(abilityName);
        if (abilityName == "ORIGIN") ActivateAbility();
    }
}
