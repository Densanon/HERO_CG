using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aRevelation : Ability
{
    private void Awake()
    {
        base.Awake();
        myType = Type.Activate;
        Name = "REVELATION";
        Description = "(A) View one opponent’s hand, and discard any one card from their hand.";
    }
    
    public override void AbilityAwake()
    {
        base.AbilityAwake();
        OnAbilityRequest.Invoke("Revelation");
    }
}
