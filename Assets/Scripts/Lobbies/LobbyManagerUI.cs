using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManagerUI : MonoBehaviour
{
    // CreateLobby UI Elements:
    [Header("CreateLobbyUI_Buttons")]
    [SerializeField] private Button leaveLobbyBtn;
    [SerializeField] private Button lobbyPrivacyBtn;
    [SerializeField] private Button nextMapBtn;
    [SerializeField] private Button previousMapBtn;
    [SerializeField] private Button startBtn;

    [Header("CreateLobbyUI_PlayerAvatar UI")]
    [SerializeField] private GameObject playerAvatarPanel;
    [SerializeField] private Button[] avatarArrayBtn;
    [SerializeField] private Button confirmAvatar;

    [Header("CreateLobbyUI_Texts")]
    [SerializeField] private TMP_Text lobbyPlayerCount;
    [SerializeField] private TMP_Text lobbyPrivacyText;
    [SerializeField] private TMP_Text lobbyCodeTextNumber;
    [SerializeField] private TMP_Text lobbyCodeText;

    private readonly string publicLobby = "PUBLIC";
    private readonly string privateLobby = "PRIVATE";
    private bool isPrivate = true;

    [Header("CreateLobbyUI_Lists")]
    [SerializeField] private List<LobbyPlayerData> lobbyPlayerDatas = new List<LobbyPlayerData>();
    private List<Player> lobbyPlayers = new List<Player>();

    // JoinLobby UI Elements:
    [Header("JoinLobbyUI_Texts")]
    [SerializeField] private TMP_InputField joinLobbyCodeInputField;

    [Header("JoinLobbyUI_Buttons")]
    [SerializeField] private Button leaveJoinLobbyBtn;
    [SerializeField] private Button JoinLobbyByCodeBtn;

    private void Start()
    {
        // Button Listeners:
        leaveLobbyBtn.onClick.AddListener(Event_OnLeaveLobby);
        leaveJoinLobbyBtn.onClick.AddListener(Event_OnLeaveJoinLobbyUI);
        lobbyPrivacyBtn.onClick.AddListener(Event_OnLobbyPrivacyStateChange);
        JoinLobbyByCodeBtn.onClick.AddListener(Event_OnJoiningLobbyByCode);

        // Events:
        LobbyEvents.OnCreateLobby += Lobby_InitializePrivacyStateToPrivate;
        LobbyEvents.OnLobbyCreated += Lobby_UpdateCodeNumberText;
        LobbyEvents.OnLobbyPrivacyStateUpdated += Lobby_UpdateLobbyPrivacyText;
        LobbyEvents.OnLobbyUpdated += Lobby_UpdateLobby;
        LobbyEvents.OnJoinedLobby += Lobby_DeactivateHostRelatedElementsOnClientSide;
        //LobbyEvents.OnLobbyUpdated += Lobby_UpdateLobbyPlayerCountText;
        //LobbyEvents.OnLobbyUpdated += Lobby_SyncPlayersNames;
        //LobbyEvents.OnLobbyUpdated += Lobby_DeactivateHostRelatedKickButtons;
        //LobbyEvents.OnLobbyUpdated += Lobby_SyncPlayerKickButtons;
    }



    private void OnDisable()
    {
        // Button Listeners:
        leaveLobbyBtn.onClick.RemoveAllListeners();
        leaveJoinLobbyBtn.onClick.RemoveAllListeners();
        lobbyPrivacyBtn.onClick.RemoveAllListeners();
        JoinLobbyByCodeBtn.onClick.RemoveAllListeners();

        // Events:
        LobbyEvents.OnCreateLobby -= Lobby_InitializePrivacyStateToPrivate;
        LobbyEvents.OnLobbyCreated -= Lobby_UpdateCodeNumberText;
        LobbyEvents.OnLobbyPrivacyStateUpdated -= Lobby_UpdateLobbyPrivacyText;
        LobbyEvents.OnLobbyUpdated -= Lobby_UpdateLobby;
        LobbyEvents.OnJoinedLobby -= Lobby_DeactivateHostRelatedElementsOnClientSide;
        //LobbyEvents.OnLobbyUpdated -= Lobby_UpdateLobbyPlayerCountText;
        //LobbyEvents.OnLobbyUpdated -= Lobby_SyncPlayersNames;
        //LobbyEvents.OnLobbyUpdated -= Lobby_DeactivateHostRelatedKickButtons;
        //LobbyEvents.OnLobbyUpdated -= Lobby_SyncPlayerKickButtons;
    }


    #region CreateLobby_Functions:

    private void Lobby_UpdateLobby(Lobby lobby)
    {
        Lobby_UpdateLobbyPlayerCountText(lobby);
        Lobby_SyncPlayersNames(lobby);
        Lobby_SyncPlayerKickButtons(lobby);
        Lobby_DeactivateHostRelatedKickButtons(lobby);
    }

    private void Event_OnLeaveLobby()
    {
        LobbyEvents.OnLeaveLobby?.Invoke();
    }

    // Lobby Privacy:
    private void Event_OnLobbyPrivacyStateChange()
    {
        isPrivate = !isPrivate;
        Debug.Log("LobbyUI => IsPrivate: " + isPrivate);
        LobbyEvents.OnLobbyPrivacyStateChange?.Invoke(isPrivate);
    }

    private void Lobby_InitializePrivacyStateToPrivate()
    {
        if (!isPrivate)
             isPrivate = true;

        Lobby_UpdateLobbyPrivacyText(isPrivate);
    }

    private void Lobby_UpdateLobbyPrivacyText(bool state)
    {
        if (state)
            lobbyPrivacyText.text = privateLobby;
        else
            lobbyPrivacyText.text = publicLobby;
    }


    // Player Count:
    private void Lobby_UpdateLobbyPlayerCountText(Lobby lobby)
    {
        lobbyPlayerCount.text = lobby.Players.Count.ToString() + "/" + lobby.MaxPlayers.ToString();
    }

    // Lobby Code:
    private void Lobby_UpdateCodeNumberText(string lobbyCode)
    {
        lobbyCodeTextNumber.text = lobbyCode;
    }


    // Player Names:
    private void Lobby_SyncPlayersNames(Lobby lobby)
    {
        Lobby_SortPlayersList(lobby);
        Lobby_ClearPlayerNames(lobby);
        Lobby_UpdatePlayerNames(lobby);
        // needs better placement:
        //Lobby_SyncPlayerKickButtons(lobby);
    }

    private void Lobby_SortPlayersList(Lobby lobby)
    {
        lobbyPlayers.Clear();

        foreach (Player player in lobby.Players)
            lobbyPlayers.Add(player);

/*        for (int i = 0; i < lobby.Players.Count; i++)
            lobbyPlayers.Add(lobby.Players[i]);*/
    }

    private void Lobby_ClearPlayerNames(Lobby lobby)
    {
        for (int i = 0; i < lobby.MaxPlayers - 1; i++)
            lobbyPlayerDatas[i].ResetPlayerNameText();
    }

    private void Lobby_UpdatePlayerNames(Lobby lobby)
    {
        for (int i = 0; i < lobby.Players.Count; i++)
            lobbyPlayerDatas[i].UpdatePlayerName(lobbyPlayers[i]);
    }

    // Lobby Host Kick Buttons:
    private void Lobby_SyncPlayerKickButtons(Lobby lobby)
    {
        Lobby_DeactivatePlayerKickButtons(lobby);
        Lobby_ActivatePlayerKickButtons(lobby);
    }

    private void Lobby_DeactivatePlayerKickButtons(Lobby lobby)
    {
        for (int i = 1; i < lobby.MaxPlayers; i++) // lobby.MaxPlayers => currently works, monitoring for possible errors
            lobbyPlayerDatas[i].DeactivateKickButtons();
    }

    private void Lobby_ActivatePlayerKickButtons(Lobby lobby)
    {
        for (int i = 1; i < lobby.Players.Count; i++)
            lobbyPlayerDatas[i].ActivateKickButtons();
    }

    // Client Limitations:
    private void Lobby_DeactivateHostRelatedElementsOnClientSide()
    {
        DeactivateHostRelatedButtons();
        DeactivateHostsRelatedTexts();
    }

    private void Lobby_DeactivateHostRelatedKickButtons(Lobby lobby)
    {
        if (LobbyManager.Instance.IsLobbyClient())
            for (int i = 1; i < lobby.MaxPlayers; i++) // lobby.MaxPlayers => currently works, monitoring for possible errors
                lobbyPlayerDatas[i].DeactivateKickButtons();
    }

    private void DeactivateHostsRelatedTexts()
    {
        lobbyPrivacyText.gameObject.SetActive(false);
        lobbyCodeTextNumber.gameObject.SetActive(false);
        lobbyCodeText.gameObject.SetActive(false);
    }

    private void DeactivateHostRelatedButtons()
    {
        lobbyPrivacyBtn.gameObject.SetActive(false);
        startBtn.gameObject.SetActive(false);
        previousMapBtn.gameObject.SetActive(false);
        nextMapBtn.gameObject.SetActive(false);
    }

    #endregion

    #region JoinLobby_Functions:

    private void Event_OnJoiningLobbyByCode()
    {
        LobbyEvents.OnJoiningLobbyByCode?.Invoke(joinLobbyCodeInputField.text);
        Debug.Log("Joining lobby!");
    }

    private void Event_OnLeaveJoinLobbyUI()
    {
        LobbyEvents.OnLeaveJoinLobbyUI?.Invoke();
    }



    #endregion

}
