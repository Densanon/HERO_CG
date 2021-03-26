using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aDrought : Ability
{
    private void Awake()
    {
        myType = Type.Passive;
        Name = "DROUGHT";
        Description = "(P) When a Heroic Ability is used, you may discard this card to nullify its effects. The affected player may still take an action but cannot use ANY Abilities this turn.";
    }
}
