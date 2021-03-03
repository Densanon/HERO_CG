using UnityEngine;
using System;

public class CardFunction : MonoBehaviour
{ 
    private CardData myCard;

    public static Action<CardData> OnCardSelected = delegate { };
    public static Action<CardData> OnHeroSelected = delegate { };
    public static Action OnCardDeselected = delegate { };
    public static Action<Card> OnCardCollected = delegate { };
    public static Action<Card> OnCardPlayed = delegate { };

    private void Awake()
    {
        myCard = GetComponent<CardData>();
    }

    public void PlayCard()
    {
        OnCardPlayed?.Invoke(myCard.myCard);
    }

    public void CardSelected()
    {
        OnCardSelected?.Invoke(myCard);
    }

    public void HeroSelected()
    {
        OnHeroSelected?.Invoke(myCard);
    }

    public void Deselect()
    {
        OnCardDeselected?.Invoke();
    }

    public void CardCollected()
    {
        OnCardCollected?.Invoke(myCard.myCard);
    }
}
