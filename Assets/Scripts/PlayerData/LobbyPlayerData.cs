using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Authentication;

public class LobbyPlayerData : MonoBehaviour
{

    [SerializeField] private Image playerAvatar = null;
    [SerializeField] private TextMeshProUGUI playerName;
    private readonly string emptyPlayerName = "<EMPTY SLOT>";
    [SerializeField] private Button kickPlayerBtn;

    private Player player;

    private void Start()
    {
        ResetPlayerNameText();

        kickPlayerBtn.onClick.AddListener(KickThisPlayer);
    }


    private void OnDisable()
    {
        kickPlayerBtn.onClick.RemoveAllListeners();
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

    public void ResetPlayerAvatar()
    {
        this.playerAvatar.sprite = PlayerAvatar.Instance.GetSprite(LobbyManager.PlayerAvatarEnum.Heart);
    }

    public void UpdatePlayerAvatar(Player player)
    {
        this.player = player;
        // Convert enum type to string.
        LobbyManager.PlayerAvatarEnum playerAvatar = 
                    Enum.Parse<LobbyManager.PlayerAvatarEnum>(player.Data[LobbyManager.KEY_PLAYER_AVATAR].Value);

        this.playerAvatar.sprite = PlayerAvatar.Instance.GetSprite(playerAvatar);
    }

    public void DeactivateKickButtons()
    {
        kickPlayerBtn.enabled = false;
        kickPlayerBtn.gameObject.SetActive(false);
    }

    public void ActivateKickButtons()
    {
        kickPlayerBtn.enabled = true;
        kickPlayerBtn.gameObject.SetActive(true);
    }

    private void KickThisPlayer()
    {
        if (player != null)
        {
            //LobbyManager.Instance.TryCatch_KickPlayer(player.Id);
            LobbyEvents.OnPlayerKicked?.Invoke(player.Id);
            // maybe should put player = null, to allow multiple kicks of same reconnecting player. Related to kick function.
        }
    }

}
