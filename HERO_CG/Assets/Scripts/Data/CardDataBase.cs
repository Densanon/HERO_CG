using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using System;
using System.Collections;

public class CardDataBase : MonoBehaviour
{
    PhotonView PV;

    public enum CardDecks { P1Hand, P1Deck, P1Field, P1Discard, P2Hand, P2Deck, P2Field, P2Discard, Reserve, HQ}

    public GameObject CardHandPrefab;
    public GameObject CardDraftPrefab;
    public GameObject CardMyFieldPrefab;
    public GameObject CardOppFieldPrefab;
    public GameObject CardHeroHQPrefab;
    public Button ReserveButton;
    public GameObject[] Hand = new GameObject[7];
    private List<CardData> lHandData = new List<CardData>();
    public Slider sHandSlider;
    public List<CardData> Draft = new List<CardData>();
    public Transform DraftArea;
    public Transform MyHeroArea;
    public Transform OppHeroArea;
    public Transform HQArea;
    public List<CardData> HQHeros = new List<CardData>();
    public PhotonGameManager GM;
    public CardData CurrentActiveCard;
    public static bool bTargeting = false;
    public static int handSize = 0;
    public static int herosFatigued = 0;

    #region Debuging
    public bool AiDraft = false;
    public bool AutoDraft = false;
    public bool SpecificDraw = false;
    #endregion

    public static Action<bool> OnTurnDelcarationReceived = delegate { };
    public static Action<Card> OnAiDraftCollected = delegate { };
    public static Action<Card, bool> OnTargeting = delegate { };

    #region Card Data Base
    [SerializeField] private Sprite[] AlphaHeros = new Sprite[20];
    [SerializeField] private Sprite[] HeroImages = new Sprite[20];
    [SerializeField] private Sprite[] AbilityImages = new Sprite[20];
    [SerializeField] private Sprite[] EnhanceImages = new Sprite[4];
    [SerializeField] private Sprite[] FeatImages = new Sprite[4];
    [SerializeField] private Sprite CardBackImage;

    Card[] Heros = new Card[20];
    Card[] Abilities = new Card[20];
    Card[] Enhancements = new Card[4];
    Card[] Feats = new Card[4];

    Card[] CardBase = new Card[48];
    #endregion

    #region Dynamic card lists
    List<Card> AbilityDraft = new List<Card>();
    List<Card> HeroReserve = new List<Card>();
    List<Card> HQ = new List<Card>();

    List<Card> P1AutoAbilities = new List<Card>();
    List<Card> P2AutoAbilities = new List<Card>();
    List<Card> P1Hand = new List<Card>();
    List<CardData> P1Field = new List<CardData>();
    List<Card> P1Deck = new List<Card>();
    List<Card> P1Discard = new List<Card>();

    List<Card> P2Hand = new List<Card>();
    List<CardData> P2Field = new List<CardData>();
    List<Card> P2Deck = new List<Card>();
    List<Card> P2Discard = new List<Card>();

    List<Card> cardListToDisplay = new List<Card>();
    Component activeFeat;

    List<Ability> heroAbilitiesOnField = new List<Ability>();
    List<Ability> cardAbilitiesOnField = new List<Ability>();
    #endregion

