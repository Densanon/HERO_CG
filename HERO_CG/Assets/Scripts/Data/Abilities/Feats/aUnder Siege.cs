﻿using System;

public class aUnderSiege : Ability
{
    //"UNDER SEIGE", "(H) Target opponent reveals their hand, then discards all non-hero cards."

    public static Action OnHandToBeRevealed = delegate { };

    #region Unity Methods
    private void Awake()
    {
        myType = Ability.Type.Feat;
        Name = "UNDER SEIGE";
        Description = "(H) Target opponent reveals their hand, then discards all non-hero cards.";
    }
    #endregion

    public override void AbilityAwake()
    {
        OnHandToBeRevealed?.Invoke();
    }
}
