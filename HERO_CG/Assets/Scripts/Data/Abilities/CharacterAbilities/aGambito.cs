using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aGambito : Ability
{
    protected override void Awake()
    {
        base.Awake();

        myType = Type.Character;
        secondaryType = Type.Passive;
        Name = "GAMBITO";
        Description = "(P) When any hero is fatigued, all players must discard a random card from their hand.";
    }

    public override void PassiveCheck(PassiveType passiveType)
    {
        if (!passiveCheckable)
            return;

        base.PassiveCheck(passiveType);

        if(passiveType == PassiveType.HeroFatigued)
        {
            Debug.Log("Everyone losses a card.");
            OnDiscardCard?.Invoke("All", "Random", 1);
        }
    }
}
