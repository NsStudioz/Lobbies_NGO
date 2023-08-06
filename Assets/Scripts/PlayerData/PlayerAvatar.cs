using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerAvatar: MonoBehaviour
{
    public static PlayerAvatar Instance;

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] private List<Sprite> playerAvatarList = new List<Sprite>();
    //  0 = Heart, 1 = Diamond, 2 = Gold, 3 = Star, 4 = Lightning

    public Sprite GetSprite(LobbyManager.PlayerAvatarEnum playerAvatar)
    {
        return playerAvatar switch
        {
            LobbyManager.PlayerAvatarEnum.Diamond   => playerAvatarList[1],
            LobbyManager.PlayerAvatarEnum.Gold      => playerAvatarList[2],
            LobbyManager.PlayerAvatarEnum.Star      => playerAvatarList[3],
            LobbyManager.PlayerAvatarEnum.Lightning => playerAvatarList[4],
            _ => playerAvatarList[0], // default = Heart
        };
    }

}
