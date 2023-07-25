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
    private readonly string publicLobby = "PUBLIC";
    private readonly string privateLobby = "PRIVATE";

    private bool isPrivate = true;

    private void Start()
    {
        LeaveLobbyBtn.onClick.AddListener(LeaveLobby);
        lobbyPrivacyBtn.onClick.AddListener(Event_OnLobbyPrivacyStateChange);
        LobbyEvents.OnLobbyUpdated += UpdateTotalPlayersInLobbyText;
        LobbyEvents.OnCreateLobby += InitializeLobbyPrivacyStateToPrivate;
        LobbyEvents.OnLobbyPrivacyStateUpdated += UpdateLobbyPrivacyText;
        Debug.Log("LobbyUI => IsPrivate: " + isPrivate);
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


}
