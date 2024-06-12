using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aConvert : Ability
{
    public static bool convert = false;

    private void Awake()
    {
        base.Awake();
        myType = Type.Passive;
        Name = "CONVERT";
        Description = "(P) When this hero is defeated, you may take control of any hero, moving it to your play area, discarding all of its enhancements. If fatigued, hero must stay that way.";
    }

    public override void PassiveCheck(PassiveType passiveType)
    {
        if(passiveType == PassiveType.CharacterDestroyed && myHero == Referee.PreviousDefender && !Referee.myTurn)
        {
            base.PassiveCheck(passiveType);
            convert = true;
            OnRequestTargeting?.Invoke(Referee.TargetType.OppHero);
        }
    }
}
