﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aReduction : Ability
{
    private void Awake()
    {
        myType = Type.Activate;
        Name = "REDUCTION";
        Description = "(A) Fatigue one of your strengthened heroes to cause any player to discard 2 random cards from their hand.";
    }
}
