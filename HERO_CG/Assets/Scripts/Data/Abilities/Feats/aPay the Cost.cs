using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aPaytheCost : Ability
{
    //"PAY THE COST", "(H) Fatigue one hero in your play area, to remove one hero from the field."

    #region Unity Methods
    private void Awake()
    {
        myType = Ability.Type.Feat;

    }

    private void OnDestroy()
    {

    }
    #endregion
}
