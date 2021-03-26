using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aConvert : Ability
{
    private void Awake()
    {
        myType = Type.Passive;
        Name = "CONVERT";
        Description = "(P) When this hero is defeated, you may take control of any hero, moving it to your play area, discarding all of its enhancements. If fatigued, hero must stay that way.";
    }
}
