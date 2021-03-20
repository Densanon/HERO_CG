using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aDrain : Ability
{
    //"DRAIN", "(H) Discard all of one opponenet's Enhancement Cards from the field."

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