    #region Unity Methods
    private void Awake()
    {
        PV = GetComponent<PhotonView>();

        UIConfirmation.OnTargetAccepted += HandleTargetAccepted;
        CardData.OnNumericAdjustment += HandleCardAdjustment;
        CardData.OnExhausted += HandleCardExhaustState;
        CardData.OnDestroyed += HandleCharacterDestroyed;
        CardData.OnAbilitiesStripped += HandleAbilityStripped;
        CardData.OnEnhancementsStripped += HandleEnhancementsStripped;
        CardData.OnGivenAbilities += HandleAbilitiesGiven;
        CardData.OnGivenEnhancements += HandleEnhancementsGiven;
        PlayerBase.OnBaseDestroyed += HandleBaseDestroyed;
        PlayerBase.OnExhaust += HandleBaseExhaust;
        aDrain.OnStripAllEnhancementsFromSideOfField += StripAllEnhancementsOnSideOfField;
        aUnderSiege.OnHandToBeRevealed += HandleRevealTargetHandAndRemoveNonHeros;
        Ability.OnAddAbilityToMasterList += HandleAddAbilityToList;
        Ability.OnDiscardCard += HandleCardForceDiscard;

        Heros[0] = new Card(Card.Type.Character, "AKIO", 20, 70, HeroImages[0], AlphaHeros[0]);
        Heros[1] = new Card(Card.Type.Character, "AYUMI", 40, 50, HeroImages[1], AlphaHeros[1]);
        Heros[2] = new Card(Card.Type.Character, "BOULOS",90, 0, HeroImages[2], AlphaHeros[2]);
        Heros[3] = new Card(Card.Type.Character, "CHRISTOPH", 30, 60, HeroImages[3], AlphaHeros[3]);
        Heros[4] = new Card(Card.Type.Character, "ENG", 10, 80, HeroImages[4], AlphaHeros[4]);
        Heros[5] = new Card(Card.Type.Character, "GAMBITO", 50, 40, HeroImages[5], AlphaHeros[5]);
        Heros[6] = new Card(Card.Type.Character, "GRIT", 0 , 90, HeroImages[6], AlphaHeros[6]);
        Heros[7] = new Card(Card.Type.Character, "HINDRA",70, 20, HeroImages[7], AlphaHeros[7]);
        Heros[8] = new Card(Card.Type.Character, "IGNACIA", 20, 70, HeroImages[8], AlphaHeros[8]);
        Heros[9] = new Card(Card.Type.Character, "ISAAC", 40, 50, HeroImages[9], AlphaHeros[9]);
        Heros[10] = new Card(Card.Type.Character, "IZUMI", 10, 80, HeroImages[10], AlphaHeros[10]);
        Heros[11] = new Card(Card.Type.Character, "KAY", 50, 40, HeroImages[11], AlphaHeros[11]);
        Heros[12] = new Card(Card.Type.Character, "KYAUTA", 90, 0, HeroImages[12], AlphaHeros[12]);
        Heros[13] = new Card(Card.Type.Character, "MACE", 0, 90, HeroImages[13], AlphaHeros[13]);
        Heros[14] = new Card(Card.Type.Character, "MICHAEL", 80, 10, HeroImages[14], AlphaHeros[14]);
        Heros[15] = new Card(Card.Type.Character, "ORIGIN", 80, 10, HeroImages[15], AlphaHeros[15]);
        Heros[16] = new Card(Card.Type.Character, "ROHAN", 70,20, HeroImages[16], AlphaHeros[16]);
        Heros[17] = new Card(Card.Type.Character, "YASMINE", 60, 30, HeroImages[17], AlphaHeros[17]);
        Heros[18] = new Card(Card.Type.Character, "ZHAO", 70, 20, HeroImages[18], AlphaHeros[18]);
        Heros[19] = new Card(Card.Type.Character, "ZOE", 50, 40, HeroImages[19], AlphaHeros[19]);

        Abilities[0] = new Card(Card.Type.Ability, "ACCELERATE", "(A) Target player draws 3 cards from their Enhancement Deck then discards 2 from their hand.", AbilityImages[0]);
        Abilities[1] = new Card(Card.Type.Ability, "BACKFIRE", "(A) Target opponent reveals all of their Abilities on the field; you may use one of the Active Abilities as if it was your own, even if it is one that is found on a hero’s card, regardless of timing.", AbilityImages[1]);
        Abilities[2] = new Card(Card.Type.Ability, "BOLSTER", "(P) For every strengthened hero on the field, this hero may gain +10 attack. It does not matter who the heroes belong to.", AbilityImages[2]);
        Abilities[3] = new Card(Card.Type.Ability, "BOOST", "(A) Discard 2 cards from your hand to choose any one player who must draw one card for every hero (regardless of who the heroes belong to) on the field.", AbilityImages[3]);
        Abilities[4] = new Card(Card.Type.Ability, "COLLATERAL DAMAGE", "(P) If this hero is used to defeat a hero, you may determine one other strengthened hero to be fatigued.", AbilityImages[4]);
        Abilities[5] = new Card(Card.Type.Ability, "CONVERT", "(P) When this hero is defeated, you may take control of any hero, moving it to your play area, discarding all of its enhancements. If fatigued, hero must stay that way.", AbilityImages[5]);
        Abilities[6] = new Card(Card.Type.Ability, "COUNTER-MEASURES", "(P) When attacked, this hero may combine its own Total Defense with the defense of the attacking hero(es), excluding their enhancements.", AbilityImages[6]);
        Abilities[7] = new Card(Card.Type.Ability, "DROUGHT", "(P) When a Heroic Ability is used, you may discard this card to nullify its effects. The affected player may still take an action but cannot use ANY Abilities this turn.", AbilityImages[7]);
        Abilities[8] = new Card(Card.Type.Ability, "FORTIFICATION", "(P) For every hero on the field, this hero may gain +10 defense.", AbilityImages[8]);
        Abilities[9] = new Card(Card.Type.Ability, "GOING NUCLEAR", "(P) When this hero is attacked, all cards on the field, including this one must be removed, except for SkyBases.", AbilityImages[9]);
        Abilities[10] = new Card(Card.Type.Ability, "HARDENED", "(P) For every opposing card on the field, this hero may gain +10 defense.", AbilityImages[10]);
        Abilities[11] = new Card(Card.Type.Ability, "IMPEDE", "(P) You may prevent the effects of one Active Ability per opponent turn.", AbilityImages[11]);
        Abilities[12] = new Card(Card.Type.Ability, "KAIROS", "(A) Discard this hero to recruit up to 2 heroes from the top of the Reserves straight to your play area.", AbilityImages[12]);
        Abilities[13] = new Card(Card.Type.Ability, "PREVENTION", "(P) If this hero is attacked, you may discard this Ability to block the attack and all other attacks against your heroes until the start of your next turn.", AbilityImages[13]);
        Abilities[14] = new Card(Card.Type.Ability, "PROTECT", "(P) When another hero is attacked, you may prevent that hero from fatigue and defeat until the end of your next turn.", AbilityImages[14]);
        Abilities[15] = new Card(Card.Type.Ability, "REDUCTION", "(A) Fatigue one of your strengthened heroes to cause any player to discard 2 random cards from their hand.", AbilityImages[15]);
        Abilities[16] = new Card(Card.Type.Ability, "REINFORCEMENT", "(P) For every card in your hand, this hero may gain +10 attack.", AbilityImages[16]);
        Abilities[17] = new Card(Card.Type.Ability, "RESURRECT", "(P) When this hero is defeated, you may return this hero to play, under your control in a strengthened position, with no enhancements.", AbilityImages[17]);
        Abilities[18] = new Card(Card.Type.Ability, "REVELATION", "(A) View one opponent’s hand, and discard any one card from their hand.", AbilityImages[18]);
        Abilities[19] = new Card(Card.Type.Ability, "SHEILDING", "(P) If fatigued, this hero may gain +20 defense for every strengthened hero. This bonus is added after the Total Defense is halved.", AbilityImages[19]);

        Enhancements[0] = new Card(Card.Type.Enhancement, "ATTACK 20", 20, 0, EnhanceImages[0]);
        for(int i = 0; i<4; i++)
        {
            P1Deck.Add(Enhancements[0]);
        }
        Enhancements[1] = new Card(Card.Type.Enhancement, "ATTACK 30", 30, 0, EnhanceImages[1]);
        for (int i = 0; i < 3; i++)
        {
            P1Deck.Add(Enhancements[1]);
        }
        Enhancements[2] = new Card(Card.Type.Enhancement, "DEFENSE 30", 0, 30, EnhanceImages[2]);
        for (int i = 0; i < 2; i++)
        {
            P1Deck.Add(Enhancements[2]);
        }
        Enhancements[3] = new Card(Card.Type.Enhancement, "DEFENSE 20", 0, 20, EnhanceImages[3]);
        for (int i = 0; i < 3; i++)
        {
            P1Deck.Add(Enhancements[3]);
        }


        Feats[0] = new Card(Card.Type.Feat, "ABSORB", "(H) Discard the Enhancements, if any, from any one hero. Then, replace with those from another hero.", FeatImages[0]);
        Feats[1] = new Card(Card.Type.Feat, "DRAIN", "(H) Discard all of one opponenet's Enhancement Cards from the field.", FeatImages[1]);
        Feats[2] = new Card(Card.Type.Feat, "PAY THE COST", "(H) Fatigue one hero in your play area, to remove one hero from the field.", FeatImages[2]);
        Feats[3] = new Card(Card.Type.Feat, "UNDER SEIGE", "(H) Target opponent reveals their hand, then discards all non-hero cards.", FeatImages[3]);

        foreach(Card item in Abilities)
        {
            AbilityDraft.Add(item);
        }
        foreach(Card item in Feats)
        {
            AbilityDraft.Add(item);
        }

        P1AutoAbilities.Add(Feats[0]);
        P1AutoAbilities.Add(Abilities[1]);
        P1AutoAbilities.Add(Abilities[6]);
        P1AutoAbilities.Add(Abilities[0]);
        P1AutoAbilities.Add(Abilities[8]);
        P1AutoAbilities.Add(Abilities[2]);
        P1AutoAbilities.Add(Feats[3]);
        P1AutoAbilities.Add(Abilities[3]);
        P1AutoAbilities.Add(Abilities[7]);
        P1AutoAbilities.Add(Abilities[14]);
        P1AutoAbilities.Add(Abilities[13]);
        P1AutoAbilities.Add(Abilities[4]);

        P2AutoAbilities.Add(Feats[1]);
        P2AutoAbilities.Add(Abilities[18]);
        P2AutoAbilities.Add(Abilities[11]);
        P2AutoAbilities.Add(Abilities[15]);
        P2AutoAbilities.Add(Abilities[10]);
        P2AutoAbilities.Add(Abilities[16]);
        P2AutoAbilities.Add(Feats[2]);
        P2AutoAbilities.Add(Abilities[9]);
        P2AutoAbilities.Add(Abilities[17]);
        P2AutoAbilities.Add(Abilities[5]);
        P2AutoAbilities.Add(Abilities[12]);
        P2AutoAbilities.Add(Abilities[19]);

        for (int i = 0; i < 48; i++)
        {
            if(i < 20)
            {
                CardBase[i] = Heros[i];
            }else if(i < 40)
            {
                CardBase[i] = Abilities[i - 20];
            }else if(i < 44)
            {
                CardBase[i] = Enhancements[i - 40];
            }else if(i < 48)
            {
                CardBase[i] = Feats[i - 44];
            }
        }
    }

