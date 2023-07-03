//Created by Jordan Ezell
//Last Editted: 6/30/23 Jordan

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

        if(type == PassiveType.HeroFatigued && Referee.OpponentExhausted && Referee.AttackingHeros.Contains(myHero))
        {
            myHero.Heal(false);
        }
    }
}
