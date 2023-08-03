using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyRefreshTimer : MonoBehaviour
{
    [SerializeField] private float lobbyPollTimer;
    [SerializeField] private float lobbyPollTimerThreshold = 1.1f;

    private void Update()
    {
        StartTimerLoop(Time.deltaTime);
    }

    private void StartTimerLoop(float deltaTime)
    {
        if (LobbyManager.Instance.GetCurrentLobby() == null)
            return;

        else
        {
            lobbyPollTimer -= deltaTime;
            if (lobbyPollTimer <= 0)
            {
                lobbyPollTimer = lobbyPollTimerThreshold;
                LobbyEvents.OnTriggerLobbyRefresh?.Invoke();
            }
        }

    }

}
