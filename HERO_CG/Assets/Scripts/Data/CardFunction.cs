using UnityEngine;
using System;

public class CardFunction : MonoBehaviour
{ 
    private CardData myCard;

    public static Action<CardData> OnCardSelected = delegate { };
    public static Action OnCardDeselected = delegate { };
    public static Action<Card> OnCardCollected = delegate { };

    private void Awake()
    {
        myCard = GetComponent<CardData>();
    }

    public void CardSelected()
    {
        OnCardSelected?.Invoke(myCard);
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
