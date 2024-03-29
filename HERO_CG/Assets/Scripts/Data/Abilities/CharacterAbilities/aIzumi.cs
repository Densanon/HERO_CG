using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aIzumi : Ability
{
    public static bool IzumiDefBoost = false;

    protected override void Awake()
    {
        base.Awake();

        myType = Type.Character;
        secondaryType = Type.Passive;
        Name = "IZUMI";
        Description = "(P) Allied heroes gain +20 defense while Izumi is in play. Fatigued heroes gain this after their defense is halved.";

        if (myHero.myPlacement == CardData.FieldPlacement.Mine) { IzumiDefBoost = true; OnToggleIzumi?.Invoke(); }
    }

    protected override void OnDestroy()
    {
        if (myHero.myPlacement == CardData.FieldPlacement.Mine) { IzumiDefBoost = false; OnToggleIzumi?.Invoke(); }
        base.OnDestroy();
    }

    public override void PassiveCheck(PassiveType passiveType)
    {
        base.PassiveCheck(passiveType);

        if (myHero.myPlacement == CardData.FieldPlacement.Mine)
        {
            //OnToggleIzumiConfirmationRequest?.Invoke("Izumi");
        }
    }
}
