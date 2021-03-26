using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aIgnacia : Ability
{
    private void Awake()
    {
        myType = Type.Character;
        secondaryType = Type.Activate;
        Name = "IGNACIA";
        Description = "(A) Ignacia may attack once per turn while fatigued (this cannot be combined with any other attack).";
    }
}
