using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public enum MapType { Start, Boss, Shop,Download, RandomSpecial, Hidden, Middle, End, MiddleBoss }
    public MapType Type;
    public int PortalNum;
    public string mapName;
    public bool isClear;

    public float nowPosition;

    void Awake()
    {
        if (Type == MapType.Start)
        {
            isClear = true;
        }
        else
        {
            isClear = false;
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        nowPosition = transform.position.x;
    }
}
