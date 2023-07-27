using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyEvents
{
    public delegate void CreateLobby();
    public static CreateLobby OnCreateLobby;

    public delegate void LobbyCreated(string lobbyCode);
    public static LobbyCreated OnLobbyCreated;

    public delegate void JoiningLobbyByCode(string lobbyCode);
    public static JoiningLobbyByCode OnJoiningLobbyByCode;

    public delegate void LeaveJoinLobbyUI();
    public static LeaveJoinLobbyUI OnLeaveJoinLobbyUI;

    public delegate void LeaveLobby();
    public static LeaveLobby OnLeaveLobby;

    public delegate void LobbyUpdated(Lobby currentLobby);
    public static LobbyUpdated OnLobbyUpdated;

    public delegate void JoinedLobby();
    public static JoinedLobby OnJoinedLobby;

    public delegate void LobbyPrivacyStateChange(bool state);
    public static LobbyPrivacyStateChange OnLobbyPrivacyStateChange;

    public delegate void LobbyPrivacyStateUpdated(bool state);
    public static LobbyPrivacyStateUpdated OnLobbyPrivacyStateUpdated;


}
