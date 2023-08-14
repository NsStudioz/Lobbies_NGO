using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;

    [SerializeField] private float refreshLobbyTimer = 1f;

    // Lobby Keys:
    private readonly int MAX_PLAYERS = 4;
    public const string KEY_PLAYER_NAME = "PlayerName"; // this is a dictionary key! not a value!
    public const string KEY_PLAYER_AVATAR = "Avatar";   // this is a dictionary key! not a value!
    private const string KEY_LOBBY_MAP = "LobbyMap";
    private string playerName = "";

    // Relay Keys:
    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
    private string relayJoinCodeValue = "NoCodeYet";

    // Lobby Objects
    private Lobby currentLobby;

    private List<Lobby> lobbyList;



    #region Lobby_Helpers:

    public Lobby GetCurrentLobby()
    {
        return currentLobby;
    }

    public bool IsInAnyLobby()
    {
        return currentLobby != null;
    }

    private void NotInAnyLobby()
    {
        currentLobby = null;
    }

    public bool IsLobbyHost()
    {
        return currentLobby != null && currentLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    public bool IsLobbyClient()
    {
        return currentLobby != null && AuthenticationService.Instance.PlayerId != currentLobby.HostId;
    }

    public bool IsPlayerInLobby()
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

    private async Task<Player> GetPlayer()
    {
        return new Player(AuthenticationService.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject> {
            { KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, await GetPlayerName()) },
            { KEY_PLAYER_AVATAR, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, PlayerAvatarEnum.Heart.ToString()) }
        });
    }

    private async Task<string> GetPlayerName()
    {
        playerName = await AuthenticationService.Instance.GetPlayerNameAsync();
        //Debug.Log("PlayerName in lobby: " + playerName);
        return playerName;
    }

    #endregion

    #region TryCatch_Helpers:
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

    private async Task<bool> CurrentLobbyCheck_TryCatchAsyncBool(Task promise)
    {
        if (IsInAnyLobby())
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

    public enum PlayerAvatarEnum
    { 
        Heart, Diamond, Gold, Star, Lightning
    }

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        MainMenuUI.OnCreateLobbyButtonClicked += TryCatch_CreateNewLobby;
        LobbyEvents.OnPlayerAvatarConfirmed += TryCatch_UpdatePlayerAvatar;
        LobbyEvents.OnLobbyPrivacyStateChange += ChangeLobbyPrivacyState;
        LobbyEvents.OnLobbyMapChange += TryCatch_UpdateLobbyMap;
        LobbyEvents.OnPlayerKicked += TryCatch_KickPlayer;
        
        LobbyEvents.OnJoiningLobbyByCode += TryCatch_JoinLobbyByCode;
        LobbyEvents.OnJoiningLobbyID += TryCatch_JoinLobbyID;
        LobbyEvents.OnQuickJoiningLobby += TryCatch_QuickJoinLobby;
        LobbyEvents.OnLeaveLobby += TryCatch_LeaveLobby;
        
        LobbyEvents.OnTriggerLobbyListRefresh += TryCatch_RefreshlobbyList;
        LobbyEvents.OnTriggerLobbyRefresh += HandleLobbyPolling;
        LobbyEvents.OnStartGame += StartGame;
    }

    private void OnDisable()
    {
        MainMenuUI.OnCreateLobbyButtonClicked -= TryCatch_CreateNewLobby;
        LobbyEvents.OnPlayerAvatarConfirmed -= TryCatch_UpdatePlayerAvatar;
        LobbyEvents.OnLobbyPrivacyStateChange -= ChangeLobbyPrivacyState;
        LobbyEvents.OnLobbyMapChange -= TryCatch_UpdateLobbyMap;
        LobbyEvents.OnPlayerKicked -= TryCatch_KickPlayer;

        LobbyEvents.OnJoiningLobbyByCode -= TryCatch_JoinLobbyByCode;
        LobbyEvents.OnJoiningLobbyID -= TryCatch_JoinLobbyID;
        LobbyEvents.OnQuickJoiningLobby -= TryCatch_QuickJoinLobby;
        LobbyEvents.OnLeaveLobby -= TryCatch_LeaveLobby;
        
        LobbyEvents.OnTriggerLobbyListRefresh -= TryCatch_RefreshlobbyList;
        LobbyEvents.OnTriggerLobbyRefresh -= HandleLobbyPolling;
        LobbyEvents.OnStartGame -= StartGame;
    }

    #region Lobby_Updates:

    // KICK FUNCTION WORKS ONCE! Null reference exception still thrown on client side, need further check & research on solving this.
    private async void HandleLobbyPolling()
    {
        try
        {
            if (!IsPlayerInLobby())
            {
                NotInAnyLobby();
                Event_OnKickedFromLobby();
            }
            else
            {
                Lobby newLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);
                currentLobby = newLobby;
                LobbyEvents.OnLobbyUpdated?.Invoke(currentLobby);
            }

            if (IsLobbyClient())
                await StartGameClientOnLobbyUpdated();

            // Causing null error:
/*            if (IsPlayerInLobby())
            {
                Lobby newLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);
                currentLobby = newLobby;
                LobbyEvents.OnLobbyUpdated?.Invoke(currentLobby);
            }
            else
            {
                NotInAnyLobby();
                Event_OnKickedFromLobby();
            }

            if (IsLobbyClient())
                await StartGameClientOnLobbyUpdated();*/

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            if ((e.Reason == LobbyExceptionReason.Forbidden || e.Reason == LobbyExceptionReason.LobbyNotFound))
            {
                NotInAnyLobby();
                Event_OnKickedFromLobby();
            }
        }
    }

    private IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        while (IsInAnyLobby()) // dont use while (true) => this will cause an exception (coroutines continue to work even when lobby is closed due to this)
        {
            Debug.Log(message: "Heartbeat");
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return new WaitForSecondsRealtime(waitTimeSeconds);
        }
    }

    private IEnumerator RefreshLobbyCoroutine(string lobbyId) // update lobby data (Player count, game mode, etc...)
    {
        // Original (No Kick Update):
        while (IsInAnyLobby()) // dont use while (true) => this will cause an exception (coroutines continue to work even when lobby is closed due to this)
        {
            if (IsPlayerInLobby())
            {
                Task<Lobby> task = LobbyService.Instance.GetLobbyAsync(lobbyId);
                yield return new WaitUntil(() => task.IsCompleted);

                Lobby newLobby = task.Result;
                if (newLobby.LastUpdated > currentLobby.LastUpdated)
                {
                    currentLobby = newLobby;
                    // send event for updates:
                    LobbyEvents.OnLobbyUpdated?.Invoke(currentLobby);
                }
            }

            yield return new WaitForSecondsRealtime(refreshLobbyTimer);
        }
    }

    #endregion


    #region Lobby_Connections:

    private async void TryCatch_CreateNewLobby()
    {
        await TryCatchAsyncBool(CreateNewLobby());
        Debug.Log("New Lobby Created: " + currentLobby + "/" + "Lobby ID: " + currentLobby.Id);
    }

    private async Task CreateNewLobby()
    {
        string lobbyName = "New Lobby";
        Player player = await GetPlayer();

        CreateLobbyOptions options = new CreateLobbyOptions
        {
            Player = player,
            IsPrivate = false,
            Data = new Dictionary<string, DataObject>
            {
                { KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCodeValue) },
                { KEY_LOBBY_MAP, new DataObject(DataObject.VisibilityOptions.Member, LobbyManagerUI.Instance.GetMapSceneNameString()) }
            }
        };

        Lobby lobbyInstance = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MAX_PLAYERS ,options);

        currentLobby = lobbyInstance;

        StartCoroutine(HeartbeatLobbyCoroutine(currentLobby.Id, waitTimeSeconds: 7f));
        //StartCoroutine(RefreshLobbyCoroutine(currentLobby.Id));

        LobbyEvents.OnCreateLobby?.Invoke();
        LobbyEvents.OnLobbyCreated?.Invoke(currentLobby.LobbyCode);
        Event_OnLobbyPrivacyStateUpdated();

        Debug.Log("Created Lobby " + currentLobby.Name + "  | Lobby's privacy state: " + currentLobby.IsPrivate + " | Lobby Code: " + currentLobby.LobbyCode);
    }


    // Show host's lobby panel, hide join lobby panel
    private static void Event_OnJoinedLobby()
    {
        LobbyEvents.OnJoinedLobby?.Invoke();
    }

    private async void TryCatch_JoinLobbyByCode(string lobbyCode)
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

        //StartCoroutine(RefreshLobbyCoroutine(currentLobby.Id));

        Event_OnJoinedLobby(); 
    }

    private async void TryCatch_JoinLobbyID(int lobbyIndex)
    {
        await TryCatchAsyncBool(JoinLobbyID(lobbyIndex));
    }

    private async Task JoinLobbyID(int lobbyIndex)
    {
        if(lobbyList.Count < lobbyIndex) 
            return;

        Player player = await GetPlayer();

        Lobby newLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyList[lobbyIndex].Id ,new JoinLobbyByIdOptions
        {
            Player = player
        });

        currentLobby = newLobby;

        Event_OnJoinedLobby();
    }

    private async void TryCatch_QuickJoinLobby()
    {
        await TryCatchAsyncBool(QuickJoinLobby());
    }

    private async Task QuickJoinLobby()
    {
        Player player = await GetPlayer();

        Lobby newLobby = await LobbyService.Instance.QuickJoinLobbyAsync(new QuickJoinLobbyOptions
        {
            Player = player
        });

        currentLobby = newLobby;

        Event_OnJoinedLobby();
    }

    private async void TryCatch_RefreshlobbyList()
    {
        await TryCatchAsyncBool(RefreshlobbyList());
    }

    private async Task RefreshlobbyList()
    {
        QueryLobbiesOptions options = new QueryLobbiesOptions();
        options.Count = 5;

        // Filter for open lobbies:
        options.Filters = new List<QueryFilter>
        {
            new QueryFilter(field: QueryFilter.FieldOptions.AvailableSlots,
                            op: QueryFilter.OpOptions.GT,
                            value: "0")       
        };

        // Decending order from newest to oldest:
        options.Order = new List<QueryOrder>
        {
            new QueryOrder(asc: false,
                           field: QueryOrder.FieldOptions.Created)
        };

        QueryResponse lobbyListQueryResponse = await Lobbies.Instance.QueryLobbiesAsync(options);

        lobbyList = lobbyListQueryResponse.Results;

        LobbyEvents.OnLobbyListChange?.Invoke(lobbyList);
    }

    #endregion


    #region StartGame_Relay:

    private async void StartGame()
    {
        if (IsLobbyHost())
            await StartGameHost();

        SceneManager.LoadSceneAsync(currentLobby.Data[KEY_LOBBY_MAP].Value);
    }

    private async Task<bool> StartGameClientOnLobbyUpdated()
    {
        if (currentLobby.Data[KEY_RELAY_JOIN_CODE].Value != relayJoinCodeValue)
        {
            await StartGameClient();
            SceneManager.LoadSceneAsync(currentLobby.Data[KEY_LOBBY_MAP].Value);
            return true;
        }
        else
            return false;
    }

    private async Task StartGameHost()
    {
        Allocation allocation = await RelayManager.Instance.AllocateRelay();

        string relayJoinCode = await RelayManager.Instance.GetRelayJoinCode(allocation);

        await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id, new UpdateLobbyOptions
        {
            Data = new Dictionary<string, DataObject>
            {
                { KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
            }
        });

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
        NetworkManager.Singleton.StartHost();
    }

    private async Task StartGameClient()
    {
        string relayJoinCode = currentLobby.Data[KEY_RELAY_JOIN_CODE].Value;

        JoinAllocation joinAllocation = await RelayManager.Instance.JoinRelay(relayJoinCode);

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
        NetworkManager.Singleton.StartClient();
    }


    #endregion


    #region Lobby_Host_Functions:

    private void Event_OnLobbyPrivacyStateUpdated()
    {
        LobbyEvents.OnLobbyPrivacyStateUpdated?.Invoke(currentLobby.IsPrivate);
    }

    private async void ChangeLobbyPrivacyState(bool privacyState)
    {
        if (IsLobbyHost() && IsInAnyLobby())
        {
            UpdateLobbyOptions options = new UpdateLobbyOptions
            {
                IsPrivate = privacyState
            };

            Lobby lobbyInstance = await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id, options);
            currentLobby = lobbyInstance;
            Event_OnLobbyPrivacyStateUpdated();
            Debug.Log("Is Lobby Private?: " + currentLobby.IsPrivate);
        }
    }

    private static void Event_OnKickedFromLobby()
    {
        LobbyEvents.OnKickedFromLobby?.Invoke();
    }

    private async void TryCatch_KickPlayer(string playerId)
    {
        if (IsLobbyHost())
            await TryCatchAsyncBool(KickPlayer(playerId));
    }

    private async Task KickPlayer(string playerId)
    {
        await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, playerId);
    }

    private async void TryCatch_UpdateLobbyMap(string mapName)
    {
        await TryCatchAsyncBool(UpdateLobbyMap(mapName));
    }

    private async Task UpdateLobbyMap(string mapName)
    {
        Lobby newLobby = await Lobbies.Instance.UpdateLobbyAsync(currentLobby.Id, new UpdateLobbyOptions
        {
            Data = new Dictionary<string, DataObject>
            {
                 { KEY_LOBBY_MAP, new DataObject(DataObject.VisibilityOptions.Member, mapName) }
            }
        });

        currentLobby = newLobby;
    }

    #endregion



    #region Player_modifications:

    private async void TryCatch_UpdatePlayerAvatar(PlayerAvatarEnum playerAvatar)
    {
        await CurrentLobbyCheck_TryCatchAsyncBool(UpdatePlayerAvatar(playerAvatar));
    }

    private async Task UpdatePlayerAvatar(PlayerAvatarEnum playerAvatar)
    {
        UpdatePlayerOptions options = new UpdatePlayerOptions();

        options.Data = new Dictionary<string, PlayerDataObject>()
        {
            { KEY_PLAYER_AVATAR, new PlayerDataObject(
                                visibility: PlayerDataObject.VisibilityOptions.Member, 
                                value: playerAvatar.ToString()) }
        };

        string playerId = AuthenticationService.Instance.PlayerId;

        Lobby newLobby = await LobbyService.Instance.UpdatePlayerAsync(currentLobby.Id, playerId, options);
        currentLobby = newLobby;
    }

    #endregion

    #region Lobby_Exit_Handling:

    private async void TryCatch_LeaveLobby()
    {
        await CurrentLobbyCheck_TryCatchAsyncBool(LeaveLobby());

        Debug.Log("Left Lobby");
    }

    private async Task LeaveLobby()
    {
        if(IsLobbyHost())
            StopAllCoroutines(); // stop lobby hearbeat.

        await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId);

        if (currentLobby.MaxPlayers < 1) // seems to have fixed lobby not found error.
            await TryCatch_DeleteLobby();

        NotInAnyLobby();
    }

    private async Task TryCatch_DeleteLobby()
    {
        await CurrentLobbyCheck_TryCatchAsyncBool(DeleteLobby());
    }

    private async Task DeleteLobby()
    {
        await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);
    }

    #endregion

    // This seems to be the proper way when closing app (Not 100% sure but it works atm):
    private void OnApplicationQuit()
    {
        if (IsInAnyLobby())
        {
            if (IsLobbyHost())
            {
                StopAllCoroutines();
                LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);
            }

            else if (IsLobbyClient())
                LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId);

            NotInAnyLobby();
        }

        // host receives a lobby not found bug when leaving lobby after the client exits abruptly.
    }
}


