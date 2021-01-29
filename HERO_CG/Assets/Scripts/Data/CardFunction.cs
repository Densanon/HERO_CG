using UnityEngine;
using System;

public class CardFunction : MonoBehaviour
{ 
    private CardData myCard;

    public static Action<CardData> OnCardSelected = delegate { };

    private void Awake()
    {
        myCard = GetComponent<CardData>();
    }

    public void CardSelected()
    {
        OnCardSelected?.Invoke(myCard);
    }
}
