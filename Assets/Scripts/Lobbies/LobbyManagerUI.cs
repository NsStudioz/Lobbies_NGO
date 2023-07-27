using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManagerUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button LeaveLobbyBtn;
    [SerializeField] private Button lobbyPrivacyBtn;

    [Header("Texts")]
    [SerializeField] private TMP_Text LobbyPlayerCount;
    [SerializeField] private TMP_Text lobbyPrivacyText;
    [SerializeField] private TMP_Text lobbyCodeText;
    private readonly string publicLobby = "PUBLIC";
    private readonly string privateLobby = "PRIVATE";

    private bool isPrivate = true;

    // JoinLobby UI Elements:
    [Header("JoinLobbyUI_Texts")]
    [SerializeField] private TMP_InputField joinLobbyCodeInputField;

    [Header("JoinLobbyUI_Buttons")]
    [SerializeField] private Button leaveJoinLobbyBtn;
    [SerializeField] private Button JoinLobbyByCodeBtn;

    private void Start()
    {
        LeaveLobbyBtn.onClick.AddListener(LeaveLobby);
        leaveJoinLobbyBtn.onClick.AddListener(Event_LeaveJoinLobbyUI);
        lobbyPrivacyBtn.onClick.AddListener(Event_OnLobbyPrivacyStateChange);
        JoinLobbyByCodeBtn.onClick.AddListener(ClickToJoinLobbyByCode);
        LobbyEvents.OnLobbyUpdated += UpdateTotalPlayersInLobbyText;
        LobbyEvents.OnCreateLobby += InitializeLobbyPrivacyStateToPrivate;
        LobbyEvents.OnLobbyCreated += UpdateLobbyCodeText;
        LobbyEvents.OnLobbyPrivacyStateUpdated += UpdateLobbyPrivacyText;
    }

    private void OnDisable()
    {
        LeaveLobbyBtn.onClick.RemoveAllListeners();
        leaveJoinLobbyBtn.onClick.RemoveAllListeners();
        lobbyPrivacyBtn.onClick.RemoveAllListeners();
        JoinLobbyByCodeBtn.onClick.RemoveAllListeners();
        LobbyEvents.OnLobbyUpdated -= UpdateTotalPlayersInLobbyText;
        LobbyEvents.OnCreateLobby -= InitializeLobbyPrivacyStateToPrivate;
        LobbyEvents.OnLobbyCreated -= UpdateLobbyCodeText;
        LobbyEvents.OnLobbyPrivacyStateUpdated -= UpdateLobbyPrivacyText;

    }

    private void ClickToJoinLobbyByCode()
    {
        LobbyEvents.OnJoiningLobbyByCode?.Invoke(joinLobbyCodeInputField.text);
        Debug.Log("Joined lobby!");
    }

    private void LeaveLobby()
    {
        LobbyEvents.OnLeaveLobby?.Invoke();
    }

    private void Event_LeaveJoinLobbyUI()
    {
        LobbyEvents.OnLeaveJoinLobbyUI?.Invoke();
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
        LobbyPlayerCount.text = currentLobby.Players.Count.ToString() + "/" + currentLobby.MaxPlayers.ToString();
    }

    private void UpdateLobbyCodeText(string lobbyCode)
    {
        lobbyCodeText.text = lobbyCode;
    }


}
