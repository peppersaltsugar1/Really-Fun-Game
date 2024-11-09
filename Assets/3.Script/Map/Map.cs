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
    public int currentMonsterNum;
    UIManager uiManager;

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
        uiManager = UIManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        nowPosition = transform.position.x;
        MonsterCheck();
        MapClearCheck();
        
    }

    public void MapClearCheck()
    {
        if (Type != Map.MapType.Start)
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).CompareTag("Monster"))
                {
                    isClear = false;
                    return;
                }
            }
            isClear = true;
        }
    }
    public void MonsterCheck()
    {
        int checkMonsterNum = 0;
        if (Type != Map.MapType.Start)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).CompareTag("Monster"))
                {
                    checkMonsterNum++;
                }
            }
            currentMonsterNum = checkMonsterNum;
            uiManager.MonsterCountHUDSet(checkMonsterNum);
        }
        else
        {
            currentMonsterNum = 0;
            uiManager.MonsterCountHUDSet(checkMonsterNum);
        }
    }
}
