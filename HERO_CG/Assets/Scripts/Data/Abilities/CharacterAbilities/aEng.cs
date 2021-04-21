using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aEng : Ability
{
    protected override void Awake()
    {
        base.Awake();

        myType = Type.Character;
        secondaryType = Type.Activate;
        Name = "ENG";
        Description = "(A) After an Action, heal one hero.";
    }

    public override void PassiveCheck(PassiveType passiveType)
    {
        base.PassiveCheck(passiveType);

        if(passiveType == PassiveType.ActionComplete)
        {
            OnActivateable?.Invoke(this);
            OnHoldTurn?.Invoke();
        }
    }

    public override void Target(CardData card)
    {
        base.Target(card);
        Debug.Log($"Healing {card.Name} from {myHero.Name}");
        card.Heal(false);
        OnAbilityUsed();
    }
}
