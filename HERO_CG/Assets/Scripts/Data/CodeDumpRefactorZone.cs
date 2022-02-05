
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

    public void SilenceAbilityToField(int turns)
    {
        abilityPlaySilenceTurnTimer = turns;
        canPlayAbilityToField = false;
    }
    #endregion

    #region Overcome Methods
    public void CalculateBattle()
    {
        bAwaitingResponse = true;
        StartCoroutine(WaitResponse(GamePhase.Overcome));
    }

    private void ActualCalculateBattle()
    {
        ///////////////////////////////////////////////////////////You were trying to set up a response system, we needed to share the characters that are getting attacked and by whom
        if (AttackingHeros.Count > 0 && DefendingHero != null)
        {
            PreviousAttackers.Clear();
            PreviousDefender = null;
            foreach (CardData card in AttackingHeros)
            {
                PreviousAttackers.Add(card);
            }
            PreviousDefender = DefendingHero;

            int tDmg = 0;
            foreach (CardData data in AttackingHeros)
            {
                Debug.Log($"{data.Name} was an attacking hero");
                tDmg += data.Attack;
                data.Exhaust(false);
            }

            DefendingHero.DamageCheck(tDmg);
            if (DefendingHero != null)
            {
                OpponentExhausted = DefendingHero.Exhausted;
                Debug.Log($"Opponent exhaust status = {OpponentExhausted}");
            }
            else
            {
                OpponentExhausted = true;
                Debug.Log($"Opponent should have been destroyed so exhaust has been set to true");
            }

            PassiveActivate(Ability.PassiveType.BattleComplete);
            CB.SendPreviousAttackersAndDefender(AttackingHeros, DefendingHero);
            AttackingHeros.Clear();
            DefendingHero = null;
            SwitchAttDef();
        }
    }

    public void SwitchAttDef()
    {
        AttDef = !AttDef;
        if (AttDef)
        {
            OnPassiveActivate?.Invoke(Ability.PassiveType.BattleStart);
        }
        if (!AttDef && !CB.CheckFieldForOpponents() && AttackingHeros.Count > 0)
        {
            //Target base
            int tDmg = 0;
            foreach (CardData data in AttackingHeros)
            {
                tDmg += data.Attack;
                data.Exhaust(false);
            }
            AttackingHeros.Clear();

            PB.Damage(tDmg);

            SwitchAttDef();
        }
        else if (AttDef && !CB.CheckMyFieldForUsableHeros())
        {
            //check if all characters are exhausted, if they are, end turn
            StartCoroutine(EndturnDelay());
            PassiveActivate(Ability.PassiveType.ActionComplete);
            return;
        }
        OnOvercomeSwitch?.Invoke();
    }
    #endregion

    #region Turn Methods

    #region Move Counter Methods
    public int GetTurnCounter()
    {
        return iTurnCounter;
    }

    public void TurnCounterDecrement()
    {
        iTurnCounter--;
        PopUpUpdater($"{iTurnCounter} moves left.");
        if (iTurnCounter > 0)
        {
            if (myPhase == GamePhase.Recruit)
            {
                tCardsToCollectReserve.text = $"{iTurnCounter}/{CB.CardsRemaining(CardDataBase.CardDecks.Reserve)}";
            }
            if (myPhase == GamePhase.Enhance)
            {
                tCardsToDrawMyDeck.text = $"{iEnhanceCardsToCollect}/{CB.CardsRemaining(CardDataBase.CardDecks.P1Deck)}";
            }
            ///if(myPhase == GamePhase.Recruit)
            {
                CB.FillHQ();
            }
            PassiveActivate(Ability.PassiveType.ActionComplete);
                        StartCoroutine(EndturnDelay());
            HandleTurnDeclaration(!myTurn);///
        }
        else
        {
            tCardsToCollectReserve.text = $"{CB.CardsRemaining(CardDataBase.CardDecks.Reserve)}";
            btEndTurn.gameObject.SetActive(true);
            PopUpUpdater("No More Actions");
        }
    }

    

    public void SetTurnGauge(int newNum)
    {
        iTurnGauge = newNum;
    }
    #endregion

    public void PopUpUpdater(string message)
    {
        StartCoroutine(PhaseDeclaration(message));
    }

    

    



    

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

private void HandleHeroSelected(CardData card)
{
    if (CB.CheckIfMyCard(card))
    {
        if (!card.Exhausted)
        {
            if (AttackingHeros.Contains(card))
            {
                Debug.Log($"Removing {card.Name} from Attacking.");
                //Untarget Card
                AttackingHeros.Remove(card);
                card.OvercomeTarget(false);
            }
            else
            {
                Debug.Log($"Adding {card.Name} to Attacking.");
                //Target Card
                AttackingHeros.Add(card);
                card.OvercomeTarget(true);
            }
        }
    }
    else
    {
        if (!AttDef)
        {
            if (DefendingHero == card)
            {
                Debug.Log($"Removing {card.Name} from Defending.");
                //Untarget Card
                DefendingHero = null;
                card.OvercomeTarget(false);
            }
            else
            {
                Debug.Log($"Adding {card.Name} to Defending.");
                //Target Card
                if (DefendingHero != null)
                {
                    DefendingHero.OvercomeTarget(false);
                }
                DefendingHero = card;
                //turn on interactible for calculate
                card.OvercomeTarget(true);
            }
        }
    }
}


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