using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapList : MonoBehaviour
{
    public static MapList Instance;

    [SerializeField] private List<string> mapListString = new List<string>();

    private void Awake()
    {
        Instance = this;
    }

    public string GetMapSceneNameString(int index)
    {
        return mapListString[index];
    }

}
