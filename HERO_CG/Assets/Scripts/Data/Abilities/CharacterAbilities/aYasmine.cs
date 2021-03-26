using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aYasmine : Ability
{
    private void Awake()
    {
        myType = Type.Character;
        secondaryType = Type.Passive;
        Name = "YASMINE";
        Description = "(P) When an attack against Yasmine is resolved, you may play a card to the field.";
    }
}
