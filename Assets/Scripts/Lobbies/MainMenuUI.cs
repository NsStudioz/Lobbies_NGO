using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public static event Action OnCreateLobbyButtonClicked;
    public static event Action OnJoinLobbyButtonClicked;

    [SerializeField] private GameObject createLobbyPanel;
    [SerializeField] private GameObject joinLobbyPanel;
    [SerializeField] private GameObject menuPanel;

    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button joinLobbyButton;
    [SerializeField] private Button optionsButton; // still not sure if I should do it, seems unnecessary atm...

    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private string PLAYER_NAME;

    private void OnEnable()
    {   
        //Buttons:
        createLobbyButton.onClick.AddListener(() =>
        {
            ShowCreateLobbyPanel();
            OnCreateLobbyButtonClicked?.Invoke();
        });

        joinLobbyButton.onClick.AddListener(() =>
        {
            ShowJoinLobbyPanel();
            OnJoinLobbyButtonClicked?.Invoke();
        });
        // Events:
        Login.OnLoginSuccess += ShowPlayerNameAsync;
        LobbyEvents.OnLeaveLobby += ShowMainMenuPanel;
        LobbyEvents.OnLeaveJoinLobbyUI += ShowMainMenuPanel;
        LobbyEvents.OnJoinedLobby += ShowCreateLobbyPanel;
        LobbyEvents.OnKickedFromLobby += ShowMainMenuPanel;

    }

    private void OnDisable()
    {
        //Buttons:
        createLobbyButton.onClick.RemoveAllListeners();
        joinLobbyButton.onClick.RemoveAllListeners();
        // Events:
        Login.OnLoginSuccess -= ShowPlayerNameAsync;
        LobbyEvents.OnLeaveLobby -= ShowMainMenuPanel;
        LobbyEvents.OnLeaveJoinLobbyUI -= ShowMainMenuPanel;
        LobbyEvents.OnJoinedLobby -= ShowCreateLobbyPanel;
        LobbyEvents.OnKickedFromLobby -= ShowMainMenuPanel;

    }

    private void ShowMainMenuPanel()
    {
        menuPanel.SetActive(true);
        createLobbyPanel.SetActive(false);
        joinLobbyPanel.SetActive(false);
    }

    private void ShowCreateLobbyPanel()
    {
        createLobbyPanel.SetActive(true);
        joinLobbyPanel.SetActive(false);
        menuPanel.SetActive(false);
    }

    private void ShowJoinLobbyPanel()
    {
        joinLobbyPanel.SetActive(true);
        createLobbyPanel.SetActive(false);
        menuPanel.SetActive(false);
    }

    private async void ShowPlayerNameAsync()
    {
        PLAYER_NAME = await AuthenticationService.Instance.GetPlayerNameAsync();
        playerNameText.text = PLAYER_NAME;
        Debug.Log("PlayerTextName: " + AuthenticationService.Instance.PlayerName);
    }

}