    private void OnDestroy()
    {
        UIConfirmation.OnTargetAccepted -= HandleTargetAccepted;
        CardData.OnNumericAdjustment -= HandleCardAdjustment;
        CardData.OnExhausted -= HandleCardExhaustState;
        CardData.OnDestroyed -= HandleCharacterDestroyed;
        CardData.OnAbilitiesStripped -= HandleAbilityStripped;
        CardData.OnEnhancementsStripped -= HandleEnhancementsStripped;
        CardData.OnGivenAbilities -= HandleAbilitiesGiven;
        CardData.OnGivenEnhancements -= HandleEnhancementsGiven;
        PlayerBase.OnBaseDestroyed -= HandleBaseDestroyed;
        PlayerBase.OnExhaust -= HandleBaseExhaust;
        aDrain.OnStripAllEnhancementsFromSideOfField -= StripAllEnhancementsOnSideOfField;
        aUnderSiege.OnHandToBeRevealed -= HandleRevealTargetHandAndRemoveNonHeros;
        Ability.OnAddAbilityToMasterList -= HandleAddAbilityToList;
        Ability.OnDiscardCard -= HandleCardForceDiscard;

    }

    #endregion

    #region Draft Methods
    public void HandleBuildHeroDraft()
    {
        List<Card> tempHero = Heros.ToList();
        List<string> shareList = new List<string>();
        for (int i = 0; i < 14; i++)
        {
            var picker = UnityEngine.Random.Range(0, tempHero.Count);
            HeroReserve.Add(tempHero[picker]);
            shareList.Add(tempHero[picker].Name);
            tempHero.Remove(tempHero[picker]);
        }

        if (PV.IsMine)
        {
            PV.RPC("ShareCardList", RpcTarget.Others, "HeroReserve", shareList.ToArray());
        }

        DisplayDraft(HeroReserve);
    }

    private void DisplayDraft(List<Card> whichDeck)
    {
        DraftArea.gameObject.SetActive(true);

        foreach(Card card in whichDeck)
        {
            GameObject obj = Instantiate(CardDraftPrefab, DraftArea);
            CardData cd = obj.GetComponent<CardData>();
            cd.CardOverride(card, CardData.FieldPlacement.Draft);
            Draft.Add(cd);
        }
    }

    [PunRPC]
    private void SetupAbilityDraft(bool yes)
    {
        if(!AutoDraft)
        {
            ClearDraft();

            GM.PhaseChange(PhotonGameManager.GamePhase.AbilityDraft);

            DisplayDraft(AbilityDraft);
        }else if(AutoDraft && PhotonGameManager.player == PhotonGameManager.PlayerNum.P1)
        {
            foreach(Card card in P1AutoAbilities)
            {
                P1Deck.Add(card);
            }
            if (PhotonGameManager.myTurn)
            {
                PV.RPC("EndAbilityDraft", RpcTarget.Others, false);
                EndAbilityDraft(true);
            }
            DraftArea.gameObject.SetActive(false);
        }
        else if(AutoDraft && PhotonGameManager.player == PhotonGameManager.PlayerNum.P2)
        {
            foreach (Card card in P2AutoAbilities)
            {
                P1Deck.Add(card);
            }
            if (PhotonGameManager.myTurn)
            {
                PV.RPC("EndAbilityDraft", RpcTarget.Others, false);
                EndAbilityDraft(true);
            }
            DraftArea.gameObject.SetActive(false);
        }
    }

    private void ClearDraft()
    {
        for (int i = Draft.Count - 1; i > -1; i--)
        {
            var item = Draft[i];
            Draft.Remove(item);
            Destroy(item.gameObject);
        }
    }

    [PunRPC]
    private void RemoveDraftOption( string card)
    {
        foreach(CardData item in Draft)
        {
            if(item.Name == card)
            {
                Draft.Remove(item);
                Destroy(item.gameObject);
                if (PhotonGameManager.myTurn)
                {
                    CheckDraft();
                }
                break;
            }
        }
    }

    private void CheckDraft()
    {
        switch (PhotonGameManager.myPhase)
        {
            case PhotonGameManager.GamePhase.HeroDraft:
                if(Draft.Count == 12 && PhotonGameManager.myTurn)
                {
                    
                        HandleCardCollected(HeroReserve[UnityEngine.Random.Range(0, 13)], PhotonGameManager.myPhase);
                        PV.RPC("SetupAbilityDraft", RpcTarget.All, true);
   
                }
                break;
            case PhotonGameManager.GamePhase.AbilityDraft:
                if(Draft.Count == 0 && PhotonGameManager.myTurn)
                {
                    PV.RPC("EndAbilityDraft", RpcTarget.Others, false);
                    EndAbilityDraft(true);
                }
                break;
        }
    }

    [PunRPC]
    private void ShareCardList(string list, string[] listToShare)
    {
        switch (list)
        {
            case "HeroReserve":
                HeroReserve.Clear();
                foreach (string name in listToShare)
                {
                    foreach (Card card in Heros)
                    {
                        if (name == card.Name)
                        {
                            HeroReserve.Add(card);
                        }
                    }
                }

                DisplayDraft(HeroReserve);
                break;
            case "P2Hand":
                P2Hand.Clear();
                foreach (string cardName in listToShare)
                {
                    foreach (Card card in CardBase)
                    {
                        if (card.Name == cardName)
                        {
                            P2Hand.Add(card);
                            break;
                        }
                    }
                }
                GM.SetOpponentHandCount(CardsRemaining(CardDecks.P2Hand));
                break;
            case "P2Discard":
                foreach (string cardName in listToShare)
                {
                    foreach (Card card in CardBase)
                    {
                        if (card.Name == cardName)
                        {
                            P2Discard.Add(card);
                            break;
                        }
                    }
                }
                break;
        }
    }

    [PunRPC]
    private void EndAbilityDraft(bool told)
    {
        if (PhotonGameManager.player == PhotonGameManager.PlayerNum.P1)
        {
            GM.SetTurnGauge(9);
            GM.PhaseChange(PhotonGameManager.GamePhase.HEROSelect);
            FillHQ();
        }
        else
        {
            GM.SetTurnGauge(8);
            GM.PhaseChange(PhotonGameManager.GamePhase.Wait);
        }
    }
    #endregion

