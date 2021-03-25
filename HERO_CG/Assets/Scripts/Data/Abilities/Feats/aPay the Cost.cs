using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aPaytheCost : Ability
{
    //"PAY THE COST", "(H) Fatigue one hero in your play area, to remove one hero from the field."

    CardData Target1;

    #region Unity Methods
    private void Awake()
    {
        myType = Ability.Type.Feat;
        Name = "PAY THE COST";
        Description = "(H) Fatigue one hero in your play area, to remove one hero from the field.";
    }
    #endregion

    public override void Target(CardData card)
    {
        if (Target1 == null && !card.Exhausted)
        {
            Target1 = card;
            //this is immediate exhaust, may look into allowing the player to back out
            card.Exhaust(false);
            return;
        }

        //may be prevented from a damage blocker?
        card.DamageCheck(1000);
    }
}
