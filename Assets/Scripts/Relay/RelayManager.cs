using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelayManager : MonoBehaviour
{

    public static RelayManager Instance;
    private const int maxPlayers = 3; // only clients are counted here, host is already considered as a connection,
                                      // therefore the host is not included as a connection here.

    void Awake()
    {
        Instance = this;
    }




}
