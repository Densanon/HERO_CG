using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aProtect : Ability
{
    private void Awake()
    {
        myType = Type.Passive;
        Name = "PROTECT";
        Description = "(P) When another hero is attacked, you may prevent that hero from fatigue and defeat until the end of your next turn.";
    }
}
