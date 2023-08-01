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
    [SerializeField] private List<Player> lobbyPlayers = new List<Player>();
    [SerializeField] private List<LobbyPlayerData> lobbyPlayerDatas = new List<LobbyPlayerData>();

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
        LobbyEvents.OnLobbyUpdated += UpdateTotalPlayersInLobbyText;
        LobbyEvents.OnCreateLobby += InitializeLobbyPrivacyStateToPrivate;
        LobbyEvents.OnLobbyCreated += UpdateLobbyCodeText;
        LobbyEvents.OnLobbyPrivacyStateUpdated += UpdateLobbyPrivacyText;
    }

    private void OnDisable()
    {
        // Button Listeners:
        leaveLobbyBtn.onClick.RemoveAllListeners();
        leaveJoinLobbyBtn.onClick.RemoveAllListeners();
        lobbyPrivacyBtn.onClick.RemoveAllListeners();
        JoinLobbyByCodeBtn.onClick.RemoveAllListeners();

        // Events:
        LobbyEvents.OnLobbyUpdated -= UpdateTotalPlayersInLobbyText;
        LobbyEvents.OnCreateLobby -= InitializeLobbyPrivacyStateToPrivate;
        LobbyEvents.OnLobbyCreated -= UpdateLobbyCodeText;
        LobbyEvents.OnLobbyPrivacyStateUpdated -= UpdateLobbyPrivacyText;
    }


    #region CreateLobby_Functions:

    private void Event_OnLeaveLobby()
    {
        LobbyEvents.OnLeaveLobby?.Invoke();
    }

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

    private void UpdateTotalPlayersInLobbyText(Lobby currentLobby)
    {
        lobbyPlayerCount.text = currentLobby.Players.Count.ToString() + "/" + currentLobby.MaxPlayers.ToString();
    }

    private void UpdateLobbyCodeText(string lobbyCode)
    {
        lobbyCodeText.text = lobbyCode;
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
