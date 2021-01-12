using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[CreateAssetMenu(menuName = "HERO/Data/Card", fileName = "card")]
public class Card : ScriptableObject
{
    [SerializeField] string _name;
    [SerializeField] int _attack;
    [SerializeField] int _defense;
    [SerializeField] string _flavor;
    [SerializeField] string _ability;
    public Image image;

    public string Name
    {
        get { return _name; }
        private set { _name = value; }
    }

    public int Attack
    {
        get { return _attack; }
        private set { _attack = value;  }
    }

    public int Defense
    {
        get { return _defense; }
        private set { _defense = value; }
    }

    public string Flavor
    {
        get { return _flavor; }
        private set { _flavor = value; }
    }

    public string Ability
    {
        get { return _ability; }
        private set { _ability = value; }
    }
}
