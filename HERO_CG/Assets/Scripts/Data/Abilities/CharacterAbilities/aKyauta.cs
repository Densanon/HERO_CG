using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aKyauta : Ability
{
    private void Awake()
    {
        myType = Type.Character;
        secondaryType = Type.Activate;
        Name = "KYAUTA";
        Description = "(A) After an Action, fatigue one of your heros to recruit one hero to your play area from the Reserves.";
    }
}
