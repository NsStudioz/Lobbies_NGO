using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class TestScripts : MonoBehaviour
{

    Lobby joinedLobby;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private async Task DeleteThisLobby()
    {
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


}
