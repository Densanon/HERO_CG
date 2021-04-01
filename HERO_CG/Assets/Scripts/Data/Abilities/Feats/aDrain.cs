using System;
using UnityEngine;

public class aDrain : Ability
{
    //"DRAIN", "(H) Discard all of one opponenet's Enhancement Cards from the field."
    public static Action<string> OnStripAllEnhancementsFromSideOfField = delegate { };

    #region Unity Methods
    private void Awake()
    {
        myType = Ability.Type.Feat;
        Name = "DRAIN";
        Description = "(H) Discard all of one opponenet's Enhancement Cards from the field.";
    }
    #endregion

    public override void AbilityAwake()
    {
        base.AbilityAwake();
        OnStripAllEnhancementsFromSideOfField?.Invoke("P2Field");
        Debug.Log("Stripped all enhancements and abilities from the opponent.");
    }
}
