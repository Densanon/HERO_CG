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
    public GameObject CardMyFieldPrefab;
    public GameObject CardOppFieldPrefab;
    public GameObject CardHeroHQPrefab;
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

    #region Debuging
    public bool AiDraft = false;
    public bool AutoDraft = false;
    #endregion

    public static Action<bool> OnTurnDelcarationReceived = delegate { };
    public static Action<Card> OnAiDraftCollected = delegate { };
    public static Action<Card> OnTargeting = delegate { };
 
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
    List<Card> HeroReserve = new List<Card>();
    List<Card> HQ = new List<Card>();
    List<Card> AbilityDraft = new List<Card>();

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
    #endregion

    #region Unity Methods
    private void Awake()
    {
        PV = GetComponent<PhotonView>();

        UIConfirmation.OnTargetAccepted += HandleTargetAccepted;
        CardData.OnNumericAdjustment += HandleCardAdjustment;

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
        if(!AutoDraft)
        {
            for (int i = Draft.Count - 1; i > -1; i--)
            {
                var item = Draft[i];
                Draft.Remove(item);
                Destroy(item.gameObject);
            }

            GM.PhaseChange(PhotonGameManager.GamePhase.AbilityDraft);

            DisplayDraft(AbilityDraft);
        }else if(AutoDraft && PhotonGameManager.player == PhotonGameManager.PlayerNum.P1)
        {
            Debug.Log("I am setting up abilities as player1");
            foreach(Card card in P1AutoAbilities)
            {
                P1Deck.Add(card);
            }
            if (PhotonGameManager.myTurn)
            {
                PV.RPC("EndAbilityDraft", RpcTarget.All, true);
            }
            DraftArea.gameObject.SetActive(false);
        }
        else if(AutoDraft && PhotonGameManager.player == PhotonGameManager.PlayerNum.P2)
        {
            Debug.Log("I am setting up abilities as player2");
            foreach (Card card in P2AutoAbilities)
            {
                P1Deck.Add(card);
            }
            if (PhotonGameManager.myTurn)
            {
                PV.RPC("EndAbilityDraft", RpcTarget.All, true);
            }
            DraftArea.gameObject.SetActive(false);
        }
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
                    
                        HandleCardCollected(HeroReserve[UnityEngine.Random.Range(0, 13)], PhotonGameManager.myPhase);
                        PV.RPC("SetupAbilityDraft", RpcTarget.All, true);
   
                }
                break;
            case PhotonGameManager.GamePhase.AbilityDraft:
                if(Draft.Count == 0 && PhotonGameManager.myTurn)
                {
                    Debug.Log("The Ability draft is over!");
                    PV.RPC("EndAbilityDraft", RpcTarget.All, true);
                }
                break;
        }
    }

    [PunRPC]
    private void ShareCardList(string list, string[] listToShare)
    {
        Debug.Log($"{list}: Sent over.");
        switch (list)
        {
            case "HeroReserve":
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
                foreach(string cardName in listToShare)
                {
                    foreach(Card card in CardBase)
                    {
                        if(card.Name == cardName)
                        {
                            Debug.Log($"Received {card.Name}.");
                            P2Hand.Add(card);
                            break;
                        }
                    }
                }
                Debug.Log($"{P2Hand.Count} cards in opponents hand.");
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
    private void EndAbilityDraft(bool yes)
    {
        if (PhotonGameManager.myTurn)
        {
            GM.PhaseChange(PhotonGameManager.GamePhase.HEROSelect);
        }
        else
        {
            GM.PhaseChange(PhotonGameManager.GamePhase.Wait);
        }
        FillHQ();
    }
    #endregion

    #region Turn Declaration
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
                Debug.Log("Picking random card for Auto hero draw.");
                OnAiDraftCollected?.Invoke(HeroReserve[UnityEngine.Random.Range(0, HeroReserve.Count)]);
                break;
            case "AbilityDraft":
                Debug.Log("Picking random card for Auto ability draw.");
                OnAiDraftCollected?.Invoke(AbilityDraft[UnityEngine.Random.Range(0, AbilityDraft.Count)]);
                break;
        }
    }
    #endregion

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
        RemoveCardFromHand(cardToUse);
        OnTargeting(cardToUse);
        GM.TurnCounterDecrement();
        if (GM.GetTurnCounter() == 0)
        {
            GM.SwitchTurn(false);
            HandleTurnDeclaration(true);
            if (PhotonGameManager.myPhase != PhotonGameManager.GamePhase.Wait)
            {
                GM.PhaseChange(PhotonGameManager.GamePhase.Wait);
            }
        }
    }

    private void HandleCardAdjustment(CardData cardToAdjust, string category, int newValue)
    {
        //could also set it to specific player
        Debug.Log("Sending A Card Adjustment Request.");
        PV.RPC("CardAdjustment", RpcTarget.OthersBuffered, cardToAdjust.Name, category, newValue);
    }

    [PunRPC]
    private void CardAdjustment(string name, string category, int newValue)
    {
        //P2Field could be set to a specified player
        Debug.Log($"Looking for {name} to adjust {category} to {newValue}.");
        bool found = false;
        foreach(CardData data in P2Field)
        {
            Debug.Log($"Checking to see if {data.Name} matches {name}");
            if(data.Name == name)
            {
                Debug.Log($"Found a match {data.Name}, in my opponents Field");
                switch (category)
                {
                    case "Attack":
                        Debug.Log("Adjusting the Attack.");
                        data.SetAttack(newValue);
                        break;
                    case "Defense":
                        Debug.Log("Adjusting the Defense.");
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
                    Debug.Log($"Found a match {data.Name}, in my Field");
                    switch (category)
                    {
                        case "Attack":
                            Debug.Log("Adjusting the Attack.");
                            data.SetAttack(newValue);
                            break;
                        case "Defense":
                            Debug.Log("Adjusting the Defense.");
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

    private void UpdateHandSlider()
    {
        if(P1Hand.Count != 0)
        {
            sHandSlider.maxValue = P1Hand.Count-1;
        }
        else
        {
            sHandSlider.maxValue = 0;
        }
    }

    private void FillHQ()
    {
        if(HeroReserve.Count > 0 && HQ.Count < 3)
        {
            for(int i = HQ.Count; i < 3; i++)
            {
                int picker = UnityEngine.Random.Range(0, HeroReserve.Count);
                HQ.Add(HeroReserve[picker]);
                HeroReserve.Remove(HeroReserve[picker]);
            }
        }

        PopulateHQ();
    }

    private void RemoveHQCard(Card card)
    {
        foreach(CardData data in HQHeros)
        {
            if(data.Name == card.Name)
            {
                HQHeros.Remove(data);
                Destroy(data.gameObject);
                break;
            }
        }
    }

    private void PopulateHQ()
    {
        //Take the container
        //Empty
        //Add new heros

        foreach(CardData data in HQHeros)
        {
            HQHeros.Remove(data);
            Destroy(data.gameObject);
        }
        foreach(Card card in HQ)
        {
            GameObject obj = Instantiate(CardHeroHQPrefab, HQArea);
            CardData data = obj.GetComponent<CardData>();
            data.CardOverride(card);
            HQHeros.Add(data);
        }
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
        GetHandToShare();
    }

    private void RemoveCardFromHand(Card cardToRemove)
    {
        P1Hand.Remove(cardToRemove);
        foreach(CardData data in lHandData)
        {
            if(data.myCard == cardToRemove)
            {
                lHandData.Remove(data);
                if(P1Hand.Count < 7)
                {
                    Destroy(data.gameObject);
                }
                break;
            }
        }
        UpdateHandSlider();
        CheckActiveCard();
        GetHandToShare();
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
        GM.CheckHandZoomInEffect();
        Debug.Log($"ActiveCardCheck: {CurrentActiveCard.Name}");
    }

    private void GetHandToShare()
    {
        List<string> names = new List<string>();
        foreach(Card card in P1Hand)
        {
            Debug.Log($"Handing over: {card.Name}");
            names.Add(card.Name);
        }
        PV.RPC("ShareCardList", RpcTarget.Others, "P2Hand" ,names.ToArray());
    }

    [PunRPC]
    private void SpawnCharacterToOpponentField(string heroToSpawn)
    {
        foreach(Card card in Heros)
        {
            if(card.Name == heroToSpawn)
            {
                GameObject obj = Instantiate(CardOppFieldPrefab, OppHeroArea);
                CardData data = obj.GetComponent<CardData>();
                data.CardOverride(card);
                P2Field.Add(data);
                break;
            }
        }
    }

    private void SpawnCharacterToMyField(Card card)
    {
        GameObject obj = Instantiate(CardMyFieldPrefab, MyHeroArea);
        CardData data = obj.GetComponent<CardData>();
        data.CardOverride(card);
        P1Field.Add(data);
    }

    private void SpawnAbility(string AbilityName, CardData cardToAttachTo, bool told)
    {
        Debug.Log($"Spawning {AbilityName} on {cardToAttachTo}.");
        Component comp = new Component();
        switch (AbilityName)
        {
            case "ACCELERATE":
                comp = cardToAttachTo.gameObject.AddComponent<aAccelerate>();
                break;
            case "BACKFIRE":
                comp = cardToAttachTo.gameObject.AddComponent<aBackfire>();
                break;
            case "BOLSTER":
                comp = cardToAttachTo.gameObject.AddComponent<aBolster>();
                break;
            case "BOOST":
                comp = cardToAttachTo.gameObject.AddComponent<aBoost>();
                break;
            case "COLLATERAL DAMAGE":
                comp = cardToAttachTo.gameObject.AddComponent<aCollateralDamage>();
                break;
            case "CONVERT":
                comp = cardToAttachTo.gameObject.AddComponent<aConvert>();
                break;
            case "COUNTER-MEASURES":
                comp = cardToAttachTo.gameObject.AddComponent<aCounterMeasures>();
                break;
            case "DROUGHT":
                comp = cardToAttachTo.gameObject.AddComponent<aDrought>();
                break;
            case "FORTIFICATION":
                comp = cardToAttachTo.gameObject.AddComponent<aFortification>();
                break;
            case "GOING NUCLEAR":
                comp = cardToAttachTo.gameObject.AddComponent<aGoingNuclear>();
                break;
            case "HARDENED":
                comp = cardToAttachTo.gameObject.AddComponent<aHardened>();
                break;
            case "IMPEDE":
                comp = cardToAttachTo.gameObject.AddComponent<aImpede>();
                break;
            case "KAIROS":
                comp = cardToAttachTo.gameObject.AddComponent<aKairos>();
                break;
            case "PREVENTION":
                comp = cardToAttachTo.gameObject.AddComponent<aPrevention>();
                break;
            case "PROTECT":
                comp = cardToAttachTo.gameObject.AddComponent<aProtect>();
                break;
            case "REDUCTION":
                comp = cardToAttachTo.gameObject.AddComponent<aReduction>();
                break;
            case "REINFORCEMENT":
                comp = cardToAttachTo.gameObject.AddComponent<aReinforcement>();
                break;
            case "RESURRECT":
                comp = cardToAttachTo.gameObject.AddComponent<aResurrect>();
                break;
            case "REVELATION":
                comp = cardToAttachTo.gameObject.AddComponent<aRevelation>();
                break;
            case "SHEILDING":
                comp = cardToAttachTo.gameObject.AddComponent<aShielding>();
                break;
            default:
                Debug.Log($"An ability was requested that doesn't exist. {AbilityName}");
                break;
        }
        cardToAttachTo.AdjustCounter(1, comp);
        if (!told)
        {
            PV.RPC("AttachAbility", RpcTarget.OthersBuffered, AbilityName, cardToAttachTo.Name);
        }
    }

    [PunRPC]
    private void AttachAbility(string abilityName, string cardName)
    {
        Debug.Log($"I was told to attach {abilityName} to {cardName}");
        bool found = false;
        foreach(CardData data in P2Field)
        {
            if(data.Name == cardName)
            {
                SpawnAbility(abilityName, data, true);
                found = true;
                break;
            }
        }
        if (!found)
        {
            Debug.Log($"Searching my field for {cardName}");
            foreach(CardData data in P1Field)
            {
                Debug.Log($"Found {data.Name}");
                if(data.Name == cardName)
                {
                    SpawnAbility(abilityName, data, true);
                    break;
                }
            }
        }
    }
    #endregion

    #region Public Methods
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
            lHandData[i].CardOverride(P1Hand[j]);
        }
        CheckActiveCard();
    }

    public void PlayCard(Card card)
    {
        //Card should be determined based on type how it will be played.
        bool endTurn = false;
        switch (card.CardType)
        {
            case Card.Type.Ability:
                //Target a Character
                //Update character
                //Count move down
                OnTargeting?.Invoke(card);
                break;
            case Card.Type.Character:
                //Place Character on the field
                //Spawn a Character on the field
                Debug.Log($"Setting a Character card on the field: {card.Name}.");
                SpawnCharacterToMyField(card);
                PV.RPC("SpawnCharacterToOpponentField", RpcTarget.OthersBuffered, card.Name);
                RemoveCardFromHand(card);
                GM.TurnCounterDecrement();
                if (GM.GetTurnCounter() == 0)
                {
                    endTurn = true;
                }
                break;
            case Card.Type.Enhancement:
                //Target a Character
                //Update character
                //Count move down
                //If move amount is up, end turn
                OnTargeting?.Invoke(card);
                break;
            case Card.Type.Feat:
                //Resolve Feat ability
                endTurn = true;
                break;
        }
        if (endTurn)
        {
            GM.SwitchTurn(false);
            HandleTurnDeclaration(true);
            if(PhotonGameManager.myPhase != PhotonGameManager.GamePhase.Wait)
            {
                GM.PhaseChange(PhotonGameManager.GamePhase.Wait);
            }
        }
    }

    public void DrawCard(CardDecks Deck)
    {
        switch (Deck)
        {
            case CardDecks.P1Deck:
                Debug.Log("Drawing a card from P1Deck.");
                DrawRandomCard(P1Deck);
                break;
            case CardDecks.Reserve:
                DrawRandomCard(HeroReserve);
                break;
        }
    }

    public void DrawCard()
    {
        int picker = UnityEngine.Random.Range(0, HeroReserve.Count);
        Card card = HeroReserve[picker];
        P1Hand.Add(card);
        HeroReserve.Remove(card);
        UpdateHandSlider();
        AddCardToHand(card);
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
            case CardDecks.P1Discard:
                i = P1Discard.Count;
                break;
            case CardDecks.P2Hand:
                i = P2Hand.Count;
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
                HeroReserve.Remove(card);
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
            case PhotonGameManager.GamePhase.Recruit:
                Debug.Log($"Removing {card.Name} from the HQ.");
                AddCardToHand(card);
                RemoveHQCard(card);
                break;
        }
    }
    #endregion
}
