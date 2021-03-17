﻿using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class CardData : MonoBehaviour
{
    public enum FieldPlacement { Mine, Opp, Draft, HQ, Zoom, Hand}
    public FieldPlacement myPlacement;

    [SerializeField] Image Icon;
    [SerializeField] TMP_Text tAttack;
    [SerializeField] TMP_Text tDefense;
    [SerializeField] TMP_Text tbAttack;
    [SerializeField] TMP_Text tbDefense;
    [SerializeField] Image[] gAbilityCounters;
    [SerializeField] Button Target;
    //[SerializeField] Button OvercomeTargetButton;
    [SerializeField] List<Component> myAbilities = new List<Component>();

    public static Action<CardData> IsTarget = delegate { };
    public static Action<CardData, string, int> OnNumericAdjustment = delegate { };
    public static Action<CardData> OnDestroyed = delegate { };

    public bool Exhausted { get; private set; }

    public  Card myCard { get; private set; }

    public Sprite CardImage { get; private set; }

    public Card.Type CardType { get; private set; }

    public string Name { get; private set; }

    public int Attack { get; private set; }

    public int Defense { get; private set; }

    public int AbilityCounter { get; private set; }

    public CardData(Card card, FieldPlacement placement)
    {
        myPlacement = placement;
        Exhausted = false;
        CardType = card.CardType;
        Name = card.Name;
        Attack = card.Attack;
        Defense = card.Defense;
        CardImage = card.image;
    }

    #region Unity Methods
    private void Awake()
    {
        CardDataBase.OnTargeting += HandleTargetting;
        /*PhotonGameManager.OnOvercomeTime += HandleActivateOvercome;
        PhotonGameManager.OnOvercomeSwitch += HandleSwitchOvercome;*/

        UISetup();
    }

    private void OnDestroy()
    {
        CardDataBase.OnTargeting -= HandleTargetting;
        /*PhotonGameManager.OnOvercomeTime -= HandleActivateOvercome;
        PhotonGameManager.OnOvercomeSwitch -= HandleSwitchOvercome;*/
    }
    #endregion

    #region Public Methods
    public void DamageCheck(int dmg)
    {
        if(dmg >= Defense)
        {
            //Destroy
            Debug.Log($"{Name} has been destroyed.");
            OnDestroyed?.Invoke(this);
        }else if (dmg >= Defense/2)
        {
            Exhaust();
            Debug.Log($"{Name} has been exhausted.");
            //Exhaust
        }
        else
        {
            Debug.Log($"{Name} has blocked.");
            //Block
        }
    }

    public void Exhaust()
    {
        Exhausted = true;
    }

    public void Heal()
    {
        Exhausted = false;
    }

    public void SetAttack(int amount)
    {
        Attack = amount;
        tAttack.text = Attack.ToString();
        tbAttack.text = Attack.ToString();
    }

    public void AdjustAttack(int amount)
    {
        bool sendit = (Attack != (Attack + amount));
        Debug.Log($"Adjusting Attack from {Attack} to {Attack + amount}");
        Attack += amount;
        tAttack.text = Attack.ToString();
        tbAttack.text = Attack.ToString();
        if (sendit)
        {
            Debug.Log($"Sending the new Attack Value. {Attack}");
            OnNumericAdjustment?.Invoke(this, "Attack", Attack);
        }
    }

    public void SetDefense(int amount)
    {
        Defense = amount;
        tDefense.text = Defense.ToString();
        tbDefense.text = Defense.ToString();
    }

    public void AdjustDefense(int amount)
    {
        bool sendit = (Defense != (Defense + amount));
        Debug.Log($"Amount to adjust defense {amount}");
        Defense += amount;
        tDefense.text = Defense.ToString();
        tbDefense.text = Defense.ToString();
        if (sendit)
        {
            Debug.Log($"Sending the new Defense Value. {Defense}");
            OnNumericAdjustment?.Invoke(this, "Defense", Defense);
        }
    }

    public void AdjustCounter(int amount, Component ability)
    {
        AbilityCounter += amount;
            for(int i = 0; i < gAbilityCounters.Length-1; i++)
            {
                if(AbilityCounter-1 >= i)
                {
                    gAbilityCounters[i].color = Color.yellow;
                }
                else
                {
                    gAbilityCounters[i].color = Color.clear;
                }
            }
        if (amount > 0)
        {
            myAbilities.Add(ability);
        }
        else
        {
            myAbilities.Remove(ability);
        }

    }

    public void CardOverride(Card card, FieldPlacement placement)
    {
        myPlacement = placement;
        myCard = card;
        CardType = card.CardType;
        Name = card.Name;
        Attack = card.Attack;
        Defense = card.Defense;
        if(myPlacement == FieldPlacement.Mine || myPlacement == FieldPlacement.Opp || myPlacement == FieldPlacement.HQ)
        {
            CardImage = card.alphaImage;
        }
        else
        {
            CardImage = card.image;
        }

        UISetup();
    }

    public void CardOverride(CardData card, FieldPlacement placement)
    {
        myPlacement = placement;
        myCard = card.myCard;
        CardType = card.CardType;
        Name = card.Name;
        Attack = card.Attack;
        Defense = card.Defense;
        if (myPlacement == FieldPlacement.Mine || myPlacement == FieldPlacement.Opp || myPlacement == FieldPlacement.HQ)
        {
            CardImage = card.myCard.alphaImage;
        }
        else
        {
            CardImage = card.myCard.image;
        }

        UISetup();
    }

    public void Targeted()
    {
        IsTarget?.Invoke(this);
    }

    public void OvercomeTarget(bool targeting)
    {
        if (targeting)
        {
            if (myPlacement == FieldPlacement.Mine)
            {
                //Highlight.color = Color.blue;
            }
            else
            {
                //Highlight.color = Color.red;
            }
        }
        else
        {
            //Highlight.color = Color.clear;
        }
    }
    #endregion

    #region Private Methods
    private void UISetup()
    {

        Icon.sprite = CardImage;
        if(tbAttack != null)
        {
            tAttack.text = Attack.ToString();
            tbAttack.text = Attack.ToString();
            tDefense.text = Defense.ToString();
            tbDefense.text = Defense.ToString();
        }else if(myPlacement == FieldPlacement.HQ || myPlacement == FieldPlacement.Mine || myPlacement == FieldPlacement.Opp)
        {
            tAttack.text = Attack.ToString();
            tDefense.text = Defense.ToString();
        }
        else
        {
            tAttack.text = "";
            tDefense.text = "";
        }
    }

    private void HandleTargetting(Card card)
    {
        if(Target != null)
        {
            bool targetting = CardDataBase.bTargeting;
            Target.gameObject.SetActive(targetting);
            //Highlight.color = targetting ? Color.green : Color.clear;
        }
    }

    /*private void HandleActivateOvercome(bool onOff)
    {
        if (onOff)
        {
            switch (myPlacement)
            {
                case FieldPlacement.Mine:
                    if (!Exhausted && OvercomeTargetButton!=null)
                    {
                        Debug.Log("OvercomeTargetButton activate");
                        OvercomeTargetButton.enabled = true;
                    }
                    break;
                case FieldPlacement.Opp:
                    if(OvercomeTargetButton != null)
                    {
                        Debug.Log("OvercomeTargetButton deactivate");
                        OvercomeTargetButton.enabled = false;
                    }
                    break;
            }
        }
        else
        {
            switch (myPlacement)
            {
                case FieldPlacement.Mine:
                    OvercomeTargetButton.enabled = false;
                    break;
                case FieldPlacement.Opp:
                    OvercomeTargetButton.enabled = false;
                    break;
            }
        }
        Highlight.color = Color.clear;
    }

    private void HandleSwitchOvercome()
    {
        if (PhotonGameManager.AttDef)
        {
            switch (myPlacement)
            {
                case FieldPlacement.Mine:
                    if (!Exhausted)
                    {
                        OvercomeTargetButton.interactable = true;
                    }
                    break;
                case FieldPlacement.Opp:
                    OvercomeTargetButton.interactable = false;
                    break;
            }
        }
        else
        {
            switch (myPlacement)
            {
                case FieldPlacement.Mine:
                    OvercomeTargetButton.interactable = false;
                    break;
                case FieldPlacement.Opp:
                    OvercomeTargetButton.interactable = true;
                    break;
            }
        }
    }
    */
    #endregion
}
