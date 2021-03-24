using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aUnderSiege : Ability
{
    //"UNDER SEIGE", "(H) Target opponent reveals their hand, then discards all non-hero cards."

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