    #region Player Declaration
    public void HandlePlayerDeclaration(int player)
    {
        PV.RPC("DeclarePlayer", RpcTarget.OthersBuffered, player);
    }

    [PunRPC]
    private void DeclarePlayer(int currentPlayer)
    {
        switch (currentPlayer)
        {
            case 0:
                GM.SetPlayerNum(PhotonGameManager.PlayerNum.P2);
                break;
            case 1:
                GM.SetPlayerNum(PhotonGameManager.PlayerNum.P1);
                break;
            case 2:
                break;
            case 3:
                break;
        }
    }
    #endregion

    #region Turn Declaration
    public void HandleTurnDeclaration(bool myTurn)
    {
        PV.RPC("DeclaredTurn", RpcTarget.Others, myTurn);
    }

    [PunRPC]
    private void DeclaredTurn(bool myTurn)
    {
        GM.ToldSwitchTurn(myTurn);
    }
    #endregion

    #region Debugging
    public void SetAiDraft(bool set)
    {
        AiDraft = set;
    }

    public void SetAutoDraft(bool set)
    {
        AutoDraft = set;
    }

    public void DrawDraftCard(string draftDeck)
    {
        switch (draftDeck)
        {
            case "HeroReserve":
                OnAiDraftCollected?.Invoke(HeroReserve[UnityEngine.Random.Range(0, HeroReserve.Count)]);
                break;
            case "AbilityDraft":
                OnAiDraftCollected?.Invoke(AbilityDraft[UnityEngine.Random.Range(0, AbilityDraft.Count)]);
                break;
        }
    }

    public void StartDrawSpecificCard()
    {
        DisplayDraft(P1Deck);
        SpecificDraw = true;
    }

    public void DrawSpecificCard(Card card)
    {
        DraftArea.gameObject.SetActive(false);
        P1Hand.Add(card);
        AddCardToHand(card);
        P1Deck.Remove(card);
    }
    #endregion

    #region Private Methods
    private void HandleTargetAccepted(CardData card, Card cardToUse)
    {
        Card.Type type = cardToUse.CardType;
        switch (type)
        {
            case Card.Type.Ability:
                SpawnAbility(cardToUse.Name, card, false);
                break;
            case Card.Type.Enhancement:
                if(cardToUse.Attack > 0)
                {
                    card.AdjustAttack(cardToUse.Attack);
                }
                if(cardToUse.Defense > 0)
                {
                    card.AdjustDefense(cardToUse.Defense);
                }
                break;
            case Card.Type.Character:
                break;
        }
        //RemoveCardFromHand(cardToUse);
        OnTargeting?.Invoke(cardToUse, false);
        GM.TurnCounterDecrement();
    }

    #region HQ and Reserve
    public void FillHQ()
    {
        if(HeroReserve.Count == 0)
        {
            ReserveButton.interactable = false;
        }

        if (HeroReserve.Count > 0 && HQ.Count < 3)
        {
            while(HQ.Count < 3)
            {
                if(HeroReserve.Count == 0)
                {
                    break;
                }

                int picker = UnityEngine.Random.Range(0, HeroReserve.Count);
                if(picker == HeroReserve.Count)
                {
                    picker = HeroReserve.Count -1;
                }
                HQ.Add(HeroReserve[picker]);
                HeroReserve.Remove(HeroReserve[picker]);
            }

            PopulateHQ();
        }
        else
        {
            Debug.Log($"Reserves: {HeroReserve.Count}, HQ: {HQ.Count}");
        }


    }

    private void PopulateHQ()
    {
        //Take the container
        //Empty
        //Add new heros
        List<string> temp = new List<string>();
        List<string> namesAlready = new List<string>();

        if(HQHeros != null)
        {
            foreach(CardData cd in HQHeros)
            {
                namesAlready.Add(cd.Name);
            }
        }

        foreach (Card card in HQ)
        {
            if (!namesAlready.Contains(card.Name))
            {
                GameObject obj = Instantiate(CardHeroHQPrefab, HQArea);
                CardData data = obj.GetComponent<CardData>();
                data.CardOverride(card, CardData.FieldPlacement.HQ);
                HQHeros.Add(data);
                temp.Add(card.Name);
            }
        }

        if(temp.Count == 0 || temp == null)
        {
            temp.Add("None");
            temp.Add("Nada");
        }else if(temp.Count == 1)
        {
            temp.Add("Null");
        }

        PV.RPC("PopulatedHQ", RpcTarget.Others, temp.ToArray());
    }

    [PunRPC]
    private void PopulatedHQ(string[] heros)
    {
        if (heros[0] != "None")
        {
            string Names = "I have received; ";
            foreach (string name in heros)
            {
                Names += $" {name},";
            }
            Debug.Log(Names);

            List<string> cardNames = new List<string>();
            List<Card> cardsToAdd = new List<Card>();

            foreach (CardData card in HQHeros)
            {
                cardNames.Add(card.Name);
            }

            foreach (string name in heros)
            {
                if (!cardNames.Contains(name))
                {
                    if(name == "Null")
                    {
                        continue;
                    }
                    foreach (Card card in Heros)
                    {
                        if (name == card.Name)
                        {
                            cardsToAdd.Add(card);
                            break;
                        }
                    }
                }
            }

            foreach (Card addCard in cardsToAdd)
            {
                HeroReserve.Remove(addCard);
                HQ.Add(addCard);
                GameObject obj = Instantiate(CardHeroHQPrefab, HQArea);
                CardData data = obj.GetComponent<CardData>();
                data.CardOverride(addCard, CardData.FieldPlacement.HQ);
                HQHeros.Add(data);
            }
        }
        if (HeroReserve.Count == 0)
        {
            ReserveButton.interactable = false;
        }
    }

    private void RemoveHQCard(Card card)
    {
        foreach(CardData data in HQHeros)
        {
            if(data.Name == card.Name)
            {
                HQHeros.Remove(data);
                HQ.Remove(card);
                Destroy(data.gameObject);
                break;
            }
        }

        PV.RPC("RemoveHQHero", RpcTarget.Others, card.Name);
    }

    [PunRPC]
    private void RemoveHQHero(string cardName)
    {
        foreach (CardData data in HQHeros)
        {
            if (data.Name == cardName)
            {
                HQHeros.Remove(data);
                HQ.Remove(data.myCard);
                Destroy(data.gameObject);
                break;
            }
        }
    }

    [PunRPC]
    private void RemoveHeroFromReserve(string hero)
    {
        foreach(Card card in HeroReserve)
        {
            if(card.Name == hero)
            {
                HeroReserve.Remove(card);
                break;
            }
        }
        if (HeroReserve.Count == 0)
        {
            ReserveButton.interactable = false;
        }
    }
    #endregion

    #region Hand and Cards
    private void UpdateHandSlider()
    {
        if(P1Hand.Count != 0)
        {
            sHandSlider.maxValue = P1Hand.Count-1;
            return;
        }

        sHandSlider.maxValue = 0;
        
    }

