using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aCollateralDamage : Ability
{
    private void Awake()
    {
        base.Awake();
        myType = Type.Passive;
        Name = "COLLATERAL DAMAGE";
        Description = "(P) If this hero is used to defeat a hero, you may determine one other strengthened hero to be fatigued.";
    }

    public override void PassiveCheck(PassiveType passiveType)
    {
        if(passiveType == PassiveType.CharacterDestroyed && CardDataBase.herosFatigued < CardDataBase.heroCount && Referee.PreviousAttackers.Contains(myHero) && Referee.myTurn)
        {
            base.PassiveCheck(passiveType);
            OnCharacterAbilityRequest.Invoke("Collateral Damage");
        }
    }

    public override void AbilityCompleteCleanup(string abilityName)
    {
        base.AbilityCompleteCleanup(abilityName);
        OnRequestTargeting?.Invoke(Referee.TargetType.AllUnExhausted);
    }
}
