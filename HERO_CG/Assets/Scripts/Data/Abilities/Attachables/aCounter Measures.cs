using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aCounterMeasures : Ability
{
    private void Awake()
    {
        myType = Type.Passive;
        Name = "COUNTER MEASURE";
        Description = "(P) When attacked, this hero may combine its own Total Defense with the defense of the attacking hero(es), excluding their enhancements.";
    }
}
