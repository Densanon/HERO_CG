using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using System;

public class CardDataBase : MonoBehaviour
{
    PhotonView PV;

    public enum CardDecks { P1Hand, P1Deck, P1Field, P1Discard, P2Hand, P2Deck, P2Field, P2Discard, Reserve, HQ}

    public GameObject CardHandPrefab;
    public GameObject CardDraftPrefab;
    public GameObject[] Hand = new GameObject[7];
    private List<CardData> lHandData = new List<CardData>();
    public Slider sHandSlider;
    public List<CardData> Draft = new List<CardData>();
    public Transform DraftArea;
    public PhotonGameManager GM;
    public CardData CurrentActiveCard;

    #region Debuging
    public bool autoDraft = false;
    #endregion

    public static Action<bool> OnTurnDelcarationReceived = delegate { };
    public static Action<Card> OnAutoDraftCollected = delegate { };
 
    #region Card Data Base
    [SerializeField] private Sprite[] HeroImages = new Sprite[20];
    [SerializeField] private Sprite[] AbilityImages = new Sprite[20];
    [SerializeField] private Sprite[] EnhanceImages = new Sprite[4];
    [SerializeField] private Sprite[] FeatImages = new Sprite[4];

    Card[] Heros = new Card[20];
    Card[] Abilities = new Card[20];
    Card[] Enhancements = new Card[4];
    Card[] Feats = new Card[4];

    Card[] CardBase = new Card[48];
    #endregion

    #region Dynamic card lists
    List<Card> HeroSelection = new List<Card>();
    List<Card> HeroReserve = new List<Card>();
    List<Card> AbilityDraft = new List<Card>();

    List<Card> P1Hand = new List<Card>();
    List<Card> P1Field = new List<Card>();
    List<Card> P1Deck = new List<Card>();
    List<Card> P1Discard = new List<Card>();

    List<Card> P2Hand = new List<Card>();
    List<Card> P2Field = new List<Card>();
    List<Card> P2Deck = new List<Card>();
    List<Card> P2Discard = new List<Card>();
    #endregion

