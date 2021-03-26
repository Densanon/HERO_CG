using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aReinforcement : Ability
{
    private void Awake()
    {
        myType = Type.Passive;
        Name = "REINFORCEMENT";
        Description = "(P) For every card in your hand, this hero may gain +10 attack.";
    }
}
