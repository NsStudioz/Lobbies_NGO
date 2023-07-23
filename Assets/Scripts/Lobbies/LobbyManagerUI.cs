using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManagerUI : MonoBehaviour
{

    [SerializeField] private Button LeaveLobbyBtn;

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


}
