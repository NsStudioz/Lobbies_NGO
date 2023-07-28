using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyEvents
{

    // Lobby_Create:
    public delegate void CreateLobby();
    public static CreateLobby OnCreateLobby;

    public delegate void LobbyCreated(string lobbyCode);
    public static LobbyCreated OnLobbyCreated;

    // Lobby_Join:
    public delegate void JoiningLobbyByCode(string lobbyCode);
    public static JoiningLobbyByCode OnJoiningLobbyByCode;

    public delegate void JoinedLobby();
    public static JoinedLobby OnJoinedLobby;

    // Lobby_Leave:
    public delegate void LeaveLobby();
    public static LeaveLobby OnLeaveLobby;

    public delegate void LeaveJoinLobbyUI();
    public static LeaveJoinLobbyUI OnLeaveJoinLobbyUI;

    // Lobby_Update:
    public delegate void LobbyUpdated(Lobby currentLobby);
    public static LobbyUpdated OnLobbyUpdated;

    public delegate void LobbyPrivacyStateChange(bool state);
    public static LobbyPrivacyStateChange OnLobbyPrivacyStateChange;

    public delegate void LobbyPrivacyStateUpdated(bool state);
    public static LobbyPrivacyStateUpdated OnLobbyPrivacyStateUpdated;


}
