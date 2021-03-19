using UnityEngine;
using System;
using UnityEngine.UI;

public class PlayerBase : MonoBehaviour
{
    [SerializeField] private int _health;
    [SerializeField] private bool _exhausted;
    [SerializeField] private Image Icon;
    public enum Type { Player, Opponent}
    public Type type;

    public static Action<PlayerBase> OnBaseDestroyed = delegate { };
    public static Action<PlayerBase> OnExhaust = delegate { };

    public int Health
    {
        get { return _health; }
        private set { _health = value; }
    }

    public bool Exhausted
    {
        get { return _exhausted; }
        private set { _exhausted = value; }
    }

    public void Damage(int amount)
    {
        if(amount >= _health)
        {
            Destroy();
        }else if(amount >= (_health / 2))
        {
            Exhaust(false);
        }
        else
        {
            Debug.Log("Wasn't able to do enough damage.");
        }
    }

    public void Exhaust(bool told)
    {
        Exhausted = true;
        _health = _health / 2;
        if (Icon != null)
        Icon.color = Color.grey;
        if(!told)
        OnExhaust?.Invoke(this);
    }

    public void Destroy()
    {
        OnBaseDestroyed?.Invoke(this);
    }
}
