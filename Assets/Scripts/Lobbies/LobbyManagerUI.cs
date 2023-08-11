using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManagerUI : MonoBehaviour
{
    public static LobbyManagerUI Instance;

    // CreateLobby UI Elements:
    [Header("CreateLobbyUI_Buttons")]
    [SerializeField] private Button leaveLobbyBtn;
    [SerializeField] private Button lobbyPrivacyBtn;
    [SerializeField] private Button nextMapBtn;
    [SerializeField] private Button previousMapBtn;
    [SerializeField] private Button startBtn;

    [Header("CreateLobbyUI_PlayerAvatar UI")]
    [SerializeField] private GameObject playerAvatarPanel;
    [SerializeField] private Button[] avatarArrayBtn;
    [SerializeField] private Button chooseAvatar;

    [Header("CreateLobbyUI_Texts")]
    [SerializeField] private TMP_Text lobbyPlayerCount;
    [SerializeField] private TMP_Text lobbyPrivacyText;
    [SerializeField] private TMP_Text lobbyCodeTextNumber;
    [SerializeField] private TMP_Text lobbyCodeText;
    [SerializeField] private TMP_Text mapNameText;
    [SerializeField] private Image mapImage;

    private readonly string publicLobby = "PUBLIC";
    private readonly string privateLobby = "PRIVATE";
    private bool isPrivate = false;

    [Header("CreateLobbyUI_Lists")]
    [SerializeField] private List<LobbyPlayerData> lobbyPlayerDatas = new List<LobbyPlayerData>();
    private List<Player> lobbyPlayers = new List<Player>();
    [SerializeField] private List<int> mapListInt = new List<int>();
    private int mapIndex = 0;

    // JoinLobby UI Elements:
    [Header("JoinLobbyUI_Texts")]
    [SerializeField] private TMP_InputField joinLobbyCodeInputField;

    [Header("JoinLobbyUI_Buttons")]
    [SerializeField] private Button leaveJoinLobbyBtn;
    [SerializeField] private Button JoinLobbyByCodeBtn;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Button Listeners:
        leaveLobbyBtn.onClick.AddListener(Event_OnLeaveLobby);
        leaveJoinLobbyBtn.onClick.AddListener(Event_OnLeaveJoinLobbyUI);
        lobbyPrivacyBtn.onClick.AddListener(Event_OnLobbyPrivacyStateChange);
        JoinLobbyByCodeBtn.onClick.AddListener(Event_OnJoiningLobbyByCode);
        chooseAvatar.onClick.AddListener(OpenPlayerAvatarPanelUI);
        startBtn.onClick.AddListener(Event_OnStartGame);
        nextMapBtn.onClick.AddListener(Lobby_NextMap);
        previousMapBtn.onClick.AddListener(Lobby_PreviousMap);

        // Events:
        LobbyEvents.OnLobbyCreated += Lobby_UpdateCodeNumberText;
        LobbyEvents.OnLobbyPrivacyStateUpdated += Lobby_UpdateLobbyPrivacyText;
        LobbyEvents.OnLobbyUpdated += Lobby_UpdateLobby;
        LobbyEvents.OnJoinedLobby += Lobby_DeactivateHostRelatedElementsOnClientSide;
        LobbyEvents.OnChoosePlayerAvatar += OpenPlayerAvatarPanelUI;
    }

    private void OnEnable()
    {
        InitializeAvatarArrayButtonListeners();
    }

    private void OnDisable()
    {
        // Button Listeners:
        leaveLobbyBtn.onClick.RemoveAllListeners();
        leaveJoinLobbyBtn.onClick.RemoveAllListeners();
        lobbyPrivacyBtn.onClick.RemoveAllListeners();
        JoinLobbyByCodeBtn.onClick.RemoveAllListeners();
        chooseAvatar.onClick.RemoveAllListeners();
        startBtn.onClick.RemoveAllListeners();
        nextMapBtn.onClick.RemoveAllListeners();
        previousMapBtn.onClick.RemoveAllListeners();
        RemoveAvatarArrayButtonListeners();

        // Events:
        LobbyEvents.OnLobbyCreated -= Lobby_UpdateCodeNumberText;
        LobbyEvents.OnLobbyPrivacyStateUpdated -= Lobby_UpdateLobbyPrivacyText;
        LobbyEvents.OnLobbyUpdated -= Lobby_UpdateLobby;
        LobbyEvents.OnJoinedLobby -= Lobby_DeactivateHostRelatedElementsOnClientSide;
        LobbyEvents.OnChoosePlayerAvatar -= OpenPlayerAvatarPanelUI;
    }

    private void Lobby_UpdateLobby(Lobby lobby)
    {
        Lobby_UpdateLobbyPlayerCountText(lobby);
        Lobby_SyncPlayersNames(lobby);
        Lobby_SyncPlayerKickButtons(lobby);
        Lobby_DeactivateHostRelatedKickButtons(lobby);
        Lobby_ResetPlayerAvatars(lobby);
        Lobby_SyncPlayerAvatars(lobby);
        Lobby_SetMapName(lobby);
        Lobby_SetMapImage(lobby);
    }

    private void Event_OnStartGame()
    {
        LobbyEvents.OnStartGame?.Invoke();
    }

    private void Event_OnLeaveLobby()
    {
        LobbyEvents.OnLeaveLobby?.Invoke();
    }

    #region Map_Changes:

    public int GetMapIndex()
    {
        return mapIndex;
    }

    public string GetMapSceneNameString()
    {
        return MapList.Instance.GetMapSceneNameString(GetMapIndex());
    }

    private void Lobby_NextMap()
    {
        mapIndex++;

        if (mapIndex >= mapListInt.Count)
            mapIndex = 0;

        LobbyEvents.OnLobbyMapChange?.Invoke(GetMapSceneNameString());
    }

    private void Lobby_PreviousMap()
    {
        mapIndex--;

        if (mapIndex < 0)
            mapIndex = 3;

        LobbyEvents.OnLobbyMapChange?.Invoke(GetMapSceneNameString());
    }

    private void Lobby_SetMapName(Lobby lobby)
    {
        mapNameText.text = lobby.Data[key: "LobbyMap"].Value;
    }

    private void Lobby_SetMapImage(Lobby lobby)
    {
        string mapName = lobby.Data[key: "LobbyMap"].Value;

        switch (mapName)
        {
            case "BlueMap":
                mapImage.color = Color.blue;
                break;
            case "YellowMap":
                mapImage.color = Color.yellow;
                break;
            case "PurpleMap":
                mapImage.color = new Color(208, 0, 255, 255);
                break;
            case "RedMap":
                mapImage.color = Color.red;
                break;
        }
    }

    #endregion

    #region Player_Avatar:

    private void InitializeAvatarArrayButtonListeners()
    {
        for (int i = 0; i < avatarArrayBtn.Length; i++)
        {
            int currentIndex = i; // 'i' doesn't work properly as an int parameter passing so we use a new var instead.

            avatarArrayBtn[currentIndex].onClick.AddListener(() =>
            {
                SetNewPlayerAvatar(currentIndex);
                ClosePlayerAvatarPanelUI();
            });
        }
    }

    private void RemoveAvatarArrayButtonListeners()
    {
        for (int i = 0; i < avatarArrayBtn.Length; i++)
            avatarArrayBtn[i].onClick.RemoveAllListeners();
    }

    private void SetNewPlayerAvatar(int index)
    {
        switch (index)
        {
            case 0:
                LobbyEvents.OnPlayerAvatarConfirmed?.Invoke(LobbyManager.PlayerAvatarEnum.Heart);
                break;
            case 1:
                LobbyEvents.OnPlayerAvatarConfirmed?.Invoke(LobbyManager.PlayerAvatarEnum.Diamond);
                break;
            case 2:
                LobbyEvents.OnPlayerAvatarConfirmed?.Invoke(LobbyManager.PlayerAvatarEnum.Gold);
                break;
            case 3:
                LobbyEvents.OnPlayerAvatarConfirmed?.Invoke(LobbyManager.PlayerAvatarEnum.Star);
                break;
            case 4:
                LobbyEvents.OnPlayerAvatarConfirmed?.Invoke(LobbyManager.PlayerAvatarEnum.Lightning);
                break;
            default:
                break;
        }
    }

    private void Lobby_ResetPlayerAvatars(Lobby lobby)
    {
        for (int i = 0; i < lobby.MaxPlayers; i++)
            lobbyPlayerDatas[i].ResetPlayerAvatar();
    }

    private void Lobby_SyncPlayerAvatars(Lobby lobby)
    {
        for (int i = 0; i < lobby.Players.Count; i++)
            lobbyPlayerDatas[i].UpdatePlayerAvatar(lobbyPlayers[i]);
    }

    private void OpenPlayerAvatarPanelUI()
    {
        playerAvatarPanel.SetActive(true);
    }

    private void ClosePlayerAvatarPanelUI()
    {
        playerAvatarPanel.SetActive(false);
    }

    #endregion

    #region Lobby_Privacy:

    private void Event_OnLobbyPrivacyStateChange()
    {
        isPrivate = !isPrivate;
        Debug.Log("LobbyUI => IsPrivate: " + isPrivate);
        LobbyEvents.OnLobbyPrivacyStateChange?.Invoke(isPrivate);
    }

    private void Lobby_UpdateLobbyPrivacyText(bool state)
    {
        if (state)
            lobbyPrivacyText.text = privateLobby;
        else
            lobbyPrivacyText.text = publicLobby;
    }

    #endregion

    #region Player_Text:

    // Player Count:
    private void Lobby_UpdateLobbyPlayerCountText(Lobby lobby)
    {
        lobbyPlayerCount.text = lobby.Players.Count.ToString() + "/" + lobby.MaxPlayers.ToString();
    }

    // Lobby Code:
    private void Lobby_UpdateCodeNumberText(string lobbyCode)
    {
        lobbyCodeTextNumber.text = lobbyCode;
    }


    // Player Names:
    private void Lobby_SyncPlayersNames(Lobby lobby)
    {
        Lobby_SortPlayersList(lobby);
        Lobby_ClearPlayerNames(lobby);
        Lobby_UpdatePlayerNames(lobby);
    }

    private void Lobby_SortPlayersList(Lobby lobby)
    {
        lobbyPlayers.Clear();

        foreach (Player player in lobby.Players)
            lobbyPlayers.Add(player);

/*        for (int i = 0; i < lobby.Players.Count; i++)
            lobbyPlayers.Add(lobby.Players[i]);*/
    }

    private void Lobby_ClearPlayerNames(Lobby lobby) // NEEDS FIXING!
    {
        for (int i = 0; i < lobby.MaxPlayers - 1; i++)
            lobbyPlayerDatas[i].ResetPlayerNameText();
    }

    private void Lobby_UpdatePlayerNames(Lobby lobby)
    {
        for (int i = 0; i < lobby.Players.Count; i++)
            lobbyPlayerDatas[i].UpdatePlayerName(lobbyPlayers[i]);
    }

    #endregion

    #region Host/Client_Elements:

    private void Lobby_SyncPlayerKickButtons(Lobby lobby)
    {
        Lobby_DeactivatePlayerKickButtons(lobby);
        Lobby_ActivatePlayerKickButtons(lobby);
    }

    private void Lobby_DeactivatePlayerKickButtons(Lobby lobby)
    {
        for (int i = 1; i < lobby.MaxPlayers; i++) // lobby.MaxPlayers => currently works, monitoring for possible errors
            lobbyPlayerDatas[i].DeactivateKickButtons();
    }

    private void Lobby_ActivatePlayerKickButtons(Lobby lobby)
    {
        for (int i = 1; i < lobby.Players.Count; i++)
            lobbyPlayerDatas[i].ActivateKickButtons();
    }

    private void Lobby_DeactivateHostRelatedKickButtons(Lobby lobby)
    {
        if (LobbyManager.Instance.IsLobbyClient())
            for (int i = 1; i < lobby.MaxPlayers; i++) // lobby.MaxPlayers => currently works, monitoring for possible errors
                lobbyPlayerDatas[i].DeactivateKickButtons();
    }

    private void Lobby_DeactivateHostRelatedElementsOnClientSide()
    {
        DeactivateHostRelatedButtons();
        DeactivateHostsRelatedTexts();
    }

    private void DeactivateHostsRelatedTexts()
    {
        lobbyPrivacyText.gameObject.SetActive(false);
        lobbyCodeTextNumber.gameObject.SetActive(false);
        lobbyCodeText.gameObject.SetActive(false);
    }

    private void DeactivateHostRelatedButtons()
    {
        lobbyPrivacyBtn.gameObject.SetActive(false);
        startBtn.gameObject.SetActive(false);
        previousMapBtn.gameObject.SetActive(false);
        nextMapBtn.gameObject.SetActive(false);
    }

    #endregion

    #region JoinLobby_Functions:

    private void Event_OnJoiningLobbyByCode()
    {
        LobbyEvents.OnJoiningLobbyByCode?.Invoke(joinLobbyCodeInputField.text);
        Debug.Log("Joining lobby!");
    }

    private void Event_OnLeaveJoinLobbyUI()
    {
        LobbyEvents.OnLeaveJoinLobbyUI?.Invoke();
    }

    #endregion

}

/*    private void Lobby_InitializePrivacyStateToPrivate()
    {
        if (isPrivate)
             isPrivate = false;

        Lobby_UpdateLobbyPrivacyText(isPrivate);
    }*/
