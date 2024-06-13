//Created by Jordan Ezell
//Last Edited: 6/12/24 Jordan

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using System;
using System.Collections;

public class CardDataBase : MonoBehaviour
{
    public PhotonInGameManager myManager;
    public Referee GM;

    public Button BOppInfo;
    public Button BOppSelection;
    public Button BMyInfo;
    public Button BMySelection;


    public CardData CurrentActiveCard;

    public enum CardDecks { MyHand, MyDeck, MyField, MyDiscard, OppHand, OppDeck, OppField, OppDiscard, Reserve, HQ}

    public static Action<bool> OnTurnDelcarationReceived = delegate { };
    public static Action<Card> OnAiDraftCollected = delegate { };
    public static Action<Card, bool> OnTargeting = delegate { };
    public static Action OnSendHeroStats = delegate { };
    public static Action<string> OnSendYasmineAbilityRequest = delegate { };
    public static Action<string> OnZhaoAbilityRequest = delegate { };
    public static Action OnRequestHeroModified = delegate { };

    public GameObject CardHandPrefab;
    public GameObject CardDraftPrefab;
    public GameObject CardMyFieldPrefab;
    public GameObject CardOppFieldPrefab;
    public GameObject CardHeroHQPrefab;

    public Button ReserveButton;
    public GameObject ReserveButtonParentObject;

    public GameObject[] Hand = new GameObject[7];
    public GameObject HandContainer;
    public GameObject HandButton;
    private List<CardData> lHandData = new List<CardData>();
    public Slider sHandSlider;

    public List<CardData> Draft = new List<CardData>();
    public Transform CardDisplayArea;
    public GameObject CardDisplayAreaOffButton;
    public Transform MyHeroArea;
    public Transform OppHeroArea;
    public Transform HQArea;
    public List<CardData> HQHeros = new List<CardData>();

    public static bool bTargeting = false;
    public static int handSize = 0;
    public static int herosFatigued = 0;
    public static int herosModified = 0;
    public static int heroCount = 0;

    #region Debuging
    public bool AiDraft = false;
    public bool AutoDraft = false;
    public bool SpecificDraw = false;
    List<Card> MasterList = new List<Card>();
    #endregion

    #region Card Database
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
    List<Card> MyHand = new List<Card>();
    List<CardData> MyField = new List<CardData>();
    List<Card> MyDeck = new List<Card>();
    private List<Card> myDiscard = new List<Card>();
    public List<Card> MyDiscard { get { return myDiscard; } }
    List<Card> DiscardedCards = new List<Card>();

    List<Card> OppHand = new List<Card>();
    List<CardData> OppField = new List<CardData>();
    List<Card> OppDeck = new List<Card>();
    List<Card> OppDiscard = new List<Card>();

    List<string> aNames = new List<string>();

    List<Card> cardListToDisplay = new List<Card>();
    Component activeFeat;

    List<Ability> heroAbilitiesOnField = new List<Ability>();
    List<Ability> cardAbilitiesOnField = new List<Ability>();
    #endregion

    #region Unity Methods

    private void Awake()
    { 

        UIConfirmation.OnTargetAccepted += HandleTargetAccepted;
        CardData.OnNumericAdjustment += HandleCardAdjustment;
        CardData.OnExhausted += HandleCardExhaustState;
        CardData.OnDestroyed += HandleCharacterDestroyed;
        CardData.OnAbilitiesStripped += HandleAbilityStripped;
        CardData.OnEnhancementsStripped += HandleEnhancementsStripped;
        CardData.OnGivenAbilities += HandleAbilitiesGiven;
        CardData.OnGivenEnhancements += HandleEnhancementsGiven;
        CardData.OnRequestStats += HandleRequestOppHeroStats;
        CardData.OnSendStats += HandleStatsSend;
        aDrain.OnStripAllEnhancementsFromSideOfField += StripAllEnhancementsOnSideOfField;
        aUnderSiege.OnHandToBeRevealed += HandleRevealTargetHandAndRemoveNonHeros;
        Ability.OnAddAbilityToMasterList += HandleAddAbilityToList;
        Ability.OnDiscardCard += HandleCardForceDiscard;
        Ability.OnPreventAbilitiesToFieldForTurn += HandleAbilityToFieldSilence;
        Ability.OnOpponentAbilityActivation += AbilityHandover;
        Ability.OnHandOverControl += AbilityDehandover;
        UIConfirmation.OnNeedDrawEnhanceCards += DrawFromEnhanceDeck;
        UIConfirmation.OnNeedDrawFromDiscard += DrawFromDiscard;
        Ability.OnNeedPlayFromReserve += HandlePlayCardFromReserve;
        UIConfirmation.OnPlayCardFromHand += HandlePlayCardFromHandSetup;
        UIConfirmation.OnAccelerate += HandleAccelerate;
        CardFunction.OnCardDiscarded += HandleChooseDiscardCard;
        UIConfirmation.OnBackfire += HandleAbilityListRequest;
        CardData.OnSendModifiedStatus += HandleHeroModifiedCensus;
        UIConfirmation.OnBoost += HandleBoost;

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
            MyDeck.Add(Enhancements[0]);
        }
        Enhancements[1] = new Card(Card.Type.Enhancement, "ATTACK 30", 30, 0, EnhanceImages[1]);
        for (int i = 0; i < 3; i++)
        {
            MyDeck.Add(Enhancements[1]);
        }
        Enhancements[2] = new Card(Card.Type.Enhancement, "DEFENSE 30", 0, 30, EnhanceImages[2]);
        for (int i = 0; i < 2; i++)
        {
            MyDeck.Add(Enhancements[2]);
        }
        Enhancements[3] = new Card(Card.Type.Enhancement, "DEFENSE 20", 0, 20, EnhanceImages[3]);
        for (int i = 0; i < 3; i++)
        {
            MyDeck.Add(Enhancements[3]);
        }


        Feats[0] = new Card(Card.Type.Feat, "ABSORB", "(H) Discard the Enhancements, if any, from any one hero. Then, replace with those from another hero.", FeatImages[0]);
        Feats[1] = new Card(Card.Type.Feat, "DRAIN", "(H) Discard all of one opponenet's Enhancement Cards from the field.", FeatImages[1]);
        Feats[2] = new Card(Card.Type.Feat, "PAY THE COST", "(H) Fatigue one hero in your play area, to remove one hero from the field.", FeatImages[2]);
        Feats[3] = new Card(Card.Type.Feat, "UNDER SEIGE", "(H) Target opponent reveals their hand, then discards all non-hero cards.", FeatImages[3]);
        
