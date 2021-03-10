using UnityEngine;

public class Card 
{
    public enum Type { Character, Ability, Enhancement, Feat}
    Type _myType;
    string _name;
    int _attack;
    int _defense;
    string _ability;
    int abilityCounter;
    public Sprite image;
    public Sprite alphaImage;

    /// <summary>
    /// For Creating Hero Cards
    /// </summary>
    /// <param name="CardType"></param>
    /// <param name="Name"></param>
    /// <param name="Attack"></param>
    /// <param name="Defense"></param>
    /// <param name="CardImage"></param>
    /// <param name="AlphaImage"></param>
    public Card(Type CardType, string Name, int Attack, int Defense, Sprite CardImage, Sprite AlphaImage)
    {
        _myType = CardType;
        _name = Name;
        _attack = Attack;
        _defense = Defense;
        image = CardImage;
        alphaImage = AlphaImage;
    }

    /// <summary>
    /// For Creating Enhancement Cards
    /// </summary>
    /// <param name="CardType"></param>
    /// <param name="Name"></param>
    /// <param name="Attack"></param>
    /// <param name="Defense"></param>
    /// <param name="Image"></param>
    public Card(Type CardType, string Name, int Attack, int Defense, Sprite Image)
    {
        _myType = CardType;
        _name = Name;
        _attack = Attack;
        _defense = Defense;
        image = Image;
    }

    /// <summary>
    /// For Creating Ability and Feat Cards
    /// </summary>
    /// <param name="CardType"></param>
    /// <param name="Name"></param>
    /// <param name="Ability"></param>
    /// <param name="Image"></param>
    public Card(Type CardType, string Name, string Ability, Sprite Image)
    {
        _myType = CardType;
        _name = Name;
        _ability = Ability;
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
}
