using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class JoinLobbyUI : MonoBehaviour
{

    //private List<Lobby> lobbiesToQuery = new List<Lobby>(6);                     // Available public lobbies.
    [SerializeField] private List<TMP_Text> lobbyNames = new List<TMP_Text>();   // Available public lobbies.
    [SerializeField] private List<TMP_Text> lobbyPlayers = new List<TMP_Text>(); // Available players in public lobby.

    [SerializeField] private Button lobbyListRefreshBtn;

    private void OnEnable()
    {
        lobbyListRefreshBtn.onClick.AddListener(Event_OnTriggerLobbyListRefresh);
    }

    private void OnDisable()
    {
        lobbyListRefreshBtn.onClick.RemoveAllListeners();
    }

    private void Event_OnTriggerLobbyListRefresh()
    {
        LobbyEvents.OnTriggerLobbyListRefresh?.Invoke();
    }



}
