using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System;

public class PhotonInGameManager : MonoBehaviourPunCallbacks
{
    PhotonView PV;
    public CardDataBase DataBase;
    public Referee GameManager;

    public static Action<string> OnOriginRequest = delegate { };

    private void Awake()
    {
        PV = GetComponent<PhotonView>();

        PlayerBase.OnExhaust += HandleBaseExhaust;
        PlayerBase.OnBaseDestroyed += HandleBaseDestroyed;
        UIResponseTimer.OnTimerRunOut += HandlePlayerResponseRunOut;
        UIConfirmation.OnSendAbilityResponse += SendAbilityResponse;

    }

    private void OnDestroy()
    {
        PlayerBase.OnExhaust -= HandleBaseExhaust;
        PlayerBase.OnBaseDestroyed -= HandleBaseDestroyed;
        UIResponseTimer.OnTimerRunOut -= HandlePlayerResponseRunOut;
        UIConfirmation.OnSendAbilityResponse -= SendAbilityResponse;
    }

    private void SendAbilityResponse(bool decide)
    {
        RPCRequest("HandleSendAbilityResponse", RpcTarget.Others, decide);
    }

    private void HandlePlayerResponseRunOut()
    {
        RPCRequest("HandlePlayerNoResponse", RpcTarget.Others, false);
    }

    public bool IsMine()
    {
        return PV.IsMine;
    }

    public bool IsMasterClient()
    {
        return PhotonNetwork.IsMasterClient;
    }

    public void RPCRequest(string methodName, RpcTarget target, params object[] parameters)
    {
        PV.RPC(methodName, target, parameters);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    //Referee
    [PunRPC]
    private void NeedResponse(bool value)
    {
        //sGameManager.ResponseTimer();
    }

    #region Player Declaration
    [PunRPC]
    private void DeclarePlayer(int currentPlayer)
    {
        switch (currentPlayer)
        {
            case 0:
                GameManager.SetPlayerNum(Referee.PlayerNum.P2);
                break;
            case 1:
                GameManager.SetPlayerNum(Referee.PlayerNum.P1);
                break;
            case 2:
                break;
            case 3:
                break;
        }
    }
    #endregion

    #region Turn Declaration
    [PunRPC]
    private void DeclaredTurn(bool myTurn)
    {
        GameManager.ToldSwitchTurn(myTurn);
    }

    [PunRPC]
    private void HandleTurnOffTurnHold(bool turnOff)
    {
        //GameManager.HandleHoldTurn(false, true);
        GameManager.PhaseChange(Referee.prevPhase);
    }

    [PunRPC]
    private void HandlePlayerAction(bool responseNeeded, string showCard)
    {
        Debug.Log($"Received a card to display: {showCard}");
        if (responseNeeded)
        {
            Debug.Log("We need to set up some sort of response.");
            GameManager.PhaseChange(Referee.GamePhase.TurnResponse);
        }
        if (showCard != "") ;
        DataBase.HandleShowOpponentCard(showCard);
    }

    [PunRPC]
    private void HandlePlayerResponded(bool b)
    {
        //Do Something...
        Debug.Log($"PhotonGameManager: Player responded with {b}");
        GameManager.AwatePlayerResponse(false);
    }

    [PunRPC]
    private void HandlePlayerNoResponse(bool b)
    {
        //Do Something...
        GameManager.AwatePlayerResponse(false);
    }

    [PunRPC]
    private void HandleAbilityDehandover(bool b)
    {
        DataBase.HandleAbilityDehandover();
    }

    [PunRPC]
    private void HandleOriginRequest(string name)
    {
        OnOriginRequest?.Invoke(name);
    }

    [PunRPC]
    private void HandleSendAbilityResponse(bool decide)
    {
        GameManager.RecieveResponse(decide);
    }
    #endregion

    #region Base Control
    private void HandleBaseExhaust(PlayerBase player)
    {
        if (player.type == PlayerBase.Type.Player)
        {
            RPCRequest("ExhaustABase", RpcTarget.Others, "Opponent");
        }
        else if (player.type == PlayerBase.Type.Opponent)
        {
            RPCRequest("ExhaustABase", RpcTarget.Others, "Player");
        }
    }

    [PunRPC]
    private void ExhaustABase(string player)
    {
        switch (player)
        {
            case "Player":
                //GameManager.OnBaseExhausted(PlayerBase.Type.Player);
                break;
            case "Opponent":
                //GameManager.OnBaseExhausted(PlayerBase.Type.Opponent);
                break;
        }
    }

    private void HandleBaseDestroyed(PlayerBase player)
    {
        if (player.type == PlayerBase.Type.Player)
        {
            RPCRequest("DestroyABase", RpcTarget.Others, "Opponent");
            //GameManager.OnBaseDestroyed(player.type);
        }
        else if (player.type == PlayerBase.Type.Opponent)
        {
            RPCRequest("DestroyABase", RpcTarget.Others, "Player");
            //GameManager.OnBaseDestroyed(player.type);
        }
    }

    [PunRPC]
    private void DestroyABase(string player)
    {
        switch (player)
        {
            case "Player":
                //GameManager.OnBaseDestroyed(PlayerBase.Type.Player);
                break;
            case "Opponent":
                //GameManager.OnBaseDestroyed(PlayerBase.Type.Opponent);
                break;
        }
    }
    #endregion

    //CardDataBase
    #region Draft
    [PunRPC]
    private void SetupAbilityDraft(bool yes)
    {
        DataBase.SetupAbilityDraft();
    }

    [PunRPC]
    private void RemoveDraftOption(string card)
    {
        DataBase.RemoveDraftOption(card);
    }

    [PunRPC]
    private void ShareCardList(string list, string[] listToShare)
    {
        DataBase.ShareCardList(list, listToShare);
    }

    [PunRPC]
    private void EndAbilityDraft(bool told)
    {
        DataBase.EndAbilityDraft(told);
    }
    #endregion

    #region HQ and Reserve
    [PunRPC]
    private void PopulatedHQ(string[] heros)
    {
        DataBase.PopulatedHQ(heros);
    }

    [PunRPC]
    private void RemoveHQHero(string cardName)
    {
        DataBase.RemoveHQHero(cardName);
    }

    [PunRPC]
    private void RemoveHeroFromReserve(string hero)
    {
        DataBase.RemoveHeroFromReserve(hero)
;
    }
    #endregion

    #region Hand and Cards
    [PunRPC]
    private void ForceDiscard(string type, int amount)
    {
        DataBase.ForceDiscard(type, amount);
    }

    [PunRPC]
    private void CardAdjustment(string name, string category, int newValue)
    {
        DataBase.CardAdjustment(name, category, newValue);
    }

    [PunRPC]
    private void FieldCardDestroy(string name, string location)
    {
        DataBase.FieldCardDestroy(name, location);
    }

    [PunRPC]
    private void ExhaustStateAdjust(string name, string location, bool state)
    {
        DataBase.ExhaustStateAdjust(name, location, state);
    }

    [PunRPC]
    private void RemoveAllNonHerosFromHand(string hand)
    {
        DataBase.RemoveAllNonHerosFromHand(hand);

    }

    [PunRPC]
    private void HeroStats(bool need)
    {
        DataBase.SendRequestedHeroStats();
    }

    [PunRPC]
    private void UpdateHeroStats(string name, int attack, int defense)
    {
        DataBase.UpdateHeroFromOpp(name, attack, defense);
    }

    [PunRPC]
    private void DrawXCards(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            DataBase.DrawCard(CardDataBase.CardDecks.MyDeck);
        }
    }
    #endregion

