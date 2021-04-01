using UnityEngine;
using System;

public class Ability : MonoBehaviour
{
    public enum Type { Feat, Activate, Passive, Character}
    public Type myType;
    public Type secondaryType;
    public string Name;
    public string Description;

    public static Action OnAbilityUsed = delegate { };
    public static Action OnFeatComplete = delegate { };

    public virtual void AbilityAwake()
    {
        Debug.Log("AbilityAwake Activated");
    }

    public virtual void Target(CardData card)
    {
        Debug.Log("AbilityTarget Activated");
    }
}
