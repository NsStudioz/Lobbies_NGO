using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerData : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerName;
    private readonly string emptyPlayerName = "<EMPTY SLOT>";

    private Player player;

    private void Start()
    {
        ResetPlayerNameText();
    }

    // For when a player in not populating this data.
    public void ResetPlayerNameText()
    {
        playerName.text = emptyPlayerName;
    }

    public void UpdatePlayerName(Player player)
    {
        this.player = player;
        playerName.text = this.player.Data[LobbyManager.KEY_PLAYER_NAME].Value;
    }

}
