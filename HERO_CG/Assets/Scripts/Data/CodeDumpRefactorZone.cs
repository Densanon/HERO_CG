
    /*#region Base Methods
    public void OnBaseDestroyed(PlayerBase.Type pBase)
    {
        if (pBase == PlayerBase.Type.Player)
        {
            EndUI.SetActive(true);
            EndText.text = "You were Overcome!";
        }
        else
        {
            EndUI.SetActive(true);
            EndText.text = "You have Overcome!";
        }
    }

    public void OnBaseExhausted(PlayerBase.Type pBase)
    {
        if (pBase == PlayerBase.Type.Player)
        {
            PB.Exhaust(true);
        }
        else
        {
            MyPB.Exhaust(true);
        }
    }
    #endregion

    #region Ability Methods
    public void PassiveActivate(Ability.PassiveType passiveType)
    {
        if (myPhase == GamePhase.HeroDraft || myPhase == GamePhase.AbilityDraft)
        {
            return;
        }
        OnPassiveActivate?.Invoke(passiveType);
    }

    public bool canPlayAbilitiesToFieldCheck()
    {
        return canPlayAbilityToField;
    }

    public void SetActiveAbility(Ability ability)
    {
        Debug.Log($"Active ability to be set: {ability.Name}");
        activeAbility = ability;
        StartCoroutine(PhaseDeclaration($"{ability.Name} ability Activated"));
        ability.AbilityAwake();
    }

    
    #endregion








    #region Overcome Methods
    

    





    #endregion

    #region Turn Methods

    #region Move Counter Methods
    

    
    }

    

    public void SetTurnGauge(int newNum)
    {
        iTurnGauge = newNum;
    }
    #endregion

   

    

    



    

    public void HandleHoldTurn(bool hold, bool myTurn)
    {
        if (myPhase == GamePhase.AbilityDraft || myPhase == GamePhase.HeroDraft)
            return;

        if (!myTurn)
            PhaseChange(GamePhase.Wait);

        bEndTurn = !hold;
    }

    private void HandleHoldTurnOff() => myManager.RPCRequest("HandleTurnOffTurnHold", RpcTarget.All, true);

    #region PlayerResponse
    public void ResponseTimer()
    {
        OnWaitTimer?.Invoke();
    }

    private void NeedAResponseFromOpponent()
    {
        myManager.RPCRequest("NeedResponse", RpcTarget.Others, true);
    }
    #endregion

    

    #endregion

    #region Card Methods
    public void DrawCardOption(int amount)
    {
        int i = CB.CardsRemaining(CardDataBase.CardDecks.P1Deck);
        if (i < amount)
        {
            amount = i;
        }
    }

    

    

    public void HandCardZoom()
    {
        handZoomed = true;
        gCardZoom.SetActive(true);
        gCard.CardOverride(CB.CurrentActiveCard, CardData.FieldPlacement.Zoom);
        HandleCardButtons(CardData.FieldPlacement.Hand);
        if (CB.CurrentActiveCard.CardType == Card.Type.Feat && myPhase != GamePhase.Feat)
        {
            NullZoomButtons();
        }
        ClearAbilityPanel();
    }

    public void CheckHandZoomInEffect()
    {
        gCard.CardOverride(CB.CurrentActiveCard, CardData.FieldPlacement.Hand);
        HandleCardButtons(CardData.FieldPlacement.Hand);
        if (CB.CurrentActiveCard.CardType == Card.Type.Feat && myPhase != GamePhase.Feat)
        {
            NullZoomButtons();
        }
        ClearAbilityPanel();
    }

    

    


#endregion




#region Ability Functions + Feat
private void HandleAbilityTargetting(CardData card)
    {
        if (activeAbility != null && card.CardType == Card.Type.Character)
        {
            Debug.Log("HandleAbilityTargetting: Activating a targeting ability.");
            activeAbility.Target(card);
            abilityTargetting = true;
        }
    }

    private void HandleActivateAbilitySetup(Ability ability)
    {
        if (myPhase == GamePhase.AbilityDraft || myPhase == GamePhase.HeroDraft)
        {
            return;
        }
        activeAbility = ability;
        StartCoroutine(PhaseDeclaration("Activate Ability?"));
        ability.AbilityAwake();
    }

    private void HandleAbilityEnd()
    {
        Debug.Log("Ability has ended.");
        if (activeAbility.myType == Ability.Type.Feat)
        {
            PassiveActivate(Ability.PassiveType.ActionComplete);
            StartCoroutine(EndturnDelay());
        }
        activeAbility = null;
        abilityTargetting = false;
    }

    

    

    private void HandleFeatComplete()
    {
        PassiveActivate(Ability.PassiveType.ActionComplete);
        StartCoroutine(EndturnDelay());
    }
    #endregion

    

    #region IEnumerators
   
    private IEnumerator EndturnDelay()
    {
        yield return new WaitForSeconds(1f);
        if (bEndTurn)
        {
            SwitchTurn();
        }
        else
        {
            StartCoroutine(EndturnDelay());
        }
    }

    private IEnumerator WaitResponse(GamePhase phase)
    {
        NeedAResponseFromOpponent();
        yield return new WaitForSeconds(1f);
        if (!bAwaitingResponse)
        {
            StartCoroutine(WaitResponse(phase));
        }
        else
        {
            switch (phase)
            {
                case GamePhase.Overcome:
                    ActualCalculateBattle();
                    break;
            }
        }
}
#endregion*/