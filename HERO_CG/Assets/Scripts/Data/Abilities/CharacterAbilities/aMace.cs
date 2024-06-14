using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aMace : Ability { 
    public static bool maceDoubleActive = false;
    public static bool used = false;

    private void Awake()
    {
        base.Awake();
        myType = Type.Character;
        secondaryType = Type.Passive;
        Name = "MACE";
        Description = "(P) You may double the Total Attack for one attack that occurs on your turn.";
    }

    public override void PassiveCheck(PassiveType passiveType)
    {
        base.PassiveCheck(passiveType);
        if (passiveType == PassiveType.BattleCalculation &&  oncePerTurnUsed == false && myHero.myPlacement == CardData.FieldPlacement.Mine)
        {
            Debug.Log("Mace says it is time to check to play ability.");
            used = oncePerTurnUsed;
            OnAbilityRequest?.Invoke("Mace");
        }
    }

    public override void AbilityCompleteCleanup(string abilityName)
    {
        base.AbilityCompleteCleanup(abilityName);
        if (abilityName == Name)
        {
            ChangeOncePerTurn(true);
            maceDoubleActive = false;
        }
    }
}
