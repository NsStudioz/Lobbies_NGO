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

    [Header("Texts")]
    [SerializeField] private TMP_Text totalPlayersInLobby;

    private void Start()
    {
        LeaveLobbyBtn.onClick.AddListener(LeaveLobby);
    }

    private void OnDisable()
    {
        LeaveLobbyBtn.onClick.RemoveAllListeners();
    }

    private void LeaveLobby()
    {
        LobbyEvents.OnLeaveLobby?.Invoke();
    }

    private void UpdateTotalPlayersInLobbyText(Lobby currentLobby)
    {
        totalPlayersInLobby.text = currentLobby.Players.ToString() + "/" + currentLobby.MaxPlayers.ToString();
    }


}
