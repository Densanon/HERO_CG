using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aKyauta : Ability
{
    protected override void Awake()
    {
        base.Awake();
    
        myType = Type.Character;
        secondaryType = Type.Activate;
        Name = "KYAUTA";
        Description = "(A) After an Action, fatigue one of your heros to recruit one hero to your play area from the Reserves.";
    }


    public override void AbilityAwake()
    {
        base.AbilityAwake();

        if (canActivate)
        {
            OnSetActive?.Invoke(this);
        }
    }

    public override void PassiveCheck(PassiveType passiveType)
    {
        base.PassiveCheck(passiveType);

        if(passiveType == PassiveType.ActionComplete)
        {
            //need to have heros to fatigue
            //recruit a hero to play area
                //target hero
                //don't draw but play to field
        }
    }
}
