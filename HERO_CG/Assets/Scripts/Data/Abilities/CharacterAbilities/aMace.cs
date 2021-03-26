using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aMace : Ability
{
    private void Awake()
    {
        myType = Type.Character;
        secondaryType = Type.Passive;
        Name = "MACE";
        Description = "(P) You may double the Total Attack for one attack that occurs on your turn.";
    }
}