    private void DrawRandomCard(List<Card> whatDeck)
    {
        var picker = UnityEngine.Random.Range(0, whatDeck.Count - 1);
        Card pickedCard = whatDeck[picker];
        P1Hand.Add(pickedCard);
        whatDeck.Remove(whatDeck[picker]);
        AddCardToHand(pickedCard);

        if(whatDeck == HeroReserve)
        {
            PV.RPC("RemoveHeroFromReserve", RpcTarget.Others, pickedCard.Name);
            if (HeroReserve.Count == 0)
            {
                ReserveButton.interactable = false;
            }
        }
    }

    private void AddCardToHand(Card cardToAdd)
    {
        if(P1Hand.Count < 8)
        {
            //Debug.Log($"Adding a card to hand(hand): {P1Hand.Count}");
            GameObject obj = Instantiate(CardHandPrefab, Hand[P1Hand.Count - 1].transform);
            CardData data = obj.GetComponent<CardData>();
            data.CardOverride(cardToAdd, CardData.FieldPlacement.Hand);
            lHandData.Add(data);
            //Debug.Log($"Adding a card to hand(visual hand): {lHandData.Count}");
            CheckActiveCard();
        }
        UpdateHandSlider();
        GetHandToShare();
        handSize = P1Hand.Count;
        GM.PassiveActivate(Ability.PassiveType.HandCardAdjustment);
    }

    private void RemoveCardFromHand(Card cardToRemove)
    {
        //Debug.Log($"Removing a card from hand(hand) before removed: {P1Hand.Count}");
        if(P1Hand.Count <= 7)
        {
            //Debug.Log($"Hand visual: {lHandData.Count}");
            if(lHandData.Count == P1Hand.Count)
            {
                GameObject obj = lHandData[P1Hand.Count-1].gameObject;
                lHandData.Remove(lHandData[P1Hand.Count-1]);
                Destroy(obj);
            }
            else
            {
                Debug.Log("lHandData doesn't match hand count.");
            }
            //Debug.Log($"Removing a card from hand(visual hand): {lHandData.Count}");

        }
        //Debug.Log($"Removing {cardToRemove.Name}");
        P1Hand.Remove(cardToRemove);
        //Debug.Log($"Removing a card from hand(hand) after removed: {P1Hand.Count}");
        UpdateHandSlider();
        HandCardOffset(0);
        GetHandToShare();
        handSize = P1Hand.Count;
        GM.PassiveActivate(Ability.PassiveType.HandCardAdjustment);
    }

    private void HandleCardForceDiscard(string who, string type, int amount)
    {
        switch (who)
        {
            case "All":
                PV.RPC("ForceDiscard", RpcTarget.All, type, amount);
                break;
            case "Target":
                PV.RPC("ForceDiscard", RpcTarget.Others, type, amount);
                break;
        }
    }

    [PunRPC]
    private void ForceDiscard(string type, int amount)
    {
        switch (type)
        {
            case "Random":
                for(int i = amount; i>0; i--)
                {
                    RemoveCardFromHand(P1Hand[UnityEngine.Random.Range(0, P1Hand.Count)]);
                }
                break;
            case "Hero":
                break;
            case "Enhancement":
                break;
            case "Ability":
                break;
            case "Feat":
                break;
        }
    }

    private void HandleCardAdjustment(CardData cardToAdjust, string category, int newValue)
    {
        //could also set it to specific player
        PV.RPC("CardAdjustment", RpcTarget.OthersBuffered, cardToAdjust.Name, category, newValue);
    }

    [PunRPC]
    private void CardAdjustment(string name, string category, int newValue)
    {
        //P2Field could be set to a specified player
        bool found = false;
        foreach(CardData data in P2Field)
        {
            if(data.Name == name)
            {
                switch (category)
                {
                    case "Attack":
                        data.SetAttack(newValue);
                        break;
                    case "Defense":
                        data.SetDefense(newValue);
                        break;
                }
                found = true;
                break;
            }
        }
        if (!found)
        {
            foreach(CardData data in P1Field)
            {
                if (data.Name == name)
                {
                    switch (category)
                    {
                        case "Attack":
                            data.SetAttack(newValue);
                            break;
                        case "Defense":
                            data.SetDefense(newValue);
                            break;
                    }
                    break;
                }
            }
        }
        else
        {
            Debug.Log("I found the Hero Card and adjusted it's values.");
        }
    }

    private void CheckActiveCard()
    {
        int i = P1Hand.Count;
        switch (i)
        {
            case 0:
                CurrentActiveCard = null;
                break;
            case 1:
                CurrentActiveCard = lHandData[0];
                break;
            case 2:
                CurrentActiveCard = lHandData[1];
                break;
            case 3:
                CurrentActiveCard = lHandData[1];
                break;
            case 4:
                CurrentActiveCard = lHandData[3];
                break;
            case 5:
                CurrentActiveCard = lHandData[3];
                break;
            default:
                CurrentActiveCard = lHandData[5];
                break;
        }
        if(i != 0)
        GM.CheckHandZoomInEffect();
    }

    private void GetHandToShare()
    {
        List<string> names = new List<string>();
        foreach(Card card in P1Hand)
        {
            names.Add(card.Name);
        }
        PV.RPC("ShareCardList", RpcTarget.Others, "P2Hand" ,names.ToArray());
    }

    private void HandleCharacterDestroyed(CardData card)
    {
        string loc = "";
        if (P2Field.Contains(card))
        {
            P2Field.Remove(card);
            Destroy(card.gameObject);
            loc = "P2Field";
        }else if (P1Field.Contains(card))
        {
            P1Field.Remove(card);
            Destroy(card.gameObject);
            loc = "P1Field";
        }

        herosFatigued--;
        GM.PassiveActivate(Ability.PassiveType.CharacterDestroyed);
        PV.RPC("FieldCardDestroy", RpcTarget.Others, card.Name, loc);
    }

    [PunRPC]
    private void FieldCardDestroy(string name, string location)
    {
        switch (location)
        {
            case "P1Field":
                foreach(CardData card in P2Field)
                {
                    if(card.Name == name)
                    {
                        P2Field.Remove(card);
                        Destroy(card.gameObject);
                        break;
                    }
                }
                break;
            case "P2Field":
                foreach (CardData card in P1Field)
                {
                    if (card.Name == name)
                    {
                        P1Field.Remove(card);
                        Destroy(card.gameObject);
                        break;
                    }
                }
                break;
        }
        herosFatigued--;
    }

    private void HandleCardExhaustState(CardData card, bool exhaust)
    {
        string loc = "";
        if (P1Field.Contains(card))
        {
            loc = "P1Field";
        }else if (P2Field.Contains(card))
        {
            loc = "P2Field";
        }

        herosFatigued = exhaust ? herosFatigued++ : herosFatigued--;
        Debug.Log($"Heros currently Fatigued: {herosFatigued}");

        PV.RPC("ExhaustStateAdjust", RpcTarget.Others, card.Name, loc, exhaust);
    }

