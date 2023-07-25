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
    private readonly string publicLobby = "Public";
    private readonly string privateLobby = "Private";

    private bool isPrivate = true;

    private void Start()
    {
        LeaveLobbyBtn.onClick.AddListener(LeaveLobby);
        lobbyPrivacyBtn.onClick.AddListener(Event_OnLobbyPrivacyStateChange);
        LobbyEvents.OnLobbyUpdated += UpdateTotalPlayersInLobbyText;
        LobbyEvents.OnCreateLobby += InitializeLobbyPrivacyStateToPrivate;
        LobbyEvents.OnLobbyPrivacyStateUpdated += UpdateLobbyPrivacyText;

    }

    private void OnDisable()
    {
        LeaveLobbyBtn.onClick.RemoveAllListeners();
        lobbyPrivacyBtn.onClick.RemoveAllListeners();
        LobbyEvents.OnLobbyUpdated -= UpdateTotalPlayersInLobbyText;
        LobbyEvents.OnCreateLobby -= InitializeLobbyPrivacyStateToPrivate;
        LobbyEvents.OnLobbyPrivacyStateUpdated -= UpdateLobbyPrivacyText;

    }

    private void LeaveLobby()
    {
        LobbyEvents.OnLeaveLobby?.Invoke();
    }

    private void InitializeLobbyPrivacyStateToPrivate()
    {
        if (!isPrivate)
             isPrivate = true;
    }

    private void Event_OnLobbyPrivacyStateChange()
    {
        isPrivate = !isPrivate;
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


}
