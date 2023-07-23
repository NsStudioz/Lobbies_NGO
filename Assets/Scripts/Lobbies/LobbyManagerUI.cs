using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManagerUI : MonoBehaviour
{

    [SerializeField] private Button LeaveLobbyBtn;

    private void OnEnable()
    {
        LeaveLobbyBtn.onClick.AddListener(LeaveLobby);
    }

    private void OnDisable()
    {
        LeaveLobbyBtn.onClick.RemoveAllListeners();
    }

    private void LeaveLobby()
    {
        // invoke event leave lobby (Host und Client)
    }


}