        foreach(Card item in Heros)//Debuggong only
        {
            MasterList.Add(item);
        }
        foreach(Card item in Abilities)
        {
            AbilityDraft.Add(item);
            MasterList.Add(item);
        }
        foreach(Card item in Feats)
        {
            AbilityDraft.Add(item);
            MasterList.Add(item);
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
        CardData.OnRequestStats -= HandleRequestOppHeroStats;
        CardData.OnSendStats -= HandleStatsSend;
        aDrain.OnStripAllEnhancementsFromSideOfField -= StripAllEnhancementsOnSideOfField;
        aUnderSiege.OnHandToBeRevealed -= HandleRevealTargetHandAndRemoveNonHeros;
        Ability.OnAddAbilityToMasterList -= HandleAddAbilityToList;
        Ability.OnDiscardCard -= HandleCardForceDiscard;
        Ability.OnPreventAbilitiesToFieldForTurn -= HandleAbilityToFieldSilence;
        Ability.OnOpponentAbilityActivation -= AbilityHandover;
        Ability.OnHandOverControl -= AbilityDehandover;
        UIConfirmation.OnNeedDrawEnhanceCards -= DrawFromEnhanceDeck;
        UIConfirmation.OnNeedDrawFromDiscard -= DrawFromDiscard;
        Ability.OnNeedPlayFromReserve -= HandlePlayCardFromReserve;
        UIConfirmation.OnPlayCardFromHand -= HandlePlayCardFromHandSetup;
        UIConfirmation.OnAccelerate -= HandleAccelerate;
        CardFunction.OnCardDiscarded -= HandleChooseDiscardCard;
        UIConfirmation.OnBackfire -= HandleAbilityListRequest;
        CardData.OnSendModifiedStatus -= HandleHeroModifiedCensus;
        UIConfirmation.OnBoost -= HandleBoost;
    }   

    private void HandleAbilityListRequest()
    {
        ShowAbilityList(Ability.Type.Activate);
    }

    private void ShowAbilityList(Ability.Type t)
    {
        List<Ability> list = new List<Ability>();
        switch (t)
        {
            case Ability.Type.Activate:
                foreach(Ability a in heroAbilitiesOnField)
                {
                    if (a.GetPlacement() == CardData.FieldPlacement.Opp && a.secondaryType == Ability.Type.Activate) list.Add(a);
                }
                foreach (Ability a in cardAbilitiesOnField)
                {
                    if (a.GetPlacement() == CardData.FieldPlacement.Opp && a.myType == Ability.Type.Activate) list.Add(a);
                }
                break;
            case Ability.Type.Character:
                break;
            case Ability.Type.Feat:
                break;
            case Ability.Type.Passive:
                break;
        }
        GM.ShowAbilityList(list);
        list = null;
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
        DisplayCardList(MasterList);
        SpecificDraw = true;
    }

    public void DrawSpecificCard(Card card)
    {
        ToggleCardDisplayArea(false);
        MyHand.Add(card);
        AddCardToHand(card);
        MyDeck.Remove(card);
    }
    public void DrawSpecificCard(Card card, List<Card> whichList)
    {
        ToggleCardDisplayArea(false);
        MyHand.Add(card);
        AddCardToHand(card);
        whichList.Remove(card);
        CardDisplayAreaOffButton.SetActive(false);
    }

    private void ToggleCardDisplayArea(bool toggle)
    {
        CardDisplayArea.parent.gameObject.SetActive(toggle);
    }
    #endregion

    #region DataBase Methods
    
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

        if (myManager.IsMine())
        {
            myManager.RPCRequest("ShareCardList", RpcTarget.Others, "HeroReserve", shareList.ToArray());
        }

