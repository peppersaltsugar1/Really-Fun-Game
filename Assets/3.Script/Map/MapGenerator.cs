using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    //맵 리스트
    [SerializeField]
    private Map[] firstMapPrefabList;
    [SerializeField]
    private Map[] normalMapPrefabList;
    [SerializeField]
    private Map[] specialMapPrefabList;
    [SerializeField]
    private Map[] randomSpecialMapPrefabList;
    [SerializeField]
    private Map[] bossMapPrefabList;
    private List<Map> mapList = new List<Map>();
    //맵 생성관련 인수
    public int maxMapNum; //맵최대갯수
    private Transform map;
    [SerializeField]
    private int createMapNum; //생성할 맵의 갯수
    private int currentMapNum; //생성된 맵의갯수
    private int makeMapNum; //만들어야 할 맵의갯수(포탈이만들어진 갯수)
    [SerializeField]
    private int specialMapNum; //생성할 스페셜맵의 갯수
    [SerializeField]
    private int randomSpeicalMapNum; //생성할 랜덤스페셜맵의 갯수
    //맵 생성 확률인수
    [SerializeField]
    private int specialPercent;
    [SerializeField]
    private int randomSpecialPercent;
    //맵이가진   
    private List<Portal> portalList = new List<Portal>();
    private List<Portal> connectPortalList = new List<Portal>();
    private List<List<Portal>> temPortalList = new List<List<Portal>>();

    // Start is called before the first frame update
    void Start()
    {
        currentMapNum = 0;
        CreateMap();
        HideMap();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void CreateMap()
    {
        CreateFirstMap();
        int createmapnum = maxMapNum - mapList.Count;
        while (mapList.Count < maxMapNum)
        {
            CreateNextMap();
        }


    }
    private void CreateFirstMap()
    {
        while (true)
        {
            int mapIndex = Random.Range(0, firstMapPrefabList.Length);
            if (firstMapPrefabList[mapIndex].Type == Map.MapType.Start)
            {
                Map firstMap = Instantiate(firstMapPrefabList[mapIndex]);
                // 생성된 Map 오브젝트를 map의 자식으로 추가
                firstMap.transform.SetParent(map, false);
                // 생성된 Map 오브젝트를 map의 자식 목록에서 마지막으로 위치시키기
                firstMap.transform.SetAsLastSibling();
                mapList.Add(firstMap);
                TakePortal(firstMap, portalList);
                MapCheck();
                switch (portalList.Count)
                {
                    case 1:
                        while (true)
                        {
                            int middleMapIndex = Random.Range(0, normalMapPrefabList.Length);
                            if (normalMapPrefabList[middleMapIndex].PortalNum == 4)
                            {
                                Map middleMap = Instantiate(normalMapPrefabList[middleMapIndex]);
                                middleMap.transform.SetParent(map, false);
                                mapList.Add(middleMap);
                                TakePortal(middleMap, connectPortalList);
                                ConnectPortal();
                                break;
                            }
                        }
                        break;
                    case 2:
                        for (int i = 0; i < 2; i++)
                        {
                            int middleMapIndex = Random.Range(0, normalMapPrefabList.Length);
                            if (normalMapPrefabList[middleMapIndex].PortalNum >= 3)
                            {
                                Map middleMap = Instantiate(normalMapPrefabList[middleMapIndex]);
                                middleMap.transform.SetParent(map, false);
                                mapList.Add(middleMap);
                                TakePortal(middleMap, connectPortalList);
                                ConnectPortal();
                            }
                            else
                            {
                                i--;
                            }
                        }
                        break;

                    case 3:
                        for (int i = 0; i < 3; i++)
                        {
                            int middleMapIndex = Random.Range(0, normalMapPrefabList.Length);
                            if (normalMapPrefabList[middleMapIndex].PortalNum <= 3)
                            {
                                Map middleMap = Instantiate(normalMapPrefabList[middleMapIndex]);
                                middleMap.transform.SetParent(map, false);
                                mapList.Add(middleMap);
                                TakePortal(middleMap, connectPortalList);
                                ConnectPortal();
                            }
                            else
                            {
                                i--;
                            }
                        }
                        break;

                }
                break;
            }
        }
        currentMapNum = mapList.Count;
    }
    private void CreateNextMap()
    {
        int createMapIndex = CheckPortal();
        Debug.Log(createMapIndex);
        for (int i = 0; i < createMapIndex; i++)
        {
            MapCheck();
            int mapNum = PersentCheck();
            if (AbleMapCheck(mapNum))
            {
                switch (mapNum)
                {
                    case 1: CreateRandomSpecialMap();
                        continue;
                    case 2: CreateSpecialMap();
                        continue;
                    case 3: CreateNomalMap();
                        continue;
                }
            }
            else
            {
                CreateNomalMap();
            }
        }
    }
    private void CreateMiddleMap()
    {
        Debug.Log(makeMapNum);
        int makemapnum = makeMapNum;
        for (int i = 0; i < makemapnum; i++)
        {
            Debug.Log(currentMapNum);
            Debug.Log(mapList[i].name);
            TakePortal(mapList[i], portalList);
            if (portalList.Count >= 1)
            {
                int mapNum = PersentCheck();
                bool canMake = AbleMapCheck(mapNum);
                Debug.Log(mapNum);
                Debug.Log(canMake);
                switch (mapNum)
                {
                    case 1:
                        if (canMake)
                        {
                            CreateRandomSpecialMap();
                        }
                        else
                        {
                            CreateNomalMap();
                        }
                        break;
                    case 2:
                        if (canMake)
                        {
                            CreateSpecialMap();
                        }
                        else
                        {
                            CreateNomalMap();
                        }
                        break;
                    case 3:
                        CreateNomalMap();
                        break;
                }

            }

        }
        }

    private int CheckPortal()
    {
        for (int i = 0; i < mapList.Count; i++)
        {
            TakePortal(mapList[i], portalList);
            if (portalList.Count > 0)
            {
                return portalList.Count;
            }
        }
        return 0;
    }
    private void TakePortal(Map map, List<Portal> portalList)
    {
        portalList.Clear();
        Portal[] portals = map.GetComponentsInChildren<Portal>();
        // 포탈들을 포탈 리스트에 추가
        foreach (Portal portal in portals)
        {
            portalList.Add(portal);
        }
        for (int i = portalList.Count - 1; i >= 0; i--)
        {
            if (portalList[i].connectPortal != null)
            {
                portalList.RemoveAt(i);
            }
        }
        return;
    }
    private void ConnectPortal()
    {
        Debug.Log(portalList.Count);
        Debug.Log(connectPortalList.Count);
        portalList[0].connectPortal = connectPortalList[0];
        connectPortalList[0].connectPortal = portalList[0];
        portalList.RemoveAt(0);
        connectPortalList.RemoveAt(0);
        MapCheck();
    }

    private void MapCheck()
    {
        currentMapNum = mapList.Count;
        makeMapNum = 0;
        for(int i = 0; i < mapList.Count; i++)
        {
            // 각 맵의 자식 객체로 있는 포탈 가져오기
            Portal[] portals = mapList[i].GetComponentsInChildren<Portal>();

            foreach (Portal portal in portals)
            {
                // 포탈의 connectPortal이 null인 경우
                if (portal.connectPortal == null)
                {
                    makeMapNum++; // 생성할 맵 갯수 증가
                }
            }
        }
    }

    private int PersentCheck()//확률의따라 int값을 반환
    {
        if (makeMapNum==maxMapNum)
        {
            return 3;
        }
        float randomValue = Random.Range(0f, 100f);

        if (randomValue <= randomSpecialPercent)
        {
            return 1;
        }
        else if (randomValue <= specialPercent)
        {
            return 2;
        }
        else
        {
            return 3;
        }
    }
    private void CreateNomalMap()
    {
        while (true)
        {
            int mapIndex = Random.Range(0, normalMapPrefabList.Length);
            if (makeMapNum + mapList.Count + normalMapPrefabList[mapIndex].PortalNum-1 <= maxMapNum)
            {
                if (normalMapPrefabList[mapIndex].PortalNum == 1)
                {
                    if (randomSpeicalMapNum > 0)
                    {
                        CreateRandomSpecialMap();
                        break;
                    }
                    else if (specialMapNum > 0)
                    {
                        CreateSpecialMap();
                        break;
                    }
                }
                if (currentMapNum == 1)
                {
                    if (normalMapPrefabList[mapIndex].PortalNum >= 2)
                    {
                        Map makeMap = Instantiate(normalMapPrefabList[mapIndex]);
                        // 생성된 Map 오브젝트를 map의 자식으로 추가
                        makeMap.transform.SetParent(map, false);
                        // 생성된 Map 오브젝트를 map의 자식 목록에서 마지막으로 위치시키기
                        makeMap.transform.SetAsLastSibling();
                        mapList.Add(makeMap);
                        TakePortal(makeMap, connectPortalList);
                        ConnectPortal();
                        MapCheck();
                        break;
                    }
                }
                else if (normalMapPrefabList[mapIndex].PortalNum - 1 + currentMapNum <= maxMapNum)
                {
                    Map makeMap = Instantiate(normalMapPrefabList[mapIndex]);
                    // 생성된 Map 오브젝트를 map의 자식으로 추가
                    makeMap.transform.SetParent(map, false);
                    // 생성된 Map 오브젝트를 map의 자식 목록에서 마지막으로 위치시키기
                    makeMap.transform.SetAsLastSibling();
                    mapList.Add(makeMap);
                    TakePortal(makeMap, connectPortalList);
                    ConnectPortal();
                    MapCheck();
                    break;
                }
                else if (mapList.Count + makeMapNum == maxMapNum)
                {
                    while (true)
                    {
                        int lastMapIndex = Random.Range(0, normalMapPrefabList.Length);
                        if (normalMapPrefabList[lastMapIndex].PortalNum == 1)
                        {
                            Map makeMap = Instantiate(normalMapPrefabList[lastMapIndex]);
                            // 생성된 Map 오브젝트를 map의 자식으로 추가
                            makeMap.transform.SetParent(map, false);
                            // 생성된 Map 오브젝트를 map의 자식 목록에서 마지막으로 위치시키기
                            makeMap.transform.SetAsLastSibling();
                            mapList.Add(makeMap);
                            TakePortal(makeMap, connectPortalList);
                            ConnectPortal();
                            MapCheck();
                            break;
                        }
                    }
                }
            }

        }

    }
    private void CreateSpecialMap()
    {
        Debug.Log("스페셜맵생성은들어옴");
        while (true)
        {
            int mapIndex = Random.Range(0, specialMapPrefabList.Length);  
            bool isDuplicate = false;  // 중복 여부를 확인할 변수

            // 현재 mapList에 같은 타입의 맵이 있는지 확인
            for (int i = 0; i < mapList.Count; i++)
            {
                Debug.Log(isDuplicate);
                Debug.Log(specialMapPrefabList[mapIndex].Type);
                Debug.Log(mapList[i].Type);
                if (specialMapPrefabList[mapIndex].Type == mapList[i].Type)
                {
                    Debug.Log("맵타입같나 체크함");
                    isDuplicate = true; // 같은 타입의 맵이 존재할 경우 true
                    i=mapList.Count; // 중복이 발견되면 더 이상 체크하지 않고 반복문 종료
                }
            }
            // 중복되지 않는 경우에만 맵을 생성
            if (!isDuplicate)
            {
                Debug.Log("스페셜맵만들러들어옴");
                Map makeMap = Instantiate(specialMapPrefabList[mapIndex]);
                // 생성된 Map 오브젝트를 map의 자식으로 추가
                makeMap.transform.SetParent(map, false);
                // 생성된 Map 오브젝트를 map의 자식 목록에서 마지막으로 위치시키기
                makeMap.transform.SetAsLastSibling();
                // mapList에 새롭게 생성된 맵을 추가
                mapList.Add(makeMap);
                // 포탈 리스트 갱신 및 확인
                TakePortal(makeMap, connectPortalList);
                ConnectPortal();
                MapCheck();
                specialMapNum--;
                break; // 중복되지 않는 맵을 생성했으므로 while문 종료
            }
            // 중복되면 다시 while 문 반복
        }
    }
    private void CreateRandomSpecialMap()
    {
        while (true)
        {
            int mapIndex = Random.Range(0, randomSpecialMapPrefabList.Length);
            bool isDuplicate = false;  // 중복 여부를 확인할 변수

            // 현재 mapList에 같은 타입의 맵이 있는지 확인
            for (int i = 0; i < mapList.Count; i++)
            {
                if (randomSpecialMapPrefabList[mapIndex].name == mapList[i].name)
                {
                    isDuplicate = true; // 같은 타입의 맵이 존재할 경우 true
                    break; // 중복이 발견되면 더 이상 체크하지 않고 반복문 종료
                }
            }
            // 중복되지 않는 경우에만 맵을 생성
            if (!isDuplicate)
            {
                Map makeMap = Instantiate(randomSpecialMapPrefabList[mapIndex]);
                // 생성된 Map 오브젝트를 map의 자식으로 추가
                makeMap.transform.SetParent(map, false);
                // 생성된 Map 오브젝트를 map의 자식 목록에서 마지막으로 위치시키기
                makeMap.transform.SetAsLastSibling();
                // mapList에 새롭게 생성된 맵을 추가
                mapList.Add(makeMap);
                // 포탈 리스트 갱신 및 확인
                TakePortal(makeMap, connectPortalList);
                ConnectPortal();
                MapCheck();
                randomSpeicalMapNum--;
                break; // 중복되지 않는 맵을 생성했으므로 while문 종료
            }

            // 중복되면 다시 while 문 반복
        }
    }
    private bool AbleMapCheck(int i)
    {
        if (makeMapNum == 1 && (specialMapNum == 0 || randomSpeicalMapNum == 0))
        {
            return false;
        }
        if (makeMapNum + mapList.Count <= maxMapNum)
        {
            switch (i)
            {
                case 1:
                    if (randomSpeicalMapNum > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case 2:
                    if (specialMapNum > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case 3:
                    return true;
            }
            return false;
        }
        else
        {
            return false;
        }
    }
    private void HideMap()
    {
        for (int i = 0; i < mapList.Count; i++)
        {
            mapList[i].gameObject.SetActive(false);
        }
        mapList[0].gameObject.SetActive(true);
    }
}
