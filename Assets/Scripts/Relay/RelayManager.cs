using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelayManager : MonoBehaviour
{

    public static RelayManager Instance;

    void Awake()
    {
        Instance = this;
    }




}
