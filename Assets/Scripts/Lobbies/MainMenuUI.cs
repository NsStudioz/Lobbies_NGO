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
    [SerializeField] private Button optionsButton;

    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private string PLAYER_NAME;

    // TO-DO:
    // Options button, options panel & Show Options menu

    private void OnEnable()
    {
        Login.OnLoginSuccess += ShowPlayerNameAsync;
        //
        createLobbyButton.onClick.AddListener(() =>
        {
            ShowCreateLobbyPanel();
            Event_OnCreateLobbyButtonClicked();
        });

        joinLobbyButton.onClick.AddListener(() =>
        {
            ShowJoinLobbyPanel();
            Event_OnJoinLobbyButtonClicked();
        });
    }

    private static void Event_OnJoinLobbyButtonClicked()
    {
        OnJoinLobbyButtonClicked?.Invoke();
    }

    private static void Event_OnCreateLobbyButtonClicked()
    {
        OnCreateLobbyButtonClicked?.Invoke();
    }

    private void OnDisable()
    {
        Login.OnLoginSuccess -= ShowPlayerNameAsync;
        //
        createLobbyButton.onClick.RemoveAllListeners();
        joinLobbyButton.onClick.RemoveAllListeners();
        optionsButton.onClick.RemoveAllListeners();
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
    }

}