    [PunRPC]
    private void ExhaustStateAdjust(string name, string location, bool state)
    {
        switch (location)
        {
            case "P1Field":
                foreach(CardData card in P2Field)
                {
                    if(card.Name == name)
                    {
                        if (state)
                        {
                            card.Exhaust(true);
                            herosFatigued++;
                        }
                        else
                        {
                            card.Heal(true);
                            herosFatigued++;
                        }
                    }
                }
                break;
            case "P2Field":
                foreach (CardData card in P1Field)
                {
                    if (card.Name == name)
                    {
                        if (state)
                        {
                            card.Exhaust(true);
                            herosFatigued++;
                        }
                        else
                        {
                            card.Heal(true);
                            herosFatigued++;
                        }
                    }
                }
                break;
        }
    }

    private void HandleRevealTargetHandAndRemoveNonHeros()
    {
        ClearDraft();
        cardListToDisplay = P2Hand;
        StartCoroutine(DisplayExtraDraft());
        PV.RPC("RemoveAllNonHerosFromHand", RpcTarget.Others, "P1Hand");
    }

    [PunRPC]
    private void RemoveAllNonHerosFromHand(string hand)
    {
        List<Card> removeList = new List<Card>();

        foreach(Card card in P1Hand)
        {
            if(card.CardType != Card.Type.Character)
            {
                removeList.Add(card);
            }
        }

        foreach(Card card in removeList)
        {
            RemoveCardFromHand(card);
        }
    }
    #endregion

    #region Spawn Character, Ability, Enhancement Functions
    [PunRPC]
    private void SpawnCharacterToOpponentField(string heroToSpawn)
    {
        foreach(Card card in Heros)
        {
            if(card.Name == heroToSpawn)
            {
                GameObject obj = Instantiate(CardOppFieldPrefab, OppHeroArea);
                CardData data = obj.GetComponent<CardData>();
                data.CardOverride(card, CardData.FieldPlacement.Opp);
                P2Field.Add(data);
                break;
            }
        }
    }

    private void SpawnCharacterToMyField(Card card)
    {
        GameObject obj = Instantiate(CardMyFieldPrefab, MyHeroArea);
        CardData data = obj.GetComponent<CardData>();
        data.CardOverride(card, CardData.FieldPlacement.Mine);
        P1Field.Add(data);
    }

    private void SpawnAbility(string AbilityName, CardData cardToAttachTo, bool told)
    {
        Debug.Log($"Spawning {AbilityName} on {cardToAttachTo}.");
        switch (AbilityName)
        {
            case "ACCELERATE":
                Ability a = cardToAttachTo.gameObject.AddComponent<aAccelerate>();
                cardToAttachTo.AdjustCounter(1, a);
                break;
            case "BACKFIRE":
                Ability b = cardToAttachTo.gameObject.AddComponent<aBackfire>();
                cardToAttachTo.AdjustCounter(1, b);
                break;
            case "BOLSTER":
                Ability c = cardToAttachTo.gameObject.AddComponent<aBolster>();
                cardToAttachTo.AdjustCounter(1, c);
                break;
            case "BOOST":
                Ability d = cardToAttachTo.gameObject.AddComponent<aBoost>();
                cardToAttachTo.AdjustCounter(1, d);
                break;
            case "COLLATERAL DAMAGE":
                Ability e = cardToAttachTo.gameObject.AddComponent<aCollateralDamage>();
                cardToAttachTo.AdjustCounter(1, e);
                break;
            case "CONVERT":
                Ability f = cardToAttachTo.gameObject.AddComponent<aConvert>();
                cardToAttachTo.AdjustCounter(1, f);
                break;
            case "COUNTER-MEASURES":
                Ability g = cardToAttachTo.gameObject.AddComponent<aCounterMeasures>();
                cardToAttachTo.AdjustCounter(1, g);
                break;
            case "DROUGHT":
                Ability h = cardToAttachTo.gameObject.AddComponent<aDrought>();
                cardToAttachTo.AdjustCounter(1, h);
                break;
            case "FORTIFICATION":
                Ability i = cardToAttachTo.gameObject.AddComponent<aFortification>();
                cardToAttachTo.AdjustCounter(1, i);
                break;
            case "GOING NUCLEAR":
                Ability j = cardToAttachTo.gameObject.AddComponent<aGoingNuclear>();
                cardToAttachTo.AdjustCounter(1, j);
                break;
            case "HARDENED":
                Ability k = cardToAttachTo.gameObject.AddComponent<aHardened>();
                cardToAttachTo.AdjustCounter(1, k);
                break;
            case "IMPEDE":
                Ability l = cardToAttachTo.gameObject.AddComponent<aImpede>();
                cardToAttachTo.AdjustCounter(1, l);
                break;
            case "KAIROS":
                Ability m = cardToAttachTo.gameObject.AddComponent<aKairos>();
                cardToAttachTo.AdjustCounter(1, m);
                break;
            case "PREVENTION":
                Ability n = cardToAttachTo.gameObject.AddComponent<aPrevention>();
                cardToAttachTo.AdjustCounter(1, n);
                break;
            case "PROTECT":
                Ability o = cardToAttachTo.gameObject.AddComponent<aProtect>();
                cardToAttachTo.AdjustCounter(1, o);
                break;
            case "REDUCTION":
                Ability p = cardToAttachTo.gameObject.AddComponent<aAccelerate>();
                cardToAttachTo.AdjustCounter(1, p);
                break;
            case "REINFORCEMENT":
                Ability q = cardToAttachTo.gameObject.AddComponent<aReinforcement>();
                cardToAttachTo.AdjustCounter(1, q);
                break;
            case "RESURRECT":
                Ability r = cardToAttachTo.gameObject.AddComponent<aResurrect>();
                cardToAttachTo.AdjustCounter(1, r);
                break;
            case "REVELATION":
                Ability s = cardToAttachTo.gameObject.AddComponent<aRevelation>();
                cardToAttachTo.AdjustCounter(1, s);
                break;
            case "SHEILDING":
                Ability t = cardToAttachTo.gameObject.AddComponent<aShielding>();
                cardToAttachTo.AdjustCounter(1, t);
                break;
            default:
                Debug.Log($"An ability was requested that doesn't exist. {AbilityName}");
                break;
        }
        if (!told)
        {
            PV.RPC("AttachAbility", RpcTarget.OthersBuffered, AbilityName, cardToAttachTo.Name);
        }
    }

    [PunRPC]
    private void AttachAbility(string abilityName, string cardName)
    {
        Debug.Log($"I was told to attach {abilityName} to {cardName}");

        CardData card = FindCardOnField(cardName);
        SpawnAbility(abilityName, card, true);
    }

    private void HandleAddAbilityToList(Ability ability)
    {
        if(ability.myType == Ability.Type.Character)
        {
            heroAbilitiesOnField.Add(ability);
            Debug.Log($"{ability.Name} added to master list.");
        }
        else if(ability.myType == Ability.Type.Activate || ability.myType == Ability.Type.Passive)
        {
            cardAbilitiesOnField.Add(ability);
            Debug.Log($"{ability.Name} added to master list.");

        }
    }

