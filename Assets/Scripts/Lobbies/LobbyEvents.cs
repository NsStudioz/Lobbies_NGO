using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyEvents
{

    public delegate void LeaveLobby();
    public static LeaveLobby OnLeaveLobby;

    public delegate void LobbyUpdated(Lobby currentLobby);
    public static LobbyUpdated OnLobbyUpdated;

    public delegate void LobbyPrivacyStateChange(bool state);
    public static LobbyPrivacyStateChange OnLobbyPrivacyStateChange;

    
}
