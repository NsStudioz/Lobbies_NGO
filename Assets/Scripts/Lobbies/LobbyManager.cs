using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{

    [SerializeField] private Lobby currentLobby;

    public string KEY_PLAYER_NAME { get; private set; }
    private string playerName = "";
    private float heartbeatTimer;
    private readonly int MAX_PLAYERS = 4;

    #region Helpers

    private bool IsLobbyHost()
    {
        return currentLobby != null && currentLobby.HostId == AuthenticationService.Instance.PlayerId;
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

    // May lead to potential exception:
    private async Task<Player> GetPlayer()
    {
        return new Player(AuthenticationService.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject> {
            { KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, await GetPlayerName()) }
        });
    }

    // May lead to potential exception:
    private async Task<string> GetPlayerName()
    {
        playerName = await AuthenticationService.Instance.GetPlayerNameAsync();
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
    }

    private void OnDisable()
    {
        MainMenuUI.OnCreateLobbyButtonClicked -= CreateNewLobby;
        LobbyEvents.OnLeaveLobby -= LeaveCurrentLobby;
    }

    private void Update()
    {
        HandleLobbyHeartbeat(Time.deltaTime);
        //HandleLobbyPolling(Time.deltaTime);
    }

    private void HandleLobbyHeartbeat(float deltaTime)
    {
        if (IsLobbyHost())
        {
            heartbeatTimer -= deltaTime;
            if (heartbeatTimer <= 0f)
            {
                float heartbeatTimerMax = 15f;
                heartbeatTimer = heartbeatTimerMax;

                LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
            }
        }
    }


    private async void CreateNewLobby()
    {
        await TryCatchAsyncBool(NewLobby());
        Debug.Log(currentLobby);
    }

/*    private async void CreateLobbyTest()
    {
        try
        {
            string lobbyName = "New Lobby"; // lobby name
            Player player = await GetPlayer(); // Get a new player data

            CreateLobbyOptions options = new CreateLobbyOptions
            {
                Player = player,        // set the new player
                IsPrivate = true, // set lobby to private
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MAX_PLAYERS, options);

            currentLobby = lobby;

            //Debug.Log("Created Lobby " + lobby.Name + "  | Lobby's privacy state: " + lobby.IsPrivate + " | Lobby Code: " + lobby.LobbyCode);
            Debug.Log("Created Lobby " + currentLobby.Name + "  | Lobby's privacy state: " + currentLobby.IsPrivate + " | Lobby Code: " + currentLobby.LobbyCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }*/

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

        //Debug.Log("Created Lobby " + lobby.Name + "  | Lobby's privacy state: " + lobby.IsPrivate + " | Lobby Code: " + lobby.LobbyCode);
        //Debug.Log("Created Lobby " + currentLobby.Name + "  | Lobby's privacy state: " + currentLobby.IsPrivate + " | Lobby Code: " + currentLobby.LobbyCode);
    }


    private async void LeaveCurrentLobby()
    {
        await CurrentLobby_TryCatchAsyncBool(LeaveLobby());

        Debug.Log("Left Lobby");
    }

    private async Task LeaveLobby()
    {
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

    private async void OnApplicationQuit()
    {
        if (IsLobbyHost())
            await DeleteCurrentLobby();
    }

}
