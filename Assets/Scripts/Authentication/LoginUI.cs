using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    public static event Action OnLoginClicked;

    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject menuPanel;

    [SerializeField] private Button loginButton;

    private void Awake()
    {
        ShowLoginPanelUI();
    }

    private void OnEnable()
    {
        Login.OnLoginSuccess += ShowMainMenuUI;

        loginButton.onClick.AddListener(() =>
        {
            OnLoginClicked?.Invoke();
        });
    }

    private void OnDisable()
    {
        loginButton.onClick.RemoveAllListeners();
        Login.OnLoginSuccess -= ShowMainMenuUI;
    }

    private void ShowLoginPanelUI()
    {
        loginPanel.SetActive(true);
        menuPanel.SetActive(false);
    }

    private void ShowMainMenuUI()
    {
        loginPanel.SetActive(false);
        menuPanel.SetActive(true);
    }





}
