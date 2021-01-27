using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public enum Type { Character, Ability, Enhancement, Feat}
    [SerializeField] Type _myType;
    [SerializeField] string _name;
    [SerializeField] int _attack;
    [SerializeField] int _defense;
    [SerializeField] string _flavor;
    [SerializeField] string _ability;
    public Image image;

    public Card(Type CardType, string Name, int Attack, int Defense, string Flavor, string Ability, Image Image)
    {
        _myType = CardType;
        _name = Name;
        _attack = Attack;
        _defense = Defense;
        _flavor = Flavor;
        _ability = Ability;
        image = Image;
    }

    public Card(Type CardType, string Name, string Flavor, string Ability, Image Image)
    {
        _myType = CardType;
        _name = Name;
        _flavor = Flavor;
        _ability = Ability;
        image = Image;
    }

    public Card(Type CardType, string Name, int Attack, int Defense, Image Image)
    {
        _myType = CardType;
        _name = Name;
        _attack = Attack;
        _defense = Defense;
        _flavor = Flavor;
        image = Image;
    }

    public Card(Type CardType, string Name, string Flavor, Image Image)
    {
        _myType = CardType;
        _name = Name;
        _flavor = Flavor;
        image = Image;
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

    public void AdjustAttack(int amount)
    {
        Attack += amount;
    }

    public void AdjustDefense(int amount)
    {
        Defense += amount;
    }
}
