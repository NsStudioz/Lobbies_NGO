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

    public delegate void QuickJoinedLobby();
    public static QuickJoinedLobby OnQuickJoiningLobby;

    public delegate void JoinedLobby();
    public static JoinedLobby OnJoinedLobby;

    // Lobby_Leave:
    public delegate void LeaveLobby();
    public static LeaveLobby OnLeaveLobby;

    public delegate void LeaveJoinLobbyUI();
    public static LeaveJoinLobbyUI OnLeaveJoinLobbyUI;

    // Player_Kick:
    public delegate void PlayerKicked(string playerId);
    public static PlayerKicked OnPlayerKicked;

    public delegate void KickedFromLobby();
    public static KickedFromLobby OnKickedFromLobby;

    // Lobby_Update:
    public delegate void TriggerLobbyRefresh();
    public static TriggerLobbyRefresh OnTriggerLobbyRefresh;

    public delegate void LobbyUpdated(Lobby currentLobby);
    public static LobbyUpdated OnLobbyUpdated;

    public delegate void LobbyPrivacyStateChange(bool state);
    public static LobbyPrivacyStateChange OnLobbyPrivacyStateChange;

    public delegate void LobbyPrivacyStateUpdated(bool state);
    public static LobbyPrivacyStateUpdated OnLobbyPrivacyStateUpdated;

    public delegate void LobbyMapChange(string mapName);
    public static LobbyMapChange OnLobbyMapChange;

    // Lobby Query:
    public delegate void TriggerLobbyListRefresh();
    public static TriggerLobbyListRefresh OnTriggerLobbyListRefresh;

    public delegate void LobbyListChange(List<Lobby> lobbyList);
    public static LobbyListChange OnLobbyListChange;

    // Player_Update:
    public delegate void PlayerAvatarUpdate(string playerAvatar);
    public static PlayerAvatarUpdate OnPlayerAvatarUpdate;

    public delegate void PlayerAvatarChoosed();
    public static PlayerAvatarChoosed OnChoosePlayerAvatar;

    public delegate void PlayerAvatarConfirmed(LobbyManager.PlayerAvatarEnum playerAvatar);
    public static PlayerAvatarConfirmed OnPlayerAvatarConfirmed;

    public delegate void StartGame();
    public static StartGame OnStartGame;
}
