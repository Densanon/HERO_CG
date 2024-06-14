using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aZoe : Ability
{
    public static bool ZoeHeal = false;
    private void Awake()
    {
        base.Awake();

        myType = Type.Character;
        secondaryType = Type.Activate;
        Name = "ZOE";
        Description = "(A) Before performing an Action, heal any hero(es) of your choice.";
        UIConfirmation.OnActivateTempHealState += () => { ZoeHeal = true; };
    }

    public override void AbilityAwake()
    {
        base.AbilityAwake();
        OnAbilityRequest?.Invoke("Zoe");
    }

    private void OnDestroy()
    {
        UIConfirmation.OnActivateTempHealState -= () => { ZoeHeal = true; };
        base.OnDestroy();
    }
}
