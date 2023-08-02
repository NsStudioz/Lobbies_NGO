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

    [Header("CreateLobbyUI_Texts")]
    [SerializeField] private TMP_Text lobbyPlayerCount;
    [SerializeField] private TMP_Text lobbyPrivacyText;
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
        LobbyEvents.OnCreateLobby += InitializeLobbyPrivacyStateToPrivate;
        LobbyEvents.OnLobbyCreated += UpdateLobbyCodeText;
        LobbyEvents.OnLobbyPrivacyStateUpdated += UpdateLobbyPrivacyText;
        LobbyEvents.OnLobbyUpdated += UpdateTotalPlayersInLobbyText;
        LobbyEvents.OnLobbyUpdated += Lobby_SyncPlayersNames;
        LobbyEvents.OnLobbyUpdated += Lobby_SyncPlayerKickButtons;
    }

    private void OnDisable()
    {
        // Button Listeners:
        leaveLobbyBtn.onClick.RemoveAllListeners();
        leaveJoinLobbyBtn.onClick.RemoveAllListeners();
        lobbyPrivacyBtn.onClick.RemoveAllListeners();
        JoinLobbyByCodeBtn.onClick.RemoveAllListeners();

        // Events:
        LobbyEvents.OnCreateLobby -= InitializeLobbyPrivacyStateToPrivate;
        LobbyEvents.OnLobbyCreated -= UpdateLobbyCodeText;
        LobbyEvents.OnLobbyPrivacyStateUpdated -= UpdateLobbyPrivacyText;
        LobbyEvents.OnLobbyUpdated -= UpdateTotalPlayersInLobbyText;
        LobbyEvents.OnLobbyUpdated -= Lobby_SyncPlayersNames;
        LobbyEvents.OnLobbyUpdated -= Lobby_SyncPlayerKickButtons;
    }


    #region CreateLobby_Functions:

    private void Event_OnLeaveLobby()
    {
        LobbyEvents.OnLeaveLobby?.Invoke();
    }

    // Lobby Privacy:
    private void InitializeLobbyPrivacyStateToPrivate()
    {
        if (!isPrivate)
             isPrivate = true;

        UpdateLobbyPrivacyText(isPrivate);
    }

    private void Event_OnLobbyPrivacyStateChange()
    {
        isPrivate = !isPrivate;
        Debug.Log("LobbyUI => IsPrivate: " + isPrivate);
        LobbyEvents.OnLobbyPrivacyStateChange?.Invoke(isPrivate);
    }

    private void UpdateLobbyPrivacyText(bool state)
    {
        if (state)
            lobbyPrivacyText.text = privateLobby;
        else
            lobbyPrivacyText.text = publicLobby;
    }


    // Player Count:
    private void UpdateTotalPlayersInLobbyText(Lobby currentLobby)
    {
        lobbyPlayerCount.text = currentLobby.Players.Count.ToString() + "/" + currentLobby.MaxPlayers.ToString();
    }

    // Lobby Code:
    private void UpdateLobbyCodeText(string lobbyCode)
    {
        lobbyCodeText.text = lobbyCode;
    }


    // Player Names:
    private void Lobby_SyncPlayersNames(Lobby lobby)
    {
        Lobby_SortPlayersList(lobby);
        Lobby_ClearPlayerNames(lobby);
        Lobby_UpdatePlayerNames(lobby);
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

    // Lobby Kick Players:
    private void Lobby_SyncPlayerKickButtons(Lobby lobby)
    {
        Lobby_DeactivatePlayerKickButtons(lobby);
        Lobby_ActivatePlayerKickButtons(lobby);
    }

    private void Lobby_DeactivatePlayerKickButtons(Lobby lobby)
    {
        for (int i = 1; i < lobby.MaxPlayers; i++) // lobby.MaxPlayers => currently works, monitoring for possible index errors
            lobbyPlayerDatas[i].DeactivateKickButtons();
    }

    private void Lobby_ActivatePlayerKickButtons(Lobby lobby)
    {
        for (int i = 1; i < lobby.Players.Count; i++)
            lobbyPlayerDatas[i].ActivateKickButtons();
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
