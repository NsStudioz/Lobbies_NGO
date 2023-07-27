using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{

    private Lobby currentLobby;

    private string KEY_PLAYER_NAME = "PlayerName";
    private string playerName = "";
    private float heartbeatTimer;
    private readonly int MAX_PLAYERS = 4;
    [SerializeField] private float refreshLobbyTimer = 1f;
    //private float lobbyPollTimer; // WIP, for update version of refresh lobby.

/*    private Coroutine heartbeatCoroutine = null;
    private Coroutine refreshLobbyCoroutine = null;
    private Coroutine refreshLobbyCoroutine_Client = null;*/

    #region Helpers

    public bool IsLobbyHost()
    {
        return currentLobby != null && currentLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    public bool IsLobbyClient()
    {
        return currentLobby != null && AuthenticationService.Instance.PlayerId != currentLobby.HostId;
    }

    private bool IsPlayerInLobby()
    {
        if (currentLobby != null && currentLobby.Players != null)
        {
            foreach (Player player in currentLobby.Players)
            {
                if (player.Id == AuthenticationService.Instance.PlayerId)
                {
                    // This player is in this lobby
                    return true;
                }
            }
        }
        return false;
    }

    // Exception fixed, monitoring for further potential issues:
    private async Task<Player> GetPlayer()
    {
        return new Player(AuthenticationService.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject> {
            { KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, await GetPlayerName()) }
        });
    }

    // Exception fixed, monitoring for further potential issues:
    private async Task<string> GetPlayerName()
    {
        playerName = await AuthenticationService.Instance.GetPlayerNameAsync();
        //Debug.Log("PlayerName in lobby: " + playerName);
        return playerName;
    }

    public Lobby GetCurrentLobby()
    {
        return currentLobby;
    }


    private async Task<bool> TryCatchAsyncBool(Task promise)
    {
        try
        {
            await promise;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return false;
        }

        return true;
    }

    private async Task<bool> CurrentLobby_TryCatchAsyncBool(Task promise)
    {
        if (currentLobby != null)
        {
            try
            {
                await promise;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
                return false;
            }

            return true;
        }
        else // if it is null
        {
            return false;
        }
    }

    #endregion

    private void OnEnable()
    {
        MainMenuUI.OnCreateLobbyButtonClicked += CreateNewLobby;
        LobbyEvents.OnLeaveLobby += LeaveCurrentLobby;
        LobbyEvents.OnLobbyPrivacyStateChange += ChangeLobbyPrivacyState;
        LobbyEvents.OnJoiningLobbyByCode += JoinCurrentLobbyByCode;
    }

    private void OnDisable()
    {
        MainMenuUI.OnCreateLobbyButtonClicked -= CreateNewLobby;
        LobbyEvents.OnLeaveLobby -= LeaveCurrentLobby;
        LobbyEvents.OnLobbyPrivacyStateChange -= ChangeLobbyPrivacyState;
        LobbyEvents.OnJoiningLobbyByCode -= JoinCurrentLobbyByCode;
    }


    private IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        while (currentLobby != null) // dont use while (true) => this will cause an exception (coroutines continue to work even when lobby is closed due to this)
        {
            Debug.Log(message: "Heartbeat");
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return new WaitForSecondsRealtime(waitTimeSeconds);
        }
    }

    private IEnumerator RefreshLobbyCoroutine(string lobbyId) // update lobby data (Player count, game mode, etc...)
    {
        while (currentLobby != null) // dont use while (true) => this will cause an exception (coroutines continue to work even when lobby is closed due to this)
        {
            Task<Lobby> task = LobbyService.Instance.GetLobbyAsync(lobbyId);
            yield return new WaitUntil(() => task.IsCompleted);

            Lobby newLobby = task.Result;
            if(newLobby.LastUpdated > currentLobby.LastUpdated)
            {
                currentLobby = newLobby;
                // send event for updates:
                LobbyEvents.OnLobbyUpdated?.Invoke(currentLobby);
            }

            yield return new WaitForSecondsRealtime(refreshLobbyTimer);
        }
    }

    private async void CreateNewLobby()
    {
        await TryCatchAsyncBool(NewLobby());
        Debug.Log(currentLobby);
    }

    private async Task NewLobby()
    {
        string lobbyName = "New Lobby";
        Player player = await GetPlayer();

        CreateLobbyOptions options = new CreateLobbyOptions
        {
            Player = player,
            IsPrivate = true,
        };

        Lobby lobbyInstance = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MAX_PLAYERS ,options);

        currentLobby = lobbyInstance;

        //heartbeatCoroutine = StartCoroutine(HeartbeatLobbyCoroutine(currentLobby.Id, waitTimeSeconds: 10f));
        //refreshLobbyCoroutine = StartCoroutine(RefreshLobbyCoroutine(currentLobby.Id));
        StartCoroutine(HeartbeatLobbyCoroutine(currentLobby.Id, waitTimeSeconds: 10f));
        StartCoroutine(RefreshLobbyCoroutine(currentLobby.Id));

        LobbyEvents.OnCreateLobby?.Invoke();
        LobbyEvents.OnLobbyCreated?.Invoke(currentLobby.LobbyCode);

        //Debug.Log("Created Lobby " + lobby.Name + "  | Lobby's privacy state: " + lobby.IsPrivate + " | Lobby Code: " + lobby.LobbyCode);
        Debug.Log("Created Lobby " + currentLobby.Name + "  | Lobby's privacy state: " + currentLobby.IsPrivate + " | Lobby Code: " + currentLobby.LobbyCode);
    }

    private async void ChangeLobbyPrivacyState(bool privacyState)
    {
        if (IsLobbyHost() && currentLobby != null)
        {
            UpdateLobbyOptions options = new UpdateLobbyOptions
            {
                IsPrivate = privacyState
            };

            Lobby lobbyInstance = await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id, options);
            currentLobby = lobbyInstance;

            LobbyEvents.OnLobbyPrivacyStateUpdated?.Invoke(currentLobby.IsPrivate);
            Debug.Log("Is Lobby Private?: " + currentLobby.IsPrivate);
        }
    }

    private async void JoinCurrentLobbyByCode(string lobbyCode)
    {
        await TryCatchAsyncBool(JoinLobbyByCode(lobbyCode));
    }

    private async Task JoinLobbyByCode(string lobbyCode)
    {
        Player player = await GetPlayer();

        Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, new JoinLobbyByCodeOptions
        {
            Player = player
        });

        currentLobby = lobby;

        StartCoroutine(RefreshLobbyCoroutine(currentLobby.Id));
        //LobbyEvents.OnLobbyUpdated?.Invoke(currentLobby);
        LobbyEvents.OnJoinedLobby?.Invoke(); // Show host's lobby panel, hide join lobby panel

    }


    private async void LeaveCurrentLobby()
    {
        await CurrentLobby_TryCatchAsyncBool(LeaveLobby());

        Debug.Log("Left Lobby");
    }

    private async Task LeaveLobby()
    {
        StopLobbyCoroutines();

        if (currentLobby.MaxPlayers > 0)
        {
            await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId);

            currentLobby = null;
        }

        else if (currentLobby.MaxPlayers <= 0)
            await DeleteCurrentLobby();
    }

    private async Task<bool> DeleteCurrentLobby()
    {
        bool succeded = await CurrentLobby_TryCatchAsyncBool(DeleteLobby());
        return succeded;
    }

    private async Task DeleteLobby()
    {
        await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);

        currentLobby = null;
    }

    private void StopLobbyCoroutines()
    {
        StopAllCoroutines();
/*        heartbeatCoroutine = null;
        refreshLobbyCoroutine = null;*/
    }

    // This seems to be the proper way when closing app:
    private void OnApplicationQuit()
    {
        if (currentLobby != null)
            StopLobbyCoroutines();

        if (IsLobbyHost())
            LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);

        else if (IsLobbyClient())
            LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId);
    }

}


/*    private void Update() // dont use async update
    {
        HandleLobbyPolling(Time.deltaTime);
    }*/

/*    private async Task HandleLobbyPolling(float deltaTime) // update lobby data (Player count, game mode, etc...)
    {
        if (currentLobby != null)
        {
            lobbyPollTimer -= deltaTime;

            if (lobbyPollTimer < 0f)
            {
                float lobbyPollTimerMax = 1.1f;
                lobbyPollTimer = lobbyPollTimerMax;
                currentLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);
            }
        }
    }*/