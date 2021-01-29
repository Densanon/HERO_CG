using UnityEngine;

public class Card 
{
    public enum Type { Character, Ability, Enhancement, Feat}
    Type _myType;
    bool _exhausted;
    string _name;
    int _attack;
    int _defense;
    string _flavor;
    string _ability;
    int abilityCounter;
    public Sprite image;

    public Card(Type CardType, string Name, int Attack, int Defense, string Flavor, string Ability, Sprite Image)
    {
        _myType = CardType;
        _name = Name;
        _attack = Attack;
        _defense = Defense;
        _flavor = Flavor;
        _ability = Ability;
        image = Image;
    }

    public Card(Type CardType, string Name, string Ability, Sprite Image)
    {
        _myType = CardType;
        _name = Name;
        _flavor = Flavor;
        _ability = Ability;
        image = Image;
    }

    public Card(Type CardType, string Name, int Attack, int Defense, Sprite Image)
    {
        _myType = CardType;
        _name = Name;
        _attack = Attack;
        _defense = Defense;
        _flavor = Flavor;
        image = Image;
    }

    public bool Exhausted
    {
        get { return _exhausted; }
        private set { _exhausted = value; }
    }

    public Type CardType
    {
        get { return _myType; }
        private set { _myType = value; }
    }

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

    public int AbilityCounter
    {
        get { return abilityCounter; }
        private set { abilityCounter = value; }
    }
}
