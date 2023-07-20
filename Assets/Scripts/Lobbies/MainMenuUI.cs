using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{

    [SerializeField] private GameObject createLobbyPanel;
    [SerializeField] private GameObject joinLobbyPanel;
    [SerializeField] private GameObject menuPanel;

    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button joinLobbyButton;
    [SerializeField] private Button optionsButton;

    // TO-DO:
    // Options button, options panel & Show Options menu

    private void Start()
    {
        ShowMainMenuPanel();
    }

    private void OnEnable()
    {
        createLobbyButton.onClick.AddListener(ShowCreateLobbyPanel);
        joinLobbyButton.onClick.AddListener(ShowJoinLobbyPanel);
    }

    private void OnDisable()
    {
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


}