    #region Unity Methods
    private void Awake()
    {
        PV = GetComponent<PhotonView>();

        Heros[0] = new Card(Card.Type.Character, "AKIO", 20, 70, "<< Societal freedom is a mirage for true hope. >>", "(P) When Akio causes another hero to be fatigued, Akio is not fatigued. If Akio attacks and causes the hero he attacks to become fatigued, Akio is not fatigued and may continue attacking.", HeroImages[0]);
        Heros[1] = new Card(Card.Type.Character, "AYUMI", 40, 50, "<< The River will meet all your needs. >>", "(P) Whenever a hero is recruited, you may draw a card from your Enhancement Deck. If a player takes a Recruit Action and they choose to recruit 2 heroes, you may draw 2 cards.", HeroImages[1]);
        Heros[2] = new Card(Card.Type.Character, "BOULOS",90, 0, "<< The dystopic is our past, present, and future--yet there is still hope. >>", "For each card in your hand, Boulos gains +10 defense.", HeroImages[2]);
        Heros[3] = new Card(Card.Type.Character, "CHRISTOPH", 30, 60, "<< Vraiment? Encore? Ok, je pensais qu'ils retiendraient la le(c)on au bout d'un moment. >>", " (P) When an attack against Christoph is resolved, you may choose one of the attacking heroes to be defeated.", HeroImages[3]);
        Heros[4] = new Card(Card.Type.Character, "ENG", 10, 80, "<< CHINESE >>", "(A) After an Action, heal one hero.", HeroImages[4]);
        Heros[5] = new Card(Card.Type.Character, "GAMBITO", 50, 40, "<< A veces, no hay negociacion. >>", "(P) When any hero is fatigued, all players must discard a random card from their hand.", HeroImages[5]);
        Heros[6] = new Card(Card.Type.Character, "GRIT", 0 , 90, "<< Broken clay must be revived before it can be used to create. >>", "(P) Grit may gain +20 attack for every fatigued hero. For every single hero on the field in a fatigued position before the start of the Attack, Grit gains +20 attack.", HeroImages[6]);
        Heros[7] = new Card(Card.Type.Character, "HINDRA",70, 20, "<< You're missing the point here. There's so much more to Ghostwalking than just jumping on air. >>", "(A) Choose one opponent to prevent from playing Ability Cards to the field on their next turn.", HeroImages[7]);
        Heros[8] = new Card(Card.Type.Character, "IGNACIA", 20, 70, "<< You are capable of greatness. Capable of shifting the course of history. Capable of bringing loife to the dead. >>", "(A) Ignacia may attack once per turn while fatigued (any strengthened allies may join the attack and become fatigued).", HeroImages[8]);
        Heros[9] = new Card(Card.Type.Character, "ISAAC", 40, 50, "(Hebrew)", "(P) When another hero is defeated, you may draw a card from your Discard Pile that was there before the hero was defeated.", HeroImages[9]);
        Heros[10] = new Card(Card.Type.Character, "IZUMI", 10, 80, "<< Don't Worry. I'm not going anywhere. >>", "(P) Allied heroes may gain +20 defense while Izumi is in play. Fatigued heroes gain this after their defense is halved.", HeroImages[10]);
        Heros[11] = new Card(Card.Type.Character, "KAY", 50, 40, "<< Are you ready yet? >>", "(A) Play a card to the field from your hand.", HeroImages[11]);
        Heros[12] = new Card(Card.Type.Character, "KYAUTA", 90, 0, "<< Yare daye basai isa ba. >>", "(A) After fully completing the Action you chose for your turn, fatigue one of your stengthened heroes. Then, recruit one hero directly to your play area from the top of the Reserves pile.", HeroImages[12]);
        Heros[13] = new Card(Card.Type.Character, "MACE", 0, 90, "<< A pillar is only as strong as it's foundation. >>", "(P) You may double the Total Attack for one attack that occurs on your turn.", HeroImages[13]);
        Heros[14] = new Card(Card.Type.Character, "MICHAEL", 80, 10, "<< A place of rest is a geyser of drive. >>", "(A) Draw a card from your Enhancement Deck to your hand.", HeroImages[14]);
        Heros[15] = new Card(Card.Type.Character, "ORIGIN", 80, 10, "<< Let's just say, I've been a mother for a very long time. >>", "(P) During each opponent’s turn, you may block one attack against Origin.", HeroImages[15]);
        Heros[16] = new Card(Card.Type.Character, "ROHAN", 70,20, "<< If you step on board the Redux, you can be sure that there will be storms ahead. >>", "(A) For each fatigued hero, recruit one hero from Hero HQ or Reserves to your hand.", HeroImages[16]);
        Heros[17] = new Card(Card.Type.Character, "YASMINE", 60, 30, "<< (Egyptian?) >>", "(P) When an attack against Yasmine is resolved, you may play a card to the field.", HeroImages[17]);
        Heros[18] = new Card(Card.Type.Character, "ZHAO", 70, 20, "<< Time to jumpstart this fallen world. >>", "(P) As long as Zhao is in play, all Allied heroes may gain +10 attack. (P) When Zhao is defeated/removed, you can play a card from your hand to the field.", HeroImages[18]);
        Heros[19] = new Card(Card.Type.Character, "ZOE", 50, 40, "<< (Hebrew) >>", "(A) Before performing an Action, heal any hero(es) of your choice.", HeroImages[19]);

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

        for(int i = 0; i < 48; i++)
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
    #endregion


    #region Draft Methods
    public void HandleBuildHeroDraft()
    {
        Debug.Log("I am supposed to handle the Hero Draft.");
        List<Card> tempHero = Heros.ToList();
        List<string> shareList = new List<string>();
        for (int i = 0; i < 14; i++)
        {
            var picker = UnityEngine.Random.Range(0, tempHero.Count);
            HeroSelection.Add(tempHero[picker]);
            shareList.Add(tempHero[picker].Name);
            tempHero.Remove(tempHero[picker]);
        }

        if (PV.IsMine)
        {
            PV.RPC("ShareCardList", RpcTarget.Others, "HeroSelection", shareList.ToArray());
        }

        DisplayDraft(HeroSelection);
    }

    private void DisplayDraft(List<Card> whichDeck)
    {
        DraftArea.gameObject.SetActive(true);

        Debug.Log($"Hero Count: {whichDeck.Count}");
        foreach(Card card in whichDeck)
        {
            GameObject obj = Instantiate(CardDraftPrefab, DraftArea);
            CardData cd = obj.GetComponent<CardData>();
            cd.CardOverride(card);
            Draft.Add(cd);
        }
        Debug.Log($"Hero Draft Count: {Draft.Count}");
    }

    [PunRPC]
    private void SetupAbilityDraft(bool yes)
    {
        for (int i = Draft.Count -1; i > -1; i--)
        {
            var item = Draft[i];
            Draft.Remove(item);
            Destroy(item.gameObject);
        }

        GM.PhaseChange(PhotonGameManager.GamePhase.AbilityDraft);

        DisplayDraft(AbilityDraft);
    }

    [PunRPC]
    private void RemoveDraftOption( string card)
    {
        foreach(CardData item in Draft)
        {
            if(item.Name == card)
            {
                Debug.Log($"Removing {item.Name} from the draft.");
                Draft.Remove(item);
                Destroy(item.gameObject);
                Debug.Log($"Cards Remaining: {Draft.Count}");
                CheckDraft();
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
                    
                        HandleCardCollected(HeroSelection[UnityEngine.Random.Range(0, 13)], PhotonGameManager.myPhase);
                        PV.RPC("SetupAbilityDraft", RpcTarget.All, true);
   
                }
                break;
            case PhotonGameManager.GamePhase.AbilityDraft:
                if(Draft.Count == 0 && PhotonGameManager.myTurn)
                {
                    Debug.Log("The Ability draft is over!");
                    PV.RPC("EndAbilityDraft", RpcTarget.AllBufferedViaServer, true);
                }
                break;
        }
    }

    [PunRPC]
    private void ShareCardList(string list, string[] listToShare)
    {
        switch (list)
        {
            case "HeroSelection":
                foreach (string name in listToShare)
                {
                    foreach (Card card in Heros)
                    {
                        if (name == card.Name)
                        {
                            HeroSelection.Add(card);
                        }
                    }
                }

                DisplayDraft(HeroSelection);
                break;
            case "P2Hand":
                foreach(string cardName in listToShare)
                {
                    foreach(Card card in CardBase)
                    {
                        if(card.Name == cardName)
                        {
                            P2Hand.Add(card);
                            break;
                        }
                    }
                }
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
    private void EndAbilityDraft(bool yes)
    {
        GM.PhaseChange(PhotonGameManager.GamePhase.HEROSelect);
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
        Debug.Log("I have received a HandleTurnDeclaration RPC.");
        GM.SwitchTurn(myTurn);
    }
    #endregion

    #region Private Methods
    #region Debugging
    public void SetAutoDraft(bool set)
    {
        autoDraft = set;
    }

    public void DrawDraftCard(string draftDeck)
    {
        switch (draftDeck)
        {
            case "HeroSelection":
                Debug.Log("Picking random card for Auto hero draw.");
                OnAutoDraftCollected?.Invoke(HeroSelection[UnityEngine.Random.Range(0, HeroSelection.Count)]);
                break;
            case "AbilityDraft":
                Debug.Log("Picking random card for Auto ability draw.");
                OnAutoDraftCollected?.Invoke(AbilityDraft[UnityEngine.Random.Range(0, AbilityDraft.Count)]);
                break;
        }
    }
    #endregion

    private void UpdateHandSlider()
    {
        sHandSlider.maxValue = P1Hand.Count;
    }

    private void FillReserves()
    {
        if(HeroSelection.Count > 0 && HeroReserve.Count < 3)
        {
            for(int i = HeroReserve.Count; i < 3; i++)
            {
                int picker = UnityEngine.Random.Range(0, HeroSelection.Count);
                HeroReserve.Add(HeroSelection[picker]);
                HeroSelection.Remove(HeroSelection[picker]);
            }
        }

        PopulateReserve();
    }

    private void PopulateReserve()
    {
        //Take the container
        //Add new heros
    }

    private void DrawRandomCard(List<Card> whatDeck)
    {
        var picker = UnityEngine.Random.Range(0, whatDeck.Count - 1);
        Card pickedCard = whatDeck[picker];
        P1Hand.Add(pickedCard);
        UpdateHandSlider();
        whatDeck.Remove(whatDeck[picker]);
        AddCardToHand(pickedCard);
    }

    private void AddCardToHand(Card cardToAdd)
    {
        if(P1Hand.Count < 7)
        {
            GameObject obj = Instantiate(CardHandPrefab, Hand[P1Hand.Count - 1].transform);
            CardData data = obj.GetComponent<CardData>();
            data.CardOverride(cardToAdd);
            lHandData.Add(data);
            CheckActiveCard();
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
        Debug.Log($"ActiveCardCheck: {CurrentActiveCard.Name}");
    }
    #endregion

    #region Public Methods
    public void HandCardOffset(int offset)
    {
        for(int i = 0; i < P1Hand.Count; i++)
        {
            int j = offset+i;
            if(j >= P1Hand.Count)
            {
                j -= P1Hand.Count;
            }
            lHandData[i].CardOverride(P1Hand[j]);
        }
        CheckActiveCard();
    }

    public void DrawCard(CardDecks Deck)
    {
        switch (Deck)
        {
            case CardDecks.P1Hand:
                Debug.Log("Drawing a card from P1Hand.");
                DrawRandomCard(P1Hand);
                break;
            case CardDecks.HQ:
                DrawRandomCard(HeroSelection);
                break;
        }
    }

    public void DrawCard(CardDecks Deck, Card hero)
    {
        switch (Deck)
        {
            case CardDecks.Reserve:
                break;
        }

        foreach (Card item in HeroReserve)
        {
            if (item.Name == hero.Name)
            {
                Debug.Log($"Removing {item.Name}.");
                P1Hand.Add(item);
                UpdateHandSlider();
                HeroReserve.Remove(item);
                AddCardToHand(item);
                break;
            }
        }
    }

    public int CardsRemaining(CardDecks Deck)
    {
        int i = 0;
        switch (Deck)
        {
            case CardDecks.HQ:
                i = HeroSelection.Count;
                break;
            case CardDecks.P1Deck:
                i = P1Deck.Count;
                break;
            case CardDecks.P1Discard:
                i = P1Discard.Count;
                break;
        }
        return i;
    }

    public void HandleCardCollected(Card card, PhotonGameManager.GamePhase phase)
    {
        switch (phase)
        {
            case PhotonGameManager.GamePhase.HeroDraft:
                Debug.Log($"Removing {card.Name} from the Draft and adding it to my hand.");
                P1Hand.Add(card);
                UpdateHandSlider();
                AddCardToHand(card);
                HeroSelection.Remove(card);
                PV.RPC("RemoveDraftOption", RpcTarget.All, card.Name);
                Debug.Log($"{P1Hand.Count} cards in my hand.");
                break;
            case PhotonGameManager.GamePhase.AbilityDraft:
                Debug.Log($"Removing {card.Name} from the Draft and adding it to my Enhancement deck.");
                P1Deck.Add(card);
                UpdateHandSlider();
                AbilityDraft.Remove(card);
                PV.RPC("RemoveDraftOption", RpcTarget.All, card.Name);
                Debug.Log($"{P1Deck.Count} cards in my deck.");
                break;
        }
    }
    #endregion
}