        DisplayCardList(HeroReserve);
    }

    public void SetupAbilityDraft()
    {
        if(!AutoDraft)
        {
            GM.PhaseChange(Referee.GamePhase.AbilityDraft);

            DisplayCardList(AbilityDraft);
        }else if(AutoDraft && Referee.player == Referee.PlayerNum.P1)
        {
            foreach(Card card in P1AutoAbilities)
            {
                MyDeck.Add(card);
            }
            if (Referee.myTurn)
            {
                //Debug.Log("SetUpabilityDraft: As player 1, activating EndAbilityDraft for self and others");
                myManager.RPCRequest("EndAbilityDraft", RpcTarget.Others, false);
                EndAbilityDraft(true);
            }
            ToggleCardDisplayArea(false);
        }
        else if(AutoDraft && Referee.player == Referee.PlayerNum.P2)
        {
            foreach (Card card in P2AutoAbilities)
            {
                MyDeck.Add(card);
            }
            ToggleCardDisplayArea(false);
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

    public void RemoveDraftOption( string card)
    {
        foreach(CardData item in Draft)
        {
            if(item.Name == card)
            {
                CatchHeroReserveOversight(card);
                Draft.Remove(item);
                Destroy(item.gameObject);
                //Debug.Log($"Draft: {Draft.Count} - Reserve: {HeroReserve.Count}");
                if (Referee.myTurn)
                {
                    CheckDraft();
                }
                return;
            }
        }
    }

    private void CatchHeroReserveOversight(string card)
    {
        //Debug.Log($"Oversight: {HeroReserve.Count}");
        foreach(Card c in HeroReserve)
        {
            if(c.Name == card)
            {
                RemoveHeroReserve(c);
                //Debug.Log($"Oversight: {HeroReserve.Count}");
                return;
            }
        }
    }

    private void RemoveHeroReserve(Card card)
    {
        //Debug.Log($"Removed {card.Name} from Reserve");
        HeroReserve.Remove(card);
    }

    private void CheckDraft()
    {
        switch (Referee.myPhase)
        {
            case Referee.GamePhase.HeroDraft:
                if(Draft.Count == 12 && Referee.myTurn)
                {
                    int i = UnityEngine.Random.Range(0, 12);
                    Debug.Log($"Draft pick: {i} size: {HeroReserve.Count}");
                    HandleCardCollected(HeroReserve[i], Referee.myPhase);
                    myManager.RPCRequest("SetupAbilityDraft", RpcTarget.All, true);
   
                }
                break;
            case Referee.GamePhase.AbilityDraft:
                if(Draft.Count == 0 && Referee.myTurn)
                {
                    myManager.RPCRequest("EndAbilityDraft", RpcTarget.Others, false);
                    EndAbilityDraft(true);
                }
                break;
        }
    }

    public void ShareCardList(string list, string[] listToShare)
    {
        switch (list)
        {
            case "HeroReserve":
                //Debug.Log("Got a list.");
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

                DisplayCardList(HeroReserve);
                break;
            case "OppHand":
                OppHand.Clear();
                foreach (string cardName in listToShare)
                {
                    foreach (Card card in CardBase)
                    {
                        if (card.Name == cardName)
                        {
                            OppHand.Add(card);
                            break;
                        }
                    }
                }
                GM.SetOpponentHandCount(CardsRemaining(CardDecks.OppHand));
                break;
            case "OppDiscard":
                foreach (string cardName in listToShare)
                {
                    foreach (Card card in CardBase)
                    {
                        if (card.Name == cardName)
                        {
                            OppDiscard.Add(card);
                            break;
                        }
                    }
                }
                break;
        }
    }

    public void EndAbilityDraft(bool told)
    {
        Debug.Log("Initializing the End of the Ability Draft.");
        if (Referee.player == Referee.PlayerNum.P1)
        {
            Debug.Log("Setting first player to Pre - Selection.");
            FillHQ();
            TurnOnGameElements();
            GM.ToldSwitchTurn(true);
            GM.PhaseChange(Referee.GamePhase.HEROSelect);
        }
        else
        {
            Debug.Log("Setting second player to wait.");
            GM.SetDeckNumberAmounts();
            TurnOnGameElements();
            GM.ToldSwitchTurn(false);
            GM.PhaseChange(Referee.GamePhase.Wait);
        }
    }

    private void TurnOnGameElements()
    {
        GM.TurnOnPersonalDeckVisual();
        HandContainer.SetActive(true);
        HandButton.SetActive(true);
        ReserveButtonParentObject.SetActive(true);
    }
    #endregion

    private void DrawFromDiscard()
    {
        DisplayCardList(myDiscard, true);
        CardDisplayAreaOffButton.SetActive(true);
    }

    private void DisplayCardList(List<Card> whichDeck)
    {
        ClearDraft();
        ToggleCardDisplayArea(true);

        foreach(Card card in whichDeck)
        {
            GameObject obj = Instantiate(CardDraftPrefab, CardDisplayArea);
            CardData cd = obj.GetComponent<CardData>();
            if(whichDeck == MyHand)
            {
                cd.CardOverride(card, CardData.FieldPlacement.Hand);
            }else if(whichDeck == myDiscard){
                cd.CardOverride(card, CardData.FieldPlacement.MyDiscard);
            }
            else if (whichDeck == OppDiscard)
            {
                cd.CardOverride(card, CardData.FieldPlacement.OppDiscard);
            }
            else
            {
                cd.CardOverride(card, CardData.FieldPlacement.Draft);
            }
            Draft.Add(cd);
        }
    }

    private void DisplayCardList(List<Card> whichDeck, bool drawable)
    {
        ToggleCardDisplayArea(true);

        foreach (Card card in whichDeck)
        {
            if (aNames.Contains(card.Name)) continue;
            GameObject obj = Instantiate(CardDraftPrefab, CardDisplayArea);
            CardData cd = obj.GetComponent<CardData>();
            if (whichDeck == MyHand)
            {
                cd.CardOverride(card, CardData.FieldPlacement.Hand);
            }
            else if (whichDeck == myDiscard)
            {
                if(drawable) cd.CardOverride(card, CardData.FieldPlacement.Draft);
                else cd.CardOverride(card, CardData.FieldPlacement.MyDiscard);
            }
            else if (whichDeck == OppDiscard)
            {
                cd.CardOverride(card, CardData.FieldPlacement.OppDiscard);
            }
            else
            {
                cd.CardOverride(card, CardData.FieldPlacement.Draft);
            }
            Draft.Add(cd);
        }
        if (Draft.Count == 0) aIsaac.IsaacDraw = false;
    }

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
    }

    private void SetFeatToActiveAbility(Card card)
    {
        string name = card.Name;
        switch (name)
        {
            case "ABSORB":
                activeFeat = GM.gameObject.AddComponent<aAbsorb>();
                //GM.SetActiveAbility((Ability)activeFeat);
                break;
            case "DRAIN":
                activeFeat = GM.gameObject.AddComponent<aDrain>();
                //GM.SetActiveAbility((Ability)activeFeat);
                break;
            case "PAY THE COST":
                activeFeat = GM.gameObject.AddComponent<aPaytheCost>();
                //GM.SetActiveAbility((Ability)activeFeat);
                break;
            case "UNDER SEIGE":
                activeFeat = GM.gameObject.AddComponent<aUnderSiege>();
                //GM.SetActiveAbility((Ability)activeFeat);
                break;
        }
    }

    #region HQ and Reserve
    public int GetHQCount()
    {
        return HQ.Count();
    }
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
                RemoveHeroReserve(HeroReserve[picker]);
            }

            PopulateHQ();
        }
        else
        {
            //Debug.Log($"Reserves: {HeroReserve.Count}, HQ: {HQ.Count}");
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

        myManager.RPCRequest("PopulatedHQ", RpcTarget.Others, temp.ToArray());
        GM.UpdateDeckCounts();
    }

    public void PopulatedHQ(string[] heros)
    {
        if (heros[0] != "None")
        {
            string Names = "I have received; ";
            foreach (string name in heros)
            {
                Names += $" {name},";
            }

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
                RemoveHeroReserve(addCard);
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

        GM.UpdateDeckCounts();
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
                return;
            }
        }

        myManager.RPCRequest("RemoveHQHero", RpcTarget.Others, card.Name);
    }

    public void RemoveHQHero(string cardName)
    {
        foreach (CardData data in HQHeros)
        {
            if (data.Name == cardName)
            {
                HQHeros.Remove(data);
                HQ.Remove(data.myCard);
                Destroy(data.gameObject);
                return;
            }
        }
    }

    public void RemoveHeroFromReserve(string hero)
    {
        foreach(Card card in HeroReserve)
        {
            if(card.Name == hero)
            {
                RemoveHeroReserve(card);
                break;
            }
        }
        if (HeroReserve.Count == 0)
        {
            ReserveButton.interactable = false;
        }
    }

    private void HandlePlayCardFromReserve()
    {
        Card card = HeroReserve[UnityEngine.Random.Range(0, HeroReserve.Count)];
        RemoveHeroReserve(card);
        PlayCard(card);
    }
    #endregion

    #region Hand and Cards

    private void CheckActiveCard()
    {
        int i = MyHand.Count;
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
        if (i != 0) ;
        //GM.CheckHandZoomInEffect();
    }

    private void GetHandToShare()
    {
        List<string> names = new List<string>();
        foreach(Card card in MyHand)
        {
            names.Add(card.Name);
        }
        myManager.RPCRequest("ShareCardList", RpcTarget.Others, "OppHand" ,names.ToArray());
    }

    public void GetOwnDiscardPile()//Via UI
    {
        Debug.Log($"Discard count: {myDiscard.Count}");
        DisplayCardList(myDiscard);
    }

    private CardData FindCardOnField(string name)
    {

        foreach(CardData cd in MyField)
        {
            if(cd.Name == name)
            {
                return cd;
            }
        }

        foreach(CardData cd in OppField)
        {
            if(cd.Name == name)
            {
                return cd;
            }
        }

        Debug.Log($"Didn't find {name}");
        return MyField[0];
    }

    private void HandlePlayCardFromHandSetup()
    {
        if (MyHand.Count > 0)
            DisplayCardList(MyHand);
    }

    public void PlayerSelection(string who)
    {
        if (true)//isBoosting)
        {
            if(who == "Opp")
            {
                Debug.Log("Not implemented.");
                myManager.RPCRequest("DrawXCards", RpcTarget.Others, heroCount);
            }
            else
            {
                Debug.Log("Clicked");
                DrawFromEnhanceDeck(heroCount);
                TogglePlayerSelect(false);
                isBoosting = false;
            }
        }
    }
    private void TogglePlayerSelect(bool toggle)
    {
        BOppInfo.gameObject.SetActive(!toggle);
        BOppSelection.gameObject.SetActive(toggle);
        BMyInfo.gameObject.SetActive(!toggle);
        BMySelection.gameObject.SetActive(toggle);
    }

    #region Drawing and Removing Cards
    private void DrawRandomCard(List<Card> whatDeck)
    {
        var picker = UnityEngine.Random.Range(0, whatDeck.Count - 1);
        Card pickedCard = whatDeck[picker];
        MyHand.Add(pickedCard);
        whatDeck.Remove(whatDeck[picker]);
        AddCardToHand(pickedCard);

        if(whatDeck == HeroReserve)
        {
            myManager.RPCRequest("RemoveHeroFromReserve", RpcTarget.Others, pickedCard.Name);
            if (HeroReserve.Count == 0)
            {
                ReserveButton.interactable = false;
            }
        }
    }

    private void AddCardToHand(Card cardToAdd)
    {
        if(MyHand.Count < 8)
        {
            //Debug.Log($"Adding a card to hand(hand): {MyHand.Count}");
            GameObject obj = Instantiate(CardHandPrefab, Hand[MyHand.Count - 1].transform);
            CardData data = obj.GetComponent<CardData>();
            data.CardOverride(cardToAdd, CardData.FieldPlacement.Hand);
            lHandData.Add(data);
            //Debug.Log($"Adding a card to hand(visual hand): {lHandData.Count}");
            CheckActiveCard();
        }
        GetHandToShare();
        handSize = MyHand.Count;
        GM.ActivatePassive(Ability.PassiveType.HandCardAdjustment);
    }

    private void RemoveCardFromHand(Card cardToRemove)
    {
        //Debug.Log($"Removing a card from hand(hand) before removed: {MyHand.Count}");
        if(MyHand.Count <= 7)
        {
            //Debug.Log($"Hand visual: {lHandData.Count}");
            if(lHandData.Count == MyHand.Count)
            {
                GameObject obj = lHandData[MyHand.Count-1].gameObject;
                lHandData.Remove(lHandData[MyHand.Count-1]);
                Destroy(obj);
            }
            else
            {
                Debug.Log("lHandData doesn't match hand count.");
            }
            //Debug.Log($"Removing a card from hand(visual hand): {lHandData.Count}");

        }
        //Debug.Log($"Removing {cardToRemove.Name}");
        MyHand.Remove(cardToRemove);
        //Debug.Log($"Removing a card from hand(hand) after removed: {MyHand.Count}");
        HandCardOffset(0);
        GetHandToShare();
        handSize = MyHand.Count;
        GM.ActivatePassive(Ability.PassiveType.HandCardAdjustment);
    }

    public void ResetDiscardRecord()
    {
        DiscardedCards.Clear();
    }
    
    public void AddDiscardedCard(Card cardToAdd)
    {
        DiscardedCards.Add(cardToAdd);
    }
    #endregion

    #region Abilitys 
    private void HandleAccelerate()
    {
        //Debug.Log("CB recieved Accelerate.");
        DrawFromEnhanceDeck(3);
        ForceDiscard("Any", 2);
    }
    bool isBoosting = false;
    private void HandleBoost()
    {
        Debug.Log("Activating the discard of two cards for Boost.");
        isBoosting = true;
        ForceDiscard("Any", 2);
        //Need to be able to pick a player??
        //Then need to make them draw a card for the total hero amount on the field
    }
    #endregion

    #region Force Discard
    private void HandleCardForceDiscard(string who, string type, int amount)
    {
        switch (who)
        {
            case "All":
                myManager.RPCRequest("ForceDiscard", RpcTarget.All, type, amount);
                break;
            case "Target":
                myManager.RPCRequest("ForceDiscard", RpcTarget.Others, type, amount);
                break;
        }
    }

    int forceCount = 0;
    public void ForceDiscard(string type, int amount)
    {
        switch (type)
        {
            case "Random":
                for(int i = amount; i>0; i--)
                {
                    if (MyHand.Count == 0) return;
                    RemoveCardFromHand(MyHand[UnityEngine.Random.Range(0, MyHand.Count)]);
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
            case "Any":
                Debug.Log("Setup for Force Discard");
                forceCount = amount;
                isDiscarding = true;
                StartCoroutine(WaitForEndDiscard());
                GM.ToggleDiscard();
                HandButton.SetActive(false);
                DisplayCardList(MyHand);
                break;
        }
    }
    bool isDiscarding = false;
    public void ReduceForceCount()
    {
        forceCount--;
        //Debug.Log($"Force count: {forceCount}");
        if(forceCount <= 0)
        {
            HandButton.SetActive(true);
            ToggleCardDisplayArea(false);
            GM.ToggleDiscard();
            isDiscarding = false;
        }
        else
        {
            DisplayCardList(MyHand);
        }
    }
    private void HandleChooseDiscardCard(Card card)
    {
        if (MyHand.Contains(card))
        {
            //Debug.Log("Card was in hand and being removed.");
            RemoveCardFromHand(card);
            GM.SetCardZoom(false);
            ReduceForceCount();
        }
    }
    #endregion

    #region Card Adjustment
    int RequestCount = 0;
    private void HandleRequestOppHeroStats()
    {
        if(RequestCount <= 0)
        {
            RequestCount++;
            myManager.RPCRequest("HeroStats", RpcTarget.Others, true);
        }
    }
    private void HandleStatsSend(string arg1, int arg2, int arg3)
    {
        myManager.RPCRequest("UpdateHeroStats", RpcTarget.Others, arg1, arg2, arg3);
    }
    public void UpdateHeroFromOpp(string name, int attack, int defense)
    {
        RequestCount--;
        foreach(CardData cd in OppField)
        {
            if(cd.Name == name)
            {
                cd.SetAttack(attack);
                cd.SetDefense(defense);
                return;
            }
        }
    }

    public void SendRequestedHeroStats()
    {
        OnSendHeroStats?.Invoke();
    }

    private void HandleCardAdjustment(CardData cardToAdjust, string category, int newValue)
    {
        //could also set it to specific player
        myManager.RPCRequest("CardAdjustment", RpcTarget.Others, cardToAdjust.Name, category, newValue);
    }

    public void CardAdjustment(string name, string category, int newValue)
    {
        //OppField could be set to a specified player
        bool found = false;
        foreach(CardData data in OppField)
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
            foreach(CardData data in MyField)
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
    }
    #endregion

    #region Card Destroyed
    private void HandleCharacterDestroyed(CardData card)
    {
        GM.ActivatePassive(Ability.PassiveType.CharacterDestroyed);
        cardAbilitiesOnField.Remove(card.charAbility);

        string loc = "";
        if (OppField.Contains(card))
        {
            OppField.Remove(card);
            OppDiscard.Add(card.myCard);
            loc = "OppField";
            foreach(Ability a in card.myAbilities)
            {
                cardAbilitiesOnField.Remove(a);
                AddCardToListByName("OppDiscard", a.Name);
            }
        }else if (MyField.Contains(card))
        {
            MyField.Remove(card);
            loc = "MyField";
            foreach (Ability a in card.myAbilities)
            {
                cardAbilitiesOnField.Remove(a);
                AddCardToListByName("MyDiscard", a.Name);
                AddDiscardedCard(card.myCard);
                AddCardToListByName("DiscardRecord", a.Name);
            }
        }
        card.gameObject.SetActive(false);

        herosFatigued--;
        heroCount--;
        myManager.RPCRequest("FieldCardDestroy", RpcTarget.Others, card.Name, loc);
    }

    private void AddCardToListByName(string list, string name)
    {
        Debug.Log($"Adding {name} to {list}");
        switch (list)
        {
            case "MyDiscard":
                AddCardToListByNameFromArray(name, myDiscard, Abilities);
                break;
            case "DiscardRecord":
                AddCardToListByNameFromArray(name, DiscardedCards, Abilities);
                break;
            case "OppDiscard":
                AddCardToListByNameFromArray(name, OppDiscard, Abilities);
                break;
        }
    }

    private void AddCardToListByNameFromArray(string name, List<Card> theList, Card[] theArray)
    {
        foreach (Card card in theArray)
        {
            if (card.Name == name)
            {
                theList.Add(card);
                return;
            }
        }
    }

    public void FieldCardDestroy(string name, string location)
    {
        switch (location)
        {
            case "MyField":
                foreach(CardData card in OppField)
                {
                    if(card.Name == name)
                    {
                        OppField.Remove(card);
                        OppDiscard.Add(card.myCard);
                        card.gameObject.SetActive(false);//delaying card destroy to carry abilities
                        GM.ActivatePassive(Ability.PassiveType.CharacterDestroyed);
                        break;
                    }
                }
                break;
            case "OppField":
                foreach (CardData card in MyField)
                {
                    if (card.Name == name)
                    {
                        MyField.Remove(card);
                        myDiscard.Add(card.myCard);
                        card.gameObject.SetActive(false);//delaying card destroy to carry abilities
                        GM.ActivatePassive(Ability.PassiveType.CharacterDestroyed);
                        break;
                    }
                }
                break;
        }
        herosFatigued--;
        heroCount--;
    }
    #endregion

    #region Alter Card Exhaust State
    private void HandleCardExhaustState(CardData card, bool exhaust)
    {
        string loc = "";
        if (MyField.Contains(card))
        {
            loc = "MyField";
        }else if (OppField.Contains(card))
        {
            loc = "OppField";
        }


        herosFatigued = exhaust ? herosFatigued + 1 : herosFatigued - 1;
        if (exhaust) GM.ActivatePassive(Ability.PassiveType.HeroFatigued);
        if (!exhaust) GM.ActivatePassive(Ability.PassiveType.HeroHealed);

        Debug.Log($"Heros currently Fatigued: {herosFatigued}");

        myManager.RPCRequest("ExhaustStateAdjust", RpcTarget.Others, card.Name, loc, exhaust);
    }

    public void ExhaustStateAdjust(string name, string location, bool state)
    {
        switch (location)
        {
            case "MyField":
                foreach(CardData card in OppField)
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
            case "OppField":
                foreach (CardData card in MyField)
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
    #endregion

    #region Reveal Target Hand and Remove Non-Heros Ability
    private void HandleRevealTargetHandAndRemoveNonHeros()
    {
        ClearDraft();
        cardListToDisplay = OppHand;
        StartCoroutine(DisplayExtraDraft());
        myManager.RPCRequest("RemoveAllNonHerosFromHand", RpcTarget.Others, "MyHand");
    }

    public void RemoveAllNonHerosFromHand(string hand)
    {
        List<Card> removeList = new List<Card>();

        foreach(Card card in MyHand)
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

    #endregion

    #region Spawn Character, Ability, Enhancement Functions

    public void ConvertCharacter(CardData character)
    {
        //Build new
        CardData data = SpawnAndReturnCharacterToMyField(character.myCard);
        myManager.RPCRequest("SpawnCharacterToOpponentField", RpcTarget.Others, character.Name);
        if (character.Exhausted) data.Exhaust(false);

        //Destroy old
        cardAbilitiesOnField.Remove(character.charAbility);
        OppField.Remove(character);

        foreach (Ability a in character.myAbilities)
        {
            cardAbilitiesOnField.Remove(a);
            AddCardToListByName("OppDiscard", a.Name);
        }
        character.gameObject.SetActive(false);

        myManager.RPCRequest("RemoveConvertedCharacter", RpcTarget.Others, character.Name);
    }
    public void HandleRemoveConvertedCharacter(string character)
    {
        CardData da = null;
        foreach(CardData dat in MyField)
        {
            if(dat.Name == character)
            {
                da = dat;
                break;
            }
        }
        cardAbilitiesOnField.Remove(da.charAbility);
        MyField.Remove(da);
        foreach (Ability a in da.myAbilities)
        {
            cardAbilitiesOnField.Remove(a);
            AddCardToListByName("MyDiscard", a.Name);
        }
        da.gameObject.SetActive(false);
    }

    #region Spawn Character to Field
    public void SpawnCharacterToOpponentField(string heroToSpawn)
    {
        foreach(Card card in Heros)
        {
            if(card.Name == heroToSpawn)
            {
                Debug.Log("spawning a card to field from opponent");
                GameObject obj = Instantiate(CardOppFieldPrefab, OppHeroArea);
                CardData data = obj.GetComponent<CardData>();
                data.CardOverride(card, CardData.FieldPlacement.Opp);
                OppField.Add(data);
                heroCount++;
                GM.ActivatePassive(Ability.PassiveType.CharacterSpawn);
                break;
            }
        }
    }
    private void SpawnCharacterToMyField(Card card)
    {
        GameObject obj = Instantiate(CardMyFieldPrefab, MyHeroArea);
        CardData data = obj.GetComponent<CardData>();
        data.CardOverride(card, CardData.FieldPlacement.Mine);
        MyField.Add(data);
        heroCount++;
        GM.ActivatePassive(Ability.PassiveType.CharacterSpawn);
    }
    private CardData SpawnAndReturnCharacterToMyField(Card card)
    {
        GameObject obj = Instantiate(CardMyFieldPrefab, MyHeroArea);
        CardData data = obj.GetComponent<CardData>();
        data.CardOverride(card, CardData.FieldPlacement.Mine);
        MyField.Add(data);
        heroCount++;
        return data;
    }
    #endregion

    #region Ability Spawning
    private void SpawnAbility(string AbilityName, CardData cardToAttachTo, bool told)
    {
        Debug.Log($"Spawning {AbilityName} on {cardToAttachTo}.");
        Ability a = new Ability();
        switch (AbilityName)
        {
            case "ACCELERATE":
                a = cardToAttachTo.gameObject.AddComponent<aAccelerate>();
                break;
            case "BACKFIRE":
                a = cardToAttachTo.gameObject.AddComponent<aBackfire>();
                break;
            case "BOLSTER":
                a = cardToAttachTo.gameObject.AddComponent<aBolster>();
                break;
            case "BOOST":
                a = cardToAttachTo.gameObject.AddComponent<aBoost>();
                break;
            case "COLLATERAL DAMAGE":
                a = cardToAttachTo.gameObject.AddComponent<aCollateralDamage>();
                break;
            case "CONVERT":
                a = cardToAttachTo.gameObject.AddComponent<aConvert>();
                break;
            case "COUNTER-MEASURES":
                a = cardToAttachTo.gameObject.AddComponent<aCounterMeasures>();
                break;
            case "DROUGHT":
                a = cardToAttachTo.gameObject.AddComponent<aDrought>();
                break;
            case "FORTIFICATION":
                a = cardToAttachTo.gameObject.AddComponent<aFortification>();
                break;
            case "GOING NUCLEAR":
                a = cardToAttachTo.gameObject.AddComponent<aGoingNuclear>();
                break;
            case "HARDENED":
                a = cardToAttachTo.gameObject.AddComponent<aHardened>();
                break;
            case "IMPEDE":
                a = cardToAttachTo.gameObject.AddComponent<aImpede>();
                break;
            case "KAIROS":
                a = cardToAttachTo.gameObject.AddComponent<aKairos>();
                break;
            case "PREVENTION":
                a = cardToAttachTo.gameObject.AddComponent<aPrevention>();
                break;
            case "PROTECT":
                a = cardToAttachTo.gameObject.AddComponent<aProtect>();
                break;
            case "REDUCTION":
                a = cardToAttachTo.gameObject.AddComponent<aReduction>();
                break;
            case "REINFORCEMENT":
                a = cardToAttachTo.gameObject.AddComponent<aReinforcement>();
                break;
            case "RESURRECT":
                a = cardToAttachTo.gameObject.AddComponent<aResurrect>();
                break;
            case "REVELATION":
                a = cardToAttachTo.gameObject.AddComponent<aRevelation>();
                break;
            case "SHIELDING":
                a = cardToAttachTo.gameObject.AddComponent<aShielding>();
                break;
            default:
                Debug.Log($"An ability was requested that doesn't exist. {AbilityName}");
                break;
        }
        cardToAttachTo.AdjustCounter(1, a);
        if (!told)
        {
            myManager.RPCRequest("AttachAbility", RpcTarget.OthersBuffered, AbilityName, cardToAttachTo.Name);
        }
    }

    public void AttachAbility(string abilityName, string cardName)
    {
        Debug.Log($"I was told to attach {abilityName} to {cardName}");

        CardData card = FindCardOnField(cardName);
        SpawnAbility(abilityName, card, true);
    }

    private void HandleAbilitiesGiven(List<Ability> abilities, CardData card)
    {
        List<string> abilityNames = new List<string>();

        foreach(Ability a in abilities)
        {
            myManager.RPCRequest("AttachAbility", RpcTarget.Others, a.Name, card.Name);
        }
    }

    private void HandleAddAbilityToList(Ability ability)
    {
        //Debug.Log($"{ability.Name} was to be added to a list");
        if(ability.myType == Ability.Type.Character)
        {
            heroAbilitiesOnField.Add(ability);
            //Debug.Log($"{ability.Name} ability added to master list.");
        }
        else if(ability.myType == Ability.Type.Activate || ability.myType == Ability.Type.Passive)
        {
            cardAbilitiesOnField.Add(ability);
            //Debug.Log($"{ability.Name} added to master list.");

        }
    }

    private void HandleNeedCopyOfAbility(string Name)
    {
        Ability a = new Ability();
        switch (Name)
        {
            case "AKIO":
                a = new aAkio();
                break;
            case "AYUMI":
                a = new aAyumi();
                break;
            case "BOULOS":
                a = new aBoulos();
                break;
            case "CHRISTOPH":
                a = new aChristoph();
                break;
            case "ENG":
                a = new aEng();
                break;
            case "GAMBITO":
                a = new aGambito();
                break;
            case "GRIT":
                a = new aGrit();
                break;
            case "HINDRA":
                a = new aHindra();
                break;
            case "IGNACIA":
                a = new aIgnacia();
                break;
            case "ISAAC":
                a = new aIsaac();
                break;
            case "IZUMI":
                a = new aIzumi();
                break;
            case "KAY":
                a = new aKay();
                break;
            case "KYAUTA":
                a = new aKyauta();
                break;
            case "MACE":
                a = new aMace();
                break;
            case "MICHAEL":
                a = new aMichael();
                break;
            case "ORIGIN":
                a = new aOrigin();
                break;
            case "ROHAN":
                a = new aRohan();
                break;
            case "YASMINE":
                a = new aYasmine();
                break;
            case "ZHAO":
                a = new aZhao();
                break;
            case "ZOE":
                a = new aZoe();
                break;
            case "ACCELERATE":
                a = new aAccelerate();
                break;
            case "BACKFIRE":
                a = new aBackfire();
                break;
            case "BOLSTER":
                a = new aBolster();
                break;
            case "COLLATERALDAMAGE":
                a = new aCollateralDamage();
                break;
            case "CONVERT":
                a = new aConvert();
                break;
            case "COUNTERMEASURES":
                a = new aCounterMeasures();
                break;
            case "DROUGHT":
                a = new aDrought();
                break;
            case "FORTIFICATION":
                a = new aFortification();
                break;
            case "GOINGNUCLEAR":
                a = new aGoingNuclear();
                break;
            case "HARDENED":
                a = new aHardened();
                break;
            case "IMPEDE":
                a = new aImpede();
                break;
            case "KAIROS":
                a = new aKairos();
                break;
            case "PREVENTION":
                a = new aPrevention();
                break;
            case "PROTECT":
                a = new aProtect();
                break;
            case "REDUCTION":
                a = new aReduction();
                break;
            case "REINFORCEMENT":
                a = new aReinforcement();
                break;
            case "RESURRECTION":
                a = new aResurrect();
                break;
            case "REVELATION":
                a = new aRevelation();
                break;
            case "SHIELDING":
                a = new aShielding();
                break;
        }
    }
    #endregion

    #region Strip Abilities
    private void HandleAbilityStripped(CardData card)
    {
        myManager.RPCRequest("StripAbilities", RpcTarget.Others, card.Name);
    }

    public void StripAbilities(string name)
    {
        FindCardOnField(name).StripAbilities(true);
    }
    #endregion

    #region Strip Enhancements
    private void StripAllEnhancementsOnSideOfField(string side)
    {
        if(side == "OppField")
        {
            foreach(CardData card in OppField)
            {
                card.StripAbilities(false);
                card.StripEnhancements(false);
            }
            return;
        }

        foreach(CardData card in MyField)
        {
            card.StripAbilities(false);
            card.StripEnhancements(false);
        }
    }

    private void HandleEnhancementsStripped(CardData card)
    {
        myManager.RPCRequest("StripEnhancements", RpcTarget.Others, card.Name);
    }

    public void StripEnhancements(string name)
    {
        FindCardOnField(name).StripEnhancements(true);
    }
    #endregion

    #region Gain Enhancements
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
            myManager.RPCRequest("GiveEnhancements", RpcTarget.Others, enhancementNums, card.Name);
        }else if(enhancementNums.Count == 1)
        {
            if(enhancementNums[0][0] > 0)
            {
                myManager.RPCRequest("CardAdjustment", RpcTarget.Others, card.Name, "Attack", enhancementNums[0][0]);
                return;
            }

            myManager.RPCRequest("CardAdjustment", RpcTarget.Others, card.Name, "Defense", enhancementNums[0][1]);
        }
    }

    public void GiveEnhancements(List<int[]> enhancements, string name)
    {
        CardData card = FindCardOnField(name);
        List<Enhancement> en = new List<Enhancement>();
        
        foreach(int[] array in enhancements)
        {
            en.Add(ConvertEnhancementIntArrayToEnhancement(array));
        }

        card.GainEnhancements(en, true);
    }
    #endregion

    private Enhancement ConvertEnhancementIntArrayToEnhancement(int[] array)
    {
        Debug.Log("Converting Int Array into Enhancement");
        Enhancement enhancement = new Enhancement();
        enhancement.attack = array[0];
        enhancement.defense = array[1];

        return enhancement;
    }

    #endregion

    #endregion

    #region Outsource Methods
    public void SetUpHandCardsToBeViewed()
    {
        DisplayCardList(MyHand);
        CardDisplayAreaOffButton.SetActive(true);
    }

    public void RemoveHandCardsDraft()
    {
        ToggleCardDisplayArea(false);
    }

    #region Ability Handover
    public void AbilityHandover(Ability ability)
    {
        Debug.Log($"Handing over control of {ability.Name} to opponent");
        GM.PhaseChange(Referee.GamePhase.Wait);
        myManager.RPCRequest("HandleAbilityHandOver", RpcTarget.Others, ability.Name);
    }
    public void HandleAbilityHandOver(string nameOfAbilityToGiveControl)
    {
        Ability ability = heroAbilitiesOnField[0];
        bool found = false;
        foreach(Ability a in heroAbilitiesOnField)
        {
            if(a.Name == nameOfAbilityToGiveControl)
            {
                ability = a;
                found = true;
                Debug.Log($"Found {ability.Name} in HeroAbilities");
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

        GM.PhaseChange(Referee.GamePhase.TurnResponse);
        GM.SetActiveAbility(ability);
        //GM.PopUpUpdater($"{ability.Name} ability activated.");
    }
    private void AbilityDehandover()
    {
        myManager.RPCRequest("HandleAbilityDehandover", RpcTarget.Others, true);
    }
    public void HandleAbilityDehandover()
    {
        GM.PhaseChange(Referee.GamePhase.Overcome);
        GM.HandleHoldTurn(false);
    }
    #endregion
    
    #region Ability Silence
    private void HandleAbilityToFieldSilence()
    {

        if (Referee.player == Referee.PlayerNum.P1)
        {
            SilenceAbilityToField(Referee.PlayerNum.P2, 1);
            return;
        }
        SilenceAbilityToField(Referee.PlayerNum.P1, 1);
    }

    public void SilenceAbilityToField(Referee.PlayerNum player, int turns)
    {
        switch (player)
        {
            case Referee.PlayerNum.P1:
                myManager.RPCRequest("SilenceAbilityToFieldCall", RpcTarget.Others, "P1", turns);
                break;
            case Referee.PlayerNum.P2:
                myManager.RPCRequest("SilenceAbilityToFieldCall", RpcTarget.Others, "P2", turns);
                break;
            case Referee.PlayerNum.P3:
                myManager.RPCRequest("SilenceAbilityToFieldCall", RpcTarget.Others, "P3", turns);
                break;
            case Referee.PlayerNum.P4:
                myManager.RPCRequest("SilenceAbilityToFieldCall", RpcTarget.Others, "P4", turns);
                break;
        }
    }

    public void SilenceAbilityToFieldCall(string name, int turns)
    {
        Debug.Log("Recieved a message to silence abilities.");
        Referee.PlayerNum num = Referee.PlayerNum.P1;
        switch (name)
        {
            case "P1":
                num = Referee.PlayerNum.P1;
                break;
            case "P2":
                num = Referee.PlayerNum.P2;
                break;
            case "P3":
                num = Referee.PlayerNum.P3;
                break;
            case "P4":
                num = Referee.PlayerNum.P4;
                break;
        }
        if(Referee.player == num)
        {
            GM.SilenceAbilityToField(turns);
        }
    }
    #endregion

    #region Card Actions
    public void PlayCard(Card card)
    {
        //Card should be determined based on type how it will be played.
        switch (card.CardType)
        {
            case Card.Type.Ability:
                //Target a Character
                //Update characters
                /*if (!GM.canPlayAbilitiesToFieldCheck())
                {
                    return;

                }*/
                myManager.RPCRequest("HandlePlayerAction", RpcTarget.Others, false, "");

                OnTargeting?.Invoke(card, true);
                bTargeting = true;
                break;
            case Card.Type.Character:
                //Place Character on the field
                //Spawn a Character on the field
                SpawnCharacterToMyField(card);
                myManager.RPCRequest("SpawnCharacterToOpponentField", RpcTarget.Others, card.Name);
                myManager.RPCRequest("HandlePlayerAction", RpcTarget.Others, false, card.Name);
                break;
            case Card.Type.Enhancement:
                //Target a Character
                //Update character
                myManager.RPCRequest("HandlePlayerAction", RpcTarget.Others, false, card.Name);
                OnTargeting?.Invoke(card, true);
                bTargeting = true;
                break;
            case Card.Type.Feat:
                myManager.RPCRequest("HandlePlayerAction", RpcTarget.Others, false, card.Name);
                SetFeatToActiveAbility(card);
                //GM.ToldSwitchTurn(false);
                //HandleTurnDeclaration(true);
                break;
        }
        if(MyHand.Contains(card))RemoveCardFromHand(card);
    }
    public void HandleCardCollected(Card card, Referee.GamePhase phase)
    {
        switch (phase)
        {
            case Referee.GamePhase.HeroDraft:
                MyHand.Add(card);
                AddCardToHand(card);
                RemoveHeroReserve(card);
                myManager.RPCRequest("RemoveDraftOption", RpcTarget.All, card.Name);
                myManager.RPCRequest("DeclaredTurn", RpcTarget.Others, true);
                GM.ToldSwitchTurn(false);
                break;
            case Referee.GamePhase.AbilityDraft:
                MyDeck.Add(card);
                AbilityDraft.Remove(card);
                myManager.RPCRequest("RemoveDraftOption", RpcTarget.All, card.Name);
                if (Draft.Count != 0)
                {
                    Debug.Log($"Draft count: {Draft.Count}");
                    myManager.RPCRequest("DeclaredTurn", RpcTarget.Others, true);
                    GM.ToldSwitchTurn(false);
                }
                break;
            case Referee.GamePhase.Recruit:
                MyHand.Add(card);
                AddCardToHand(card);
                RemoveHQCard(card);
                GM.ActivatePassive(Ability.PassiveType.HeroRecruited);
                break;
            default:
                if (GM.Rohan) goto case Referee.GamePhase.Recruit;
                break;
        }
    }
    public void HandCardOffset(System.Single offset)
    {
        int o = (int)Math.Floor(offset);
        for(int i = 0; i < lHandData.Count; i++)
        {
            int j = o+i;
            if(j >= MyHand.Count)
            {
                j -= MyHand.Count;
            }
            lHandData[i].CardOverride(MyHand[j], CardData.FieldPlacement.Hand);
        }
        CheckActiveCard();
    }
    public void HandleShowOpponentCard(string name)
    {
        foreach(Card c in CardBase)
        {
            if(c.Name == name)
            {
                StartCoroutine(GM.ShowOpponentPlayedCard(c));
                return;
            }
        }
    }
    #endregion

    #region Card Checks
    public bool CheckIfMyCard(CardData card)
    {
        bool isMyCard = MyField.Contains(card);

        return isMyCard;
    }

    public bool CheckFieldForOpponents()
    {
        if(OppField.Count > 0)
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
        foreach (CardData card in MyField)
        {
            if (!card.Exhausted)
            {
                usable = true;
                break;
            }
        }

            return usable;
    }

    public bool CheckForHerosOnField()
    {
        if (OppField.Count > 0 || MyField.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CheckForHealableHeros()
    {
        bool healable = false;

        foreach(CardData card in MyField)
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

        foreach(Card card in MyHand)
        {
            if(card.CardType == Card.Type.Feat)
            {
                return true;
            }
        }

        return haveCard;
    }

    public int CardsRemaining(CardDecks Deck)
    {
        int i = 0;
        switch (Deck)
        {
            case CardDecks.HQ:
                i = HeroReserve.Count;
                break;
            case CardDecks.MyDeck:
                i = MyDeck.Count;
                break;
            case CardDecks.MyHand:
                i = MyHand.Count;
                break;
            case CardDecks.MyDiscard:
                i = myDiscard.Count;
                break;
            case CardDecks.OppHand:
                i = OppHand.Count;
                break;
            case CardDecks.Reserve:
                i = HeroReserve.Count;
                break;
        }
        return i;
    }
    #endregion
    
    #region Drawing Cards
    void DrawFromEnhanceDeck(int amount)
    {
        for(int i =0; i < amount; i++)
        {
            DrawCard(CardDecks.MyDeck);
            GM.UpdateDeckCounts();
        }
    }
    public void DrawCard(CardDecks Deck)
    {
        switch (Deck)
        {
            case CardDecks.MyDeck:
                DrawRandomCard(MyDeck);
                break;
        }
    }
    public void DrawReserveCard()
    {
        if(Referee.myTurn && (Referee.myPhase == Referee.GamePhase.Recruit || GM.Rohan))
        {
            DrawRandomCard(HeroReserve);
            GM.TurnCounterDecrement();
            if(GM.GetTurnCounter() == 0)
            {
                ReserveButton.interactable = false;
            }
            GM.ActivatePassive(Ability.PassiveType.HeroRecruited);
        }
    }
    #endregion

    #region Send Previous Attackers and Defender
    public void SendPreviousAttackersAndDefender(List<CardData> attackers, CardData defender)
    {
        aNames.Clear();

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

        myManager.RPCRequest("HandlePreviousAttackersAndDefender", RpcTarget.Others, aNames.ToArray());
    }

    public void HandlePreviousAttackersAndDefender(string[] names)
    {
        //Debug.Log("Made it into the server HandlePreviousAttackersAndDefender.");
        List<CardData> cards = new List<CardData>();

        aNames = names.ToList();

        foreach(CardData data in OppField)
        {
            if (names.Contains(data.Name))
            {
                cards.Add(data);
            }
        }
        Referee.PreviousAttackers = cards;

        if(names[names.Length-1] != "NULL")
        {
            foreach(CardData data in MyField)
            {
                if(data.Name == names[names.Length - 1])
                {
                    Referee.PreviousDefender = data;
                    if (data.Name == "YASMINE") OnSendYasmineAbilityRequest?.Invoke("Yasmine");
                    if (data.Name == "ZHAO") OnZhaoAbilityRequest?.Invoke("Zhao");
                    break;
                }
            }
        }
    }
    #endregion

    public void GetHeroModifiedCount()
    {
        herosModified = 0;
        OnRequestHeroModified?.Invoke();
    }
    private void HandleHeroModifiedCensus(bool value)
    {
        if (value) herosModified++;
    }

    #endregion

    #region IEnumerators
    private IEnumerator DisplayExtraDraft()
    {
        DisplayCardList(cardListToDisplay);
        Debug.Log("New Card list displayed.");
        yield return new WaitForSeconds(5f);
        ToggleCardDisplayArea(false);
        //GM.ToldSwitchTurn(false);
        myManager.RPCRequest("DeclaredTurn", RpcTarget.Others, true);
    }
    private IEnumerator WaitForEndDiscard()
    {
        yield return new WaitUntil(() => !isDiscarding);
        Debug.Log("Stopped the discard sequence.");
        if (isBoosting)
        {
            TogglePlayerSelect(true);
            GM.PopUpUpdater("Select target player.", 2f);
        }
    }
    #endregion
}
