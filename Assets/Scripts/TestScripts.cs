using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class TestScripts : MonoBehaviour
{
    #region Try_Catch methods:


    Lobby joinedLobby;

    private async Task DeleteThisLobby()
    {
        if (joinedLobby != null)
            await TryCatchAsync(AsyncTask_DeleteLobby());
    }

    private async Task AsyncTask_DeleteLobby()
    {
        await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);

        joinedLobby = null;
    }

    public async Task TryCatchAsync(Task promise)
    {
        try
        {
            await promise;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async Task CurrentLobby_TryCatchAsync(Task promise)
    {
        if (joinedLobby != null)
        {
            try
            {
                await promise;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }


    public async void KickPlayer(string playerId)
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void DeleteLobby()
    {
        if (joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);

                joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    #endregion

    #region LobbyManager:

    /*    private void Update()
    {
        HandleLobbyPollingNew(); // KICK FUNCTION WORKS!!!
    }*/

    /*    private async void HandleLobbyPollingNew()
        {
            if (currentLobby != null)
            {
                lobbyPollTimer -= Time.deltaTime;
                if (lobbyPollTimer <= 0)
                {
                    float lobbyPollTimerMax = 1.1f;
                    lobbyPollTimer = lobbyPollTimerMax;
                    try
                    {
                        if (IsPlayerInLobby())
                        {
                            Lobby newLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);
                            currentLobby = newLobby;
                            LobbyEvents.OnLobbyUpdated?.Invoke(currentLobby);
                        }
                        else
                        {
                            currentLobby = null;
                            LobbyEvents.OnKickedFromLobby?.Invoke();
                        }
                    }
                    catch (LobbyServiceException e)
                    {
                        Debug.Log(e);
                        if ((e.Reason == LobbyExceptionReason.Forbidden || e.Reason == LobbyExceptionReason.LobbyNotFound))
                        {
                            currentLobby = null;
                            LobbyEvents.OnKickedFromLobby?.Invoke();
                        }
                    }
                }
            }
        }*/

    /*    private async void HandleLobbyPollingOLD()
        {
            if (currentLobby != null)
            {
                lobbyPollTimer -= Time.deltaTime;
                if (lobbyPollTimer < 0f)
                {
                    float lobbyPollTimerMax = 1.1f;
                    lobbyPollTimer = lobbyPollTimerMax;
                    Lobby newLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);

                    currentLobby = newLobby;

                    LobbyEvents.OnLobbyUpdated?.Invoke(currentLobby);

                    if (!IsPlayerInLobby())
                    {
                        // Player was kicked out of this lobby
                        Debug.Log("Kicked from Lobby!");
                        currentLobby = newLobby;

                        currentLobby = null;
                    }
                }
            }
        }*/



    /*    private IEnumerator RefreshLobbyCoroutine(string lobbyId) // update lobby data (Player count, game mode, etc...)
        {
            *//*        while (currentLobby != null)
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

                        if(!IsPlayerInLobby())
                        {
                            currentLobby = null;
                            LobbyEvents.OnKickedFromLobby?.Invoke();

                            try
                            {
                                //StopAllCoroutines();
                                currentLobby = null;
                                LobbyEvents.OnKickedFromLobby?.Invoke();
                            }
                            catch (LobbyServiceException e)
                            {
                                Debug.Log(e);
                                if (e.Reason == LobbyExceptionReason.Forbidden || e.Reason == LobbyExceptionReason.LobbyNotFound)
                                {
                                    //StopAllCoroutines();
                                    currentLobby = null;
                                    LobbyEvents.OnKickedFromLobby?.Invoke();
                                }
                            }
                        }
                    }*/


    /*        while (currentLobby != null) 
            {
                Task<Lobby> task = LobbyService.Instance.GetLobbyAsync(lobbyId);
                yield return new WaitUntil(() => task.IsCompleted);

                try
                {
                    if (IsPlayerInLobby())
                    {
                        Lobby newLobby = task.Result;
                        if (newLobby.LastUpdated > currentLobby.LastUpdated)
                        {
                            currentLobby = newLobby;
                            // send event for updates:
                            LobbyEvents.OnLobbyUpdated?.Invoke(currentLobby);
                        }
                    }
                }
                catch (LobbyServiceException e)
                {
                    Debug.Log(e);
                    if (e.Reason == LobbyExceptionReason.Forbidden || e.Reason == LobbyExceptionReason.LobbyNotFound)
                    {
                        Debug.Log("Catch_Update");
                        //StopAllCoroutines();
                        currentLobby = null;
                        LobbyEvents.OnKickedFromLobby?.Invoke();
                    }
                }

                yield return new WaitForSecondsRealtime(refreshLobbyTimer);
                Debug.Log("FullLoop_Update");
            }*//*

    // Current Try Catch Coroutine loop (created: 2/8/2023):
    *//*        while (currentLobby != null) // dont use while (true) => this will cause an exception (coroutines continue to work even when lobby is closed due to this)
            {
                if (IsPlayerInLobby())
                {
                    Task<Lobby> task = LobbyService.Instance.GetLobbyAsync(lobbyId);
                    yield return new WaitUntil(() => task.IsCompleted);

                    try
                    {
                        Lobby newLobby = task.Result;
                        if (newLobby.LastUpdated > currentLobby.LastUpdated)
                        {
                            currentLobby = newLobby;
                            // send event for updates:
                            LobbyEvents.OnLobbyUpdated?.Invoke(currentLobby);
                        }
                    }
                    catch (LobbyServiceException e)
                    {
                        Debug.Log(e);
                        if (e.Reason == LobbyExceptionReason.Forbidden || e.Reason == LobbyExceptionReason.LobbyNotFound)
                        {
                            StopAllCoroutines();
                            currentLobby = null;
                            LobbyEvents.OnKickedFromLobby?.Invoke();
                        }
                    }
                }

                yield return new WaitForSecondsRealtime(refreshLobbyTimer);
            }*//*


    // Original (No Kick Update):
*//*        while (currentLobby != null) // dont use while (true) => this will cause an exception (coroutines continue to work even when lobby is closed due to this)
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
        }*//*
    }*/

    /*    private async Task KickPlayer(string playerId)
        {
            await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, playerId);

            *//*        if (playerId == AuthenticationService.Instance.PlayerId)
            {
                StopLobbyCoroutines();

                await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, playerId);

                currentLobby = null;

                Debug.Log("Left Lobby");
            }*//*
        }*/

    /*    public async void TryCatch_KickPlayer(string playerId)
        {
            if (IsLobbyHost())
            {
                try
                {
                    await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, playerId);
                }
                catch (LobbyServiceException e)
                {
                    Debug.Log(e);
                }
            }
            *//*        if (IsLobbyHost())
                        await TryCatchAsyncBool(KickPlayer(playerId));*//*
        }*/

/*    public Sprite GetSprite(LobbyManager.PlayerAvatarEnum playerAvatar)
    {
        return playerAvatar switch
        {
            LobbyManager.PlayerAvatarEnum.Diamond => playerAvatarList[1],
            LobbyManager.PlayerAvatarEnum.Gold => playerAvatarList[2],
            LobbyManager.PlayerAvatarEnum.Star => playerAvatarList[3],
            LobbyManager.PlayerAvatarEnum.Lightning => playerAvatarList[4],
            _ => playerAvatarList[0], // default = Heart
        };
    }*/


    #endregion
}
