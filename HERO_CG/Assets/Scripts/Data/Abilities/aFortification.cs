using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aFortification : MonoBehaviour,IAbility
{
    public Ability.Type myType = Ability.Type.Passive;

    #region IAbility
    void IAbility.Activate()
    {

    }

    void IAbility.Remove()
    {

    }

    void IAbility.Target()
    {

    }
    #endregion
}
