using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aPrevention : Ability
{
    private void Awake()
    {
        myType = Type.Passive;
        Name = "PREVENTION";
        Description = "(P) If this hero is attacked, you may discard this Ability to block the attack and all other attacks against your heroes until the start of your next turn.";
    }
}
