using UnityEngine;
using System;

public class Ability : MonoBehaviour
{
    public enum Type { Feat, Activate, Passive}
    public Type myType;
    public string Name;
    public string Description;

    public static Action OnAbilityUsed = delegate { };

    public virtual void AbilityAwake()
    {

    }

    public virtual void Target(CardData card)
    {

    }
}
