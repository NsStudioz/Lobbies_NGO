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

    private void Start()
    {
        LeaveLobbyBtn.onClick.AddListener(LeaveLobby);
        LobbyEvents.OnLobbyUpdated += UpdateTotalPlayersInLobbyText;
    }

    private void OnDisable()
    {
        LeaveLobbyBtn.onClick.RemoveAllListeners();
        LobbyEvents.OnLobbyUpdated -= UpdateTotalPlayersInLobbyText;
    }

    private void LeaveLobby()
    {
        LobbyEvents.OnLeaveLobby?.Invoke();
    }

    private void UpdateTotalPlayersInLobbyText(Lobby currentLobby)
    {
        LobbyPlayerCount.text = currentLobby.Players.Count.ToString() + "/" + currentLobby.MaxPlayers.ToString();
    }


}
