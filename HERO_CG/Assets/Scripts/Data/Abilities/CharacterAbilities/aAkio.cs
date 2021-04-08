using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aAkio : Ability
{
    private void Awake()
    {
        myType = Type.Character;
        secondaryType = Type.Passive;
        Name = "AKIO";
        Description = "(P) When Akio causes another hero to be fatigued, Akio is not fatigued.";
    }

    public override void PassiveCheck(PassiveType type)
    {
        base.PassiveCheck(type);

        //this should only get fired off when Akio battles an opponent and at least exhausts them.
        if(type == PassiveType.BattleComplete && PhotonGameManager.OpponentExhausted)
        {
            myHero.Heal(false);
        }
    }
}