    #region Battle Functions
    [PunRPC]
    private void HandlePreviousAttackersAndDefender(string[] names)
    {
        DataBase.HandlePreviousAttackersAndDefender(names);
    }
    #endregion

    #region Spawn Character, Ability, Enhancement Functions
    [PunRPC]
    private void SpawnCharacterToOpponentField(string heroToSpawn)
    {
        DataBase.SpawnCharacterToOpponentField(heroToSpawn);
    }

    [PunRPC]
    private void AttachAbility(string abilityName, string cardName)
    {
        DataBase.AttachAbility(abilityName, cardName);
    }

    [PunRPC]
    private void StripEnhancements(string name)
    {
        DataBase.StripEnhancements(name);
    }

    [PunRPC]
    private void StripAbilities(string name)
    {
        DataBase.StripAbilities(name);
    }

    [PunRPC]
    private void GiveEnhancements(List<int[]> enhancements, string name)
    {
        DataBase.GiveEnhancements(enhancements, name);
    }

    [PunRPC]
    private void SilenceAbilityToFieldCall(string name, int turns)
    {
        DataBase.SilenceAbilityToFieldCall(name, turns);
    }

    [PunRPC]
    private void HandleAbilityHandOver(string nameOfAbilityToGiveControl)
    {
        DataBase.HandleAbilityHandOver(nameOfAbilityToGiveControl);
    }
    #endregion


    #region Photon Callbacks
    /// <summary>
    /// Called when the local player left the room. We need to load the Main Menu.
    /// </summary>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public override void OnPlayerEnteredRoom(Player other)
    {
        //Not seen if you are the player connecting.
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            //Called before OnPlayerLeftRoom
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient", PhotonNetwork.IsMasterClient);
        }
    }

    public override void OnPlayerLeftRoom(Player other)
    {
        //Seen when other disconnects
        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName);

        if (PhotonNetwork.IsMasterClient)
        {
            //Called before OnPlayerLeftRoom
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);
        }
    }
    #endregion
}
