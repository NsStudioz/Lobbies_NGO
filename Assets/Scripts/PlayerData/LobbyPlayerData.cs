using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerData : MonoBehaviour
{
    [SerializeField] private Sprite playerAvatar;
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
            LobbyManager.Instance.TryCatch_KickPlayer(player.Id);
        }
    }

}