    private void StripAllEnhancementsOnSideOfField(string side)
    {
        if(side == "P2Field")
        {
            foreach(CardData card in P2Field)
            {
                card.StripAbilities(false);
                card.StripEnhancements(false);
            }
            return;
        }

        foreach(CardData card in P1Field)
        {
            card.StripAbilities(false);
            card.StripEnhancements(false);
        }
    }

    [PunRPC]
    private void HandleAbilityHandOver(string nameOfAbilityToGiveControl)
    {
        Ability ability = heroAbilitiesOnField[0];
        bool found = false;
        foreach(Ability a in heroAbilitiesOnField)
        {
            if(a.Name == nameOfAbilityToGiveControl)
            {
                ability = a;
                found = true;
                Debug.Log($"Found {nameOfAbilityToGiveControl} in HeroAbilities");
                break;
            }
        }
        if(found == false)
        {
            foreach (Ability a in cardAbilitiesOnField)
            {
                if (a.Name == nameOfAbilityToGiveControl)
                {
                    ability = a;
                    Debug.Log($"Found {nameOfAbilityToGiveControl} in CardAbilities");
                    break;
                }
            }
        }

        foreach(CardData card in P1Field)
        {
            if (card.myAbilities.Contains(ability))
            {
                GM.SetActiveAbility(ability);
            }
        }
    }

    private void HandleEnhancementsStripped(CardData card)
    {
        PV.RPC("StripEnhancements", RpcTarget.Others, card.Name);
    }

    [PunRPC]
    private void StripEnhancements(string name)
    {
        FindCardOnField(name).StripEnhancements(true);
    }

    private void HandleAbilityStripped(CardData card)
    {
        PV.RPC("StripAbilities", RpcTarget.Others, card.Name);
    }

    [PunRPC]
    private void StripAbilities(string name)
    {
        FindCardOnField(name).StripAbilities(true);
    }

    private void HandleEnhancementsGiven(List<Enhancement> enhancements, CardData card)
    {
        //need to give enhancements to a card over cloud
        List<int[]> enhancementNums = new List<int[]>();

        foreach(Enhancement e in enhancements)
        {
            int[] i = new int[2];
            i[0] = e.attack;
            i[1] = e.defense;

            enhancementNums.Add(i);
        }

        if(enhancementNums.Count > 1)
        {
            PV.RPC("GiveEnhancements", RpcTarget.Others, enhancementNums, card.Name);
        }else if(enhancementNums.Count == 1)
        {
            if(enhancementNums[0][0] > 0)
            {
                PV.RPC("CardAdjustment", RpcTarget.Others, card.Name, "Attack", enhancementNums[0][0]);
                return;
            }

            PV.RPC("CardAdjustment", RpcTarget.Others, card.Name, "Defense", enhancementNums[0][1]);
        }
    }

    [PunRPC]
    private void GiveEnhancements(List<int[]> enhancements, string name)
    {
        CardData card = FindCardOnField(name);
        List<Enhancement> en = new List<Enhancement>();
        
        foreach(int[] array in enhancements)
        {
            en.Add(ConvertEnhancementIntArrayToEnhancement(array));
        }

        card.GainEnhancements(en, true);
    }

    private void HandleAbilitiesGiven(List<Ability> abilities, CardData card)
    {
        List<string> abilityNames = new List<string>();

        foreach(Ability a in abilities)
        {
            PV.RPC("AttachAbility", RpcTarget.Others, a.Name, card.Name);
        }
    }

    private CardData FindCardOnField(string name)
    {

        foreach(CardData cd in P1Field)
        {
            if(cd.Name == name)
            {
                return cd;
            }
        }

        foreach(CardData cd in P2Field)
        {
            if(cd.Name == name)
            {
                return cd;
            }
        }

        Debug.Log($"Didn't find {name}");
        return P1Field[0];
    }

    private Enhancement ConvertEnhancementIntArrayToEnhancement(int[] array)
    {
        Debug.Log("Converting Int Array into Enhancement");
        Enhancement enhancement = new Enhancement();
        enhancement.attack = array[0];
        enhancement.defense = array[1];

        return enhancement;
    }
    #endregion

    #region Base Controls
    private void HandleBaseExhaust(PlayerBase player)
    {
        if (player.type == PlayerBase.Type.Player)
        {
            PV.RPC("ExhaustABase", RpcTarget.Others, "Opponent");
        }
        else if (player.type == PlayerBase.Type.Opponent)
        {
            PV.RPC("ExhaustABase", RpcTarget.Others, "Player");
        }
    }

    [PunRPC]
    private void ExhaustABase(string player)
    {
        switch (player)
        {
            case "Player":
                GM.OnBaseExhausted(PlayerBase.Type.Player);
                break;
            case "Opponent":
                GM.OnBaseExhausted(PlayerBase.Type.Opponent);
                break;
        }
    }

    private void HandleBaseDestroyed(PlayerBase player)
    {
        if (player.type == PlayerBase.Type.Player)
        {
            PV.RPC("DestroyABase", RpcTarget.Others, "Opponent");
            GM.OnBaseDestroyed(player.type);
        }else if(player.type == PlayerBase.Type.Opponent)
        {
            PV.RPC("DestroyABase", RpcTarget.Others, "Player");
            GM.OnBaseDestroyed(player.type);
        }
    }

    [PunRPC]
    private void DestroyABase(string player)
    {
        switch (player)
        {
            case "Player":
                GM.OnBaseDestroyed(PlayerBase.Type.Player);
                break;
            case "Opponent":
                GM.OnBaseDestroyed(PlayerBase.Type.Opponent);
                break;
        }
    }
    #endregion

    private void SetFeatToActiveAbility(Card card)
    {
        string name = card.Name;
        switch (name)
        {
            case "ABSORB":
                activeFeat = GM.gameObject.AddComponent<aAbsorb>();
                GM.SetActiveAbility((Ability)activeFeat);
                break;
            case "DRAIN":
                activeFeat = GM.gameObject.AddComponent<aDrain>();
                GM.SetActiveAbility((Ability)activeFeat);
                break;
            case "PAY THE COST":
                activeFeat = GM.gameObject.AddComponent<aPaytheCost>();
                GM.SetActiveAbility((Ability)activeFeat);
                break;
            case "UNDER SEIGE":
                activeFeat = GM.gameObject.AddComponent<aUnderSiege>();
                GM.SetActiveAbility((Ability)activeFeat);
                break;
        }
    }
    #endregion

    #region Public Methods
    public void SilenceAbilityToField(PhotonGameManager.PlayerNum player, int turns)
    {
        switch (player)
        {
            case PhotonGameManager.PlayerNum.P1:
                PV.RPC("SilenceAbilityToFieldCall", RpcTarget.Others, "P1", turns);
                break;
            case PhotonGameManager.PlayerNum.P2:
                PV.RPC("SilenceAbilityToFieldCall", RpcTarget.Others, "P2", turns);
                break;
            case PhotonGameManager.PlayerNum.P3:
                PV.RPC("SilenceAbilityToFieldCall", RpcTarget.Others, "P3", turns);
                break;
            case PhotonGameManager.PlayerNum.P4:
                PV.RPC("SilenceAbilityToFieldCall", RpcTarget.Others, "P4", turns);
                break;
        }
    }

