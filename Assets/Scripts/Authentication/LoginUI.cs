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

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }



}
