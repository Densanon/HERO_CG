using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aChristoph : Ability
{
    private void Awake()
    {
        myType = Type.Character;
        secondaryType = Type.Passive;
        Name = "CHRISTOPH";
        Description = "(P) When an attack against Christoph is resolved, you may choose one of the attacking heroes to be defeated. This occurs even if Christoph is defeated.";
    }

    public override void PassiveCheck(PassiveType passiveType)
    {
        base.PassiveCheck(passiveType);

        if(passiveType == PassiveType.BattleComplete && myHero == PhotonGameManager.DefendingHero)
        {
            OnOpponentAbilityActivation?.Invoke(this);
        }
    }

    public override void Target(CardData card)
    {
        base.Target(card);

        if (PhotonGameManager.PreviousAttackers.Contains(card))
        {
            card.DamageCheck(1000);
            OnAbilityUsed?.Invoke();
        }
    }
}
