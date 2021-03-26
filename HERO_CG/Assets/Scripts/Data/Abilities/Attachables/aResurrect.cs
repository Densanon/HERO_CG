using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aResurrect : Ability
{
    private void Awake()
    {
        myType = Type.Passive;
        Name = "RESURRECT";
        Description = "(P) When this hero is defeated, you may return this hero to play, under your control in a strengthened position, with no enhancements.";
    }
}
