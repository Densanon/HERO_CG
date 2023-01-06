//Created by Jordan Ezell
//Last Editted: 1/5/23 Jordan

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aAkio : Ability
{
    protected override void Awake()
    {
        base.Awake();

        myType = Type.Character;
        secondaryType = Type.Passive;
        Name = "AKIO";
        Description = "(P) When Akio causes another hero to be fatigued, Akio is not fatigued.";
    }

    public override void PassiveCheck(PassiveType type)
    {
        base.PassiveCheck(type);

        if(type == PassiveType.BattleComplete && Referee.OpponentExhausted)
        {
            myHero.Heal(false);
        }
    }
}
