using UnityEngine;

public class Ability : MonoBehaviour
{
    public enum Type { Feat, Activate, Passive}
    public Type myType;

    public virtual void Target()
    {

    }
}
