using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aRohan : Ability
{
    private void Awake()
    {
        myType = Type.Character;
        secondaryType = Type.Activate;
        Name = "ROHAN";
        Description = "(A) For every 2 fatigued heroes, recruit one hero from Hero HQ or Reserves to your hand.";
    }
}
