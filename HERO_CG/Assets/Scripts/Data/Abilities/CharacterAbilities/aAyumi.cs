using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aAyumi : Ability
{
    protected override void Awake()
    {
        base.Awake();

        myType = Type.Character;
        secondaryType = Type.Passive;
        Name = "AYUMI";
        Description = "(P) Whenever a hero is recruited, you may draw a card from your Enhancement Deck.";
    }

    public override void PassiveCheck(PassiveType passiveType)
    {
        base.PassiveCheck(passiveType);

        if(passiveType == PassiveType.HeroRecruited && myHero.myPlacement == CardData.FieldPlacement.Mine)
        {
            Debug.Log($"{myHero.Name} should be activating the draw pasive.");
            OnNeedCardDraw?.Invoke(1);
        }
    }
}
