using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aRevelation : Ability
{
    private void Awake()
    {
        myType = Type.Activate;
        Name = "REVELATION";
        Description = "(A) View one opponent’s hand, and discard any one card from their hand.";
    }
}
