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

    [Header("LobbyList Elements")]
    [SerializeField] private List<TMP_Text> lobbyNames = new List<TMP_Text>();   // Available public lobbies.
    [SerializeField] private List<TMP_Text> lobbyPlayers = new List<TMP_Text>(); // Available players in public lobby.
    [SerializeField] private List<Button> lobbyListButtons = new List<Button>();

    // JoinLobby UI Elements:
    [Header("InputField")]
    [SerializeField] private TMP_InputField joinLobbyCodeInputField;

    [Header("Buttons")]
    [SerializeField] private Button leaveJoinLobbyBtn;
    [SerializeField] private Button lobbyListRefreshBtn;
    [SerializeField] private Button JoinLobbyByCodeBtn;
    [SerializeField] private Button quickJoinLobbyBtn;

    [Header("LobbyList Refresh Elements")]
    [SerializeField] private bool isEnabled;
    [SerializeField] private float lobbyListRefreshTimer = 10f;



    private void OnEnable()
    {
        lobbyListRefreshBtn.onClick.AddListener(Event_OnTriggerLobbyListRefresh);
        quickJoinLobbyBtn.onClick.AddListener(Event_OnQuickJoiningLobby);
        leaveJoinLobbyBtn.onClick.AddListener(Event_OnLeaveJoinLobbyUI);
        JoinLobbyByCodeBtn.onClick.AddListener(Event_OnJoiningLobbyByCode);
        InitializeLobbyListButtonArrayListeners();

        LobbyEvents.OnLobbyListChange += LobbyList_Refresh;
        MainMenuUI.OnJoinLobbyButtonClicked += StartAutoRefreshLobbyList;
        LobbyEvents.OnLeaveJoinLobbyUI += StopAutoRefreshLobbyList;
    }

    private void OnDisable()
    {
        lobbyListRefreshBtn.onClick.RemoveAllListeners();
        quickJoinLobbyBtn.onClick.RemoveAllListeners();
        leaveJoinLobbyBtn.onClick.RemoveAllListeners();
        JoinLobbyByCodeBtn.onClick.RemoveAllListeners();
        RemoveLobbyListButtonArrayListeners();

        LobbyEvents.OnLobbyListChange -= LobbyList_Refresh;
        MainMenuUI.OnJoinLobbyButtonClicked -= StartAutoRefreshLobbyList;
        LobbyEvents.OnLeaveJoinLobbyUI -= StopAutoRefreshLobbyList;
    }

    private void Event_OnJoiningLobbyByCode()
    {
        LobbyEvents.OnJoiningLobbyByCode?.Invoke(joinLobbyCodeInputField.text);
        Debug.Log("Joining lobby!");
    }

    private void Event_OnLeaveJoinLobbyUI()
    {
        LobbyEvents.OnLeaveJoinLobbyUI?.Invoke();
    }

    private void InitializeLobbyListButtonArrayListeners()
    {
        for (int i = 0; i < lobbyListButtons.Count; i++)
        {
            int lobbyIndex = i;
            lobbyListButtons[lobbyIndex].onClick.AddListener(() =>
            {
                Event_OnJoiningLobbyID(lobbyIndex);
            });
        }
    }

    private static void Event_OnJoiningLobbyID(int lobbyIndex)
    {
        LobbyEvents.OnJoiningLobbyID?.Invoke(lobbyIndex);
    }

    private void RemoveLobbyListButtonArrayListeners()
    {
        for (int i = 0; i < lobbyListButtons.Count; i++)
            lobbyListButtons[i].onClick.RemoveAllListeners();
    }



    private void Event_OnTriggerLobbyListRefresh()
    {
        LobbyEvents.OnTriggerLobbyListRefresh?.Invoke();
    }

    private void Event_OnQuickJoiningLobby()
    {
        LobbyEvents.OnQuickJoiningLobby?.Invoke();
    }

    private void LobbyList_Refresh(List<Lobby> lobbyList)
    {
        ClearAllUsedTextsInLists();

        for (int i = 0; i < lobbyList.Count; i++)
        {
            lobbyNames[i].text = lobbyList[i].Name;
            lobbyPlayers[i].text = lobbyList[i].Players.Count + "/" + lobbyList[i].MaxPlayers;
        }
    }

    private void ClearAllUsedTextsInLists()
    {
        foreach (TMP_Text item in lobbyNames)
            item.text = "N/A";
        foreach (TMP_Text item in lobbyPlayers)
            item.text = "N/A";
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