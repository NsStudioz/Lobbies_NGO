using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class JoinLobbyUI : MonoBehaviour
{

    //private List<Lobby> lobbiesToQuery = new List<Lobby>(6);                     // Available public lobbies.
    [SerializeField] private List<TMP_Text> lobbyNames = new List<TMP_Text>();   // Available public lobbies.
    [SerializeField] private List<TMP_Text> lobbyPlayers = new List<TMP_Text>(); // Available players in public lobby.

}
