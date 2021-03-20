using UnityEngine;
using System.Collections.Generic;

public class aAbsorb : Ability
{
    //"ABSORB", "(H) Discard the Enhancements, if any, from any one hero. Then, replace with those from another hero."

    CardData Target1;
    CardData Target2;

    #region Unity Methods
    private void Awake()
    {
        myType = Ability.Type.Feat;
        
    }

    private void OnDestroy()
    {
        
    }
    #endregion

    public override void Target()
    {
        
    }
}
