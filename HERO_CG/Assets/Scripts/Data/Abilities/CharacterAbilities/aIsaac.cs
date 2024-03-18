using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aIsaac : Ability
{
    public static bool AisaacDraw = false;

    protected override void Awake()
    {
        base.Awake();

        myType = Type.Character;
        secondaryType = Type.Passive;
        Name = "ISAAC";
        Description = "(P) When another hero is defeated, you may draw a card from your Discard Pile that was there before the hero was defeated.";
    }

    public override void PassiveCheck(PassiveType passiveType)
    {
        base.PassiveCheck(passiveType);

        if(passiveType == PassiveType.CharacterDestroyed && myHero.myPlacement == CardData.FieldPlacement.Mine)
        {
            OnNeedDrawFromDiscard?.Invoke("Discard");
            //need to save what all cards are in discard prior to battle
            //need to populate said cards on screen to be drawn from
        }
    }
}
