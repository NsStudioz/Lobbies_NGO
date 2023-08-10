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

    [SerializeField] private List<TMP_Text> lobbyNames = new List<TMP_Text>();   // Available public lobbies.
    [SerializeField] private List<TMP_Text> lobbyPlayers = new List<TMP_Text>(); // Available players in public lobby.

    [SerializeField] private Button lobbyListRefreshBtn;

    [SerializeField] private bool isEnabled;
    [SerializeField] private float lobbyListRefreshTimer = 10f;


    private void OnEnable()
    {
        lobbyListRefreshBtn.onClick.AddListener(Event_OnTriggerLobbyListRefresh);
        LobbyEvents.OnLobbyListChange += LobbyList_Refresh;
        MainMenuUI.OnJoinLobbyButtonClicked += StartAutoRefreshLobbyList;
        LobbyEvents.OnLeaveJoinLobbyUI += StopAutoRefreshLobbyList;
    }

    private void OnDisable()
    {
        lobbyListRefreshBtn.onClick.RemoveAllListeners();
        LobbyEvents.OnLobbyListChange -= LobbyList_Refresh;
        MainMenuUI.OnJoinLobbyButtonClicked -= StartAutoRefreshLobbyList;
        LobbyEvents.OnLeaveJoinLobbyUI -= StopAutoRefreshLobbyList;
    }

    private void Event_OnTriggerLobbyListRefresh()
    {
        LobbyEvents.OnTriggerLobbyListRefresh?.Invoke();
    }

    private void LobbyList_Refresh(List<Lobby> lobbyList)
    {
        for(int i = 0; i < lobbyList.Count; i++)
        {
            lobbyNames[i].text = lobbyList[i].Name;
            lobbyPlayers[i].text = lobbyList[i].Players.Count + "/" + lobbyList[i].MaxPlayers;
        }
    }

    private void StartAutoRefreshLobbyList()
    {
        isEnabled = true;

        StartCoroutine(RefreshLobbyListTimer());
    }
    private void StopAutoRefreshLobbyList()
    {
        isEnabled = false;

        StopAllCoroutines();
    }

    private IEnumerator RefreshLobbyListTimer()
    {
        while (isEnabled)
        {
            Event_OnTriggerLobbyListRefresh();

            Debug.Log("Trigger lobby list refresh!");
            yield return new WaitForSecondsRealtime(lobbyListRefreshTimer);
        }
    }


}

//private List<Lobby> lobbiesToQuery = new List<Lobby>(6); // Available public lobbies.