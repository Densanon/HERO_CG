using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aChristoph : Ability
{
    protected override void Awake()
    {
        base.Awake();

        myType = Type.Character;
        secondaryType = Type.Passive;
        Name = "CHRISTOPH";
        Description = "(P) When an attack against Christoph is resolved, you may choose one of the attacking heroes to be defeated. This occurs even if Christoph is defeated.";
    }

    public override void PassiveCheck(PassiveType passiveType)
    {
        base.PassiveCheck(passiveType);

        if(passiveType == PassiveType.BattleComplete && myHero == Referee.DefendingHero)
        {
            Debug.Log($"{Name} activating switchoff.");
            OnOpponentAbilityActivation?.Invoke(this);
            OnHoldTurn?.Invoke(true);
        }
    }

    public override void AbilityAwake()
    {
        base.AbilityAwake();
        OnRequestTargeting?.Invoke(Referee.TargetType.OppHero);
        OnTargetedFrom?.Invoke(this);
    }

    public override void TargetOpponent(CardData card)
    {
        base.Target(card);
        Debug.Log($"{Name} has targeted {card.Name}");

        if (Referee.PreviousAttackers.Contains(card))
        {
            card.DamageCheck(1000);
            OnHandOverControl?.Invoke();
            return;
        }
        Debug.Log("You didn't pick a right target.");
        
    }
}
