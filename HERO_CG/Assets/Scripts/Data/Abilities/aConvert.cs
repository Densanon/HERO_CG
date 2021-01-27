using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aConvert : MonoBehaviour, IAbility
{
    public Ability.Type myType = Ability.Type.Passive;

    #region IAbility
    void IAbility.Activate()
    {
        throw new System.NotImplementedException();
    }

    void IAbility.Remove()
    {
        throw new System.NotImplementedException();
    }

    void IAbility.Target()
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