    [PunRPC]
    private void SilenceAbilityToFieldCall(string name, int turns)
    {
        PhotonGameManager.PlayerNum num = PhotonGameManager.PlayerNum.P1;
        switch (name)
        {
            case "P1":
                num = PhotonGameManager.PlayerNum.P1;
                break;
            case "P2":
                num = PhotonGameManager.PlayerNum.P2;
                break;
            case "P3":
                num = PhotonGameManager.PlayerNum.P3;
                break;
            case "P4":
                num = PhotonGameManager.PlayerNum.P4;
                break;
        }
        if(PhotonGameManager.player == num)
        {
            GM.SilenceAbilityToField(turns);
        }
    }

    public void AbilityHandover(Ability ability)
    {
        string name = ability.Name;

        PV.RPC("HandleAbilityHandOver", RpcTarget.Others, name);
    }

    public void SendPreviousAttackersAndDefender(List<CardData> attackers, CardData defender)
    {
        List<string> aNames = new List<string>();

        foreach(CardData card in attackers)
        {
            aNames.Add(card.Name);
        }

        if(defender != null)
        {
            Debug.Log($"Defender was {defender.Name}");
            aNames.Add(defender.Name);
        }
        else
        {
            Debug.Log("Defender was NULL");
            aNames.Add("NULL");
        }

        PV.RPC("HandlePreviousAttackersAndDefender", RpcTarget.Others, aNames.ToArray());
    }

    [PunRPC]
    private void HandlePreviousAttackersAndDefender(string[] names)
    {
        Debug.Log("Made it into the server HandlePreviousAttackersAndDefender.");
        List<CardData> cards = new List<CardData>();

        foreach(CardData data in P2Field)
        {
            if (names.Contains(data.Name))
            {
                cards.Add(data);
            }
        }
        PhotonGameManager.PreviousAttackers = cards;

        if(names[names.Length-1] != "NULL")
        {
            foreach(CardData data in P1Field)
            {
                if(data.Name == names[names.Length - 1])
                {
                    PhotonGameManager.PreviousDefender = data;
                    break;
                }
            }
        }
    }

    public bool CheckIfMyCard(CardData card)
    {
        bool isMyCard = P1Field.Contains(card);

        return isMyCard;
    }

    public bool CheckFieldForOpponents()
    {
        if(P2Field.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CheckMyFieldForUsableHeros()
    {
        bool usable = false;
        foreach (CardData card in P1Field)
        {
            if (!card.Exhausted)
            {
                usable = true;
                break;
            }
        }

            return usable;
    }

    public bool CheckForHealableHeros()
    {
        bool healable = false;

        foreach(CardData card in P1Field)
        {
            if (card.Exhausted)
            {
                return true;
            }
        }

        return healable;
    }

    public bool CheckIfFeatCards()
    {
        bool haveCard = false;

        foreach(Card card in P1Hand)
        {
            if(card.CardType == Card.Type.Feat)
            {
                return true;
            }
        }

        return haveCard;
    }

    public void HandCardOffset(System.Single offset)
    {
        int o = (int)Math.Floor(offset);
        for(int i = 0; i < lHandData.Count; i++)
        {
            int j = o+i;
            if(j >= P1Hand.Count)
            {
                j -= P1Hand.Count;
            }
            lHandData[i].CardOverride(P1Hand[j], CardData.FieldPlacement.Hand);
        }
        CheckActiveCard();
    }

    public void PlayCard(Card card)
    {
        //Card should be determined based on type how it will be played.
        switch (card.CardType)
        {
            case Card.Type.Ability:
                //Target a Character
                //Update characters
                if (!GM.canPlayAbilitiesToFieldCheck())
                {
                    return;
                }
                OnTargeting?.Invoke(card, true);
                bTargeting = true;
                break;
            case Card.Type.Character:
                //Place Character on the field
                //Spawn a Character on the field
                SpawnCharacterToMyField(card);
                PV.RPC("SpawnCharacterToOpponentField", RpcTarget.OthersBuffered, card.Name);
                GM.TurnCounterDecrement();
                break;
            case Card.Type.Enhancement:
                //Target a Character
                //Update character
                OnTargeting?.Invoke(card, true);
                bTargeting = true;
                break;
            case Card.Type.Feat:
                SetFeatToActiveAbility(card);
                //GM.ToldSwitchTurn(false);
                //HandleTurnDeclaration(true);
                break;
        }
        RemoveCardFromHand(card);
    }

    public void DrawCard(CardDecks Deck)
    {
        switch (Deck)
        {
            case CardDecks.P1Deck:
                DrawRandomCard(P1Deck);
                break;
        }
    }

    public void DrawReserveCard()
    {
        if(PhotonGameManager.myTurn && PhotonGameManager.myPhase == PhotonGameManager.GamePhase.Recruit)
        {
            DrawRandomCard(HeroReserve);
            GM.PassiveActivate(Ability.PassiveType.HeroRecruited);
            GM.TurnCounterDecrement();
        }
    }

    public int CardsRemaining(CardDecks Deck)
    {
        int i = 0;
        switch (Deck)
        {
            case CardDecks.HQ:
                i = HeroReserve.Count;
                break;
            case CardDecks.P1Deck:
                i = P1Deck.Count;
                break;
            case CardDecks.P1Hand:
                i = P1Hand.Count;
                break;
            case CardDecks.P1Discard:
                i = P1Discard.Count;
                break;
            case CardDecks.P2Hand:
                i = P2Hand.Count;
                break;
            case CardDecks.Reserve:
                i = HeroReserve.Count;
                break;
        }
        return i;
    }

    public void HandleCardCollected(Card card, PhotonGameManager.GamePhase phase)
    {
        switch (phase)
        {
            case PhotonGameManager.GamePhase.HeroDraft:
                P1Hand.Add(card);
                AddCardToHand(card);
                HeroReserve.Remove(card);
                PV.RPC("RemoveDraftOption", RpcTarget.All, card.Name);
                break;
            case PhotonGameManager.GamePhase.AbilityDraft:
                P1Deck.Add(card);
                UpdateHandSlider();
                AbilityDraft.Remove(card);
                PV.RPC("RemoveDraftOption", RpcTarget.All, card.Name);
                break;
            case PhotonGameManager.GamePhase.Recruit:
                P1Hand.Add(card);
                AddCardToHand(card);
                RemoveHQCard(card);
                GM.PassiveActivate(Ability.PassiveType.HeroRecruited);
                break;
        }
    }
    #endregion

    #region IEnumerators
    private IEnumerator DisplayExtraDraft()
    {
        DisplayDraft(cardListToDisplay);
        Debug.Log("New Card list displayed.");
        yield return new WaitForSeconds(5f);
        DraftArea.gameObject.SetActive(false);
        GM.ToldSwitchTurn(false);
        HandleTurnDeclaration(true);
    }
    #endregion
}
