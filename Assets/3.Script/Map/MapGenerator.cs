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
    [SerializeField]
    private Map[] hiddenMapList;
    [SerializeField]
    private List<Portal> portalPrefabList = new();
    public List<Map> mapList = new List<Map>();
    //맵 생성관련 인수
    public int maxMapNum; //맵최대갯수
    //부모객체 맵
    [SerializeField]
    public Transform map;
    //히든맵 부모객체
    [SerializeField]
    public GameObject hiddenMap;
    private int createMapNum; //생성할 맵의 갯수
    private int currentMapNum; //생성된 맵의갯수
    private int makeMapNum; //만들어야 할 맵의갯수(포탈이만들어진 갯수)
    private int specialMapNum; //생성할 스페셜맵의 갯수
    private int randomSpeicalMapNum; //생성할 랜덤스페셜맵의 갯수
    //맵 생성 확률인수
    [SerializeField]
    private int specialPercent;
    [SerializeField]
    private int randomSpecialPercent;
    //스테이지 정보 스크립트
    [SerializeField]
    List<Stage> stageInfoList = new();

    public bool currentMapClear;
    //맵이가진   
    private List<Portal> portalList = new List<Portal>();
    private List<Portal> connectPortalList = new List<Portal>();
    private List<List<Portal>> temPortalList = new List<List<Portal>>();
    TeleportManager telManager;
    CameraManager cameraManager;
    // Start is called before the first frame update
    void Start()
    {
        StageSet(stageInfoList[0]);
        currentMapNum = 0;
        cameraManager = CameraManager.Instance;
        telManager = TeleportManager.Instance;
        CreateMap();
        CreateHiddenMap();
        HideMap();
        telManager.StartPlayerTel();
    }
    private void Update()
    {
        foreach (Map map in mapList)
        {
            if (map.gameObject.activeSelf) // 활성화된 맵을 찾음
            {
                currentMapClear = map.isClear; // 활성화된 맵의 isClear 값을 currentMapClear로 설정
                break; // 첫 번째 활성화된 맵만 확인 후 종료
            }
        }
    }

    public void CreateMap()
    {
        CreateFirstMap();
        int createmapnum = maxMapNum - mapList.Count;
        for(int i = 0; i < createmapnum; i++)
        {
            InfiniteLoopDetector.Run();
            CreateNextMap();
        }
        PortalChange();
        cameraManager.CameraLimit(mapList[0].gameObject);

    }
    private void CreateFirstMap()//초기맵생성
    {
        while (true)
        {
            //만들어둔맵의 프리펩중에 랜덤으로 선택
            int mapIndex = Random.Range(0, firstMapPrefabList.Length);
            //그 맵의 타입이 시작맵일시 if문시작
            if (firstMapPrefabList[mapIndex].Type == Map.MapType.Start)
            {
                //맵생성
                Map firstMap = Instantiate(firstMapPrefabList[mapIndex]);
                // 생성된 Map 오브젝트를 map의 자식으로 추가
                firstMap.transform.SetParent(map, false);
                // 생성된 Map 오브젝트를 map의 자식 목록에서 마지막으로 위치시키기
                firstMap.transform.SetAsLastSibling();
                //만든맵을 맵리스트에추가
                mapList.Add(firstMap);
                //만든맵의 포탈리스트를 가져옴
                TakePortal(firstMap, portalList);
                //연결된 포탈의 갯수를 확인해 만들맵 갯수를 확인
                MapCheck();
                //포탈 갯수에 따라 맵을만듬
                switch (portalList.Count)
                {
                    //포탈이 1일경우
                    case 1:
                        while (true)
                        {
                            //중간맵 리스트에서 랜덤하게 포탈4개짜리 맵이 나올때까지 반복
                            int middleMapIndex = Random.Range(0, normalMapPrefabList.Length);
                            if (normalMapPrefabList[middleMapIndex].PortalNum == 4)
                            {
                                //맵을만듬
                                Map middleMap = Instantiate(normalMapPrefabList[middleMapIndex]);
                                //맵을 맵오브젝트의 자식객체에 넣음
                                middleMap.transform.SetParent(map, false);
                                //맵을 맵리스트에넣음
                                mapList.Add(middleMap);
                                //만드맵의 포탈을가져옴
                                TakePortal(middleMap, connectPortalList);
                                //만든맵과 이전맵의 포탈을연결함
                                ConnectPortal();
                                break;
                            }
                        }
                        break;
                        //포탈이2개일경우
                    case 2:
                        for (int i = 0; i < 2; i++)
                        {
                            //
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
                        //3개일경우
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
        //만들어야할 맵포탈 확인
        int createMapIndex = CheckPortal();
        for (int i = 0; i < createMapIndex; i++)
        {
            if (maxMapNum - mapList.Count == 1)
            {
                int lastMapIndex = Random.Range(0, bossMapPrefabList.Length);
                Map makeMap = Instantiate(bossMapPrefabList[lastMapIndex]);
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
            //만들어야할 맵의갯수르롹인
            MapCheck();
            //맵생성될 확률계산
            int mapNum = PersentCheck();
            //특수맵이 가능한지 불가능한지 확인
            if (AbleMapCheck(mapNum))
            {
                switch (mapNum)
                {
                    //랜덤특수맵생성
                    case 1: CreateRandomSpecialMap();
                        continue;
                        //특수맵생성
                    case 2: CreateSpecialMap();
                        continue;
                        //일반맵생성
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
    
    //포탈확인
    private int CheckPortal()
    {
        //맵리스트 전체를 확인하면서 연결이안된 포탈이 있는맵을찾기
        for (int i = 0; i < mapList.Count; i++)
        {
            TakePortal(mapList[i], portalList);
            if (portalList.Count > 0)
            {
                //연결안된 포탈이 한개라도있으면 연결안된 포탈갯수를 연결할 포탈리스트에 넣고 종료
                return portalList.Count;
            }
        }
        return 0;
    }
    //포탈 가져오기
    private void TakePortal(Map map, List<Portal> portalList)
    {
        //기존포탈리스트를 초기화
        portalList.Clear();
        //맵의 포탈을 전부가져옴
        Portal[] portals = map.GetComponentsInChildren<Portal>();
        // 포탈들을 포탈 리스트에 추가
        foreach (Portal portal in portals)
        {
            portalList.Add(portal);
        }
        //포탈에 연결된 포탈이있으면 리스트에서 제거
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
        portalList[0].connectPortal = connectPortalList[0];
        connectPortalList[0].connectPortal = portalList[0];
        portalList.RemoveAt(0);
        connectPortalList.RemoveAt(0);
        MapCheck();
    }

    //맵확인
    private void MapCheck()
    {
        //현제 맵의 갯수를 설정
        currentMapNum = mapList.Count;
        //만들어야할 맵의 갯수를 0으로설정
        makeMapNum = 0;
        //맵리스트검사
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
            InfiniteLoopDetector.Run();
            int mapIndex = Random.Range(0, normalMapPrefabList.Length);
            if (makeMapNum == 1 && maxMapNum - mapList.Count != 1)
            {
                if (normalMapPrefabList[mapIndex].PortalNum == 1)
                {
                    continue;
                }
                else
                {
                    Map makeMap = Instantiate(normalMapPrefabList[mapIndex]);
                    // 생성된 Map 오브젝트를 map의 자식으로 추가
                    makeMap.transform.SetParent(map, false);
                    // 생성된 Map 오브젝트를 map의 자식 목록에서 마지막으로 위치시키기
                    makeMap.transform.SetAsLastSibling();
                    //맵만들면 만든맵이름을 스크립트의 지정한 변수 맵이름으로 변경
                    //makeMap.name = makeMap.mapName;
                    mapList.Add(makeMap);
                    TakePortal(makeMap, connectPortalList);
                    ConnectPortal();
                    MapCheck();
                    break;
                }
            }
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
        while (true)
        {
            int mapIndex = Random.Range(0, specialMapPrefabList.Length);  
            bool isDuplicate = false;  // 중복 여부를 확인할 변수

            // 현재 mapList에 같은 타입의 맵이 있는지 확인
            for (int i = 0; i < mapList.Count; i++)
            {
                if (specialMapPrefabList[mapIndex].Type == mapList[i].Type)
                {
                    isDuplicate = true; // 같은 타입의 맵이 존재할 경우 true
                    i=mapList.Count; // 중복이 발견되면 더 이상 체크하지 않고 반복문 종료
                }
            }
            // 중복되지 않는 경우에만 맵을 생성
            if (!isDuplicate)
            {
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
                if (randomSpecialMapPrefabList[mapIndex].mapName == mapList[i].mapName)
                {
                    isDuplicate = true; // 같은 타입의 맵이 존재할 경우 true
                    continue; // 중복이 발견되면 더 이상 체크하지 않고 반복문 종료
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
    //해당맵을 만들수있나 체크
    private bool AbleMapCheck(int i)
    {
        if(makeMapNum == 1 &&maxMapNum-mapList.Count!=1)
        {
            return false;
        }
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
        for(int j = 0; j < hiddenMapList.Length; j++)
        {
            hiddenMap.transform.GetChild(j).gameObject.SetActive(false);
        }
        mapList[0].gameObject.SetActive(true);
    }
    
    //포탈 연결된맵의 종류에 따라 변경
    private void PortalChange()
    {
        for(int i = 0; i<mapList.Count; i++)
        {
            Map currentMap = mapList[i];
           
            Portal[] curretPortaList = currentMap.GetComponentsInChildren<Portal>();
            if (curretPortaList != null)
            {
                for(int j = 0; j < curretPortaList.Length; j++)
                {
                    Map connectMap = curretPortaList[j].connectPortal.GetComponentInParent<Map>();
                    if(connectMap.Type != Map.MapType.Middle)
                    {
                        Portal changePortal = new();
                        switch (connectMap.Type) 
                        {
                            
                            case Map.MapType.Download: 
                                changePortal = Instantiate(portalPrefabList[0],currentMap.transform);
                                changePortal.transform.position = curretPortaList[j].transform.position;
                                changePortal.transform.SetSiblingIndex(curretPortaList[j].transform.GetSiblingIndex());
                                changePortal.connectPortal = curretPortaList[j].connectPortal;
                                curretPortaList[j].connectPortal.connectPortal = changePortal;
                                Destroy(curretPortaList[j].gameObject);
                                break;
                            case Map.MapType.Shop:
                                changePortal = Instantiate(portalPrefabList[1], currentMap.transform);
                                changePortal.transform.position = curretPortaList[j].transform.position;
                                changePortal.transform.SetSiblingIndex(curretPortaList[j].transform.GetSiblingIndex());
                                changePortal.connectPortal = curretPortaList[j].connectPortal;
                                curretPortaList[j].connectPortal.connectPortal = changePortal;
                                Destroy(curretPortaList[j].gameObject);
                                break;
                            case Map.MapType.Boss:
                                changePortal = Instantiate(portalPrefabList[2], currentMap.transform);
                                changePortal.transform.position = curretPortaList[j].transform.position;
                                changePortal.transform.SetSiblingIndex(curretPortaList[j].transform.GetSiblingIndex());
                                changePortal.connectPortal = curretPortaList[j].connectPortal;
                                curretPortaList[j].connectPortal.connectPortal = changePortal;
                                Destroy(curretPortaList[j].gameObject);
                                break;
                             case Map.MapType.RandomSpecial:
                                switch (connectMap.mapName) 
                                {
                                    case "전원 옵션":
                                        changePortal = Instantiate(portalPrefabList[3], currentMap.transform);
                                        changePortal.transform.position = curretPortaList[j].transform.position;
                                        changePortal.transform.SetSiblingIndex(curretPortaList[j].transform.GetSiblingIndex());
                                        changePortal.connectPortal = curretPortaList[j].connectPortal;
                                        curretPortaList[j].connectPortal.connectPortal = changePortal;
                                        Destroy(curretPortaList[j].gameObject);
                                        break;
                                    case "Window 방화벽":
                                        changePortal = Instantiate(portalPrefabList[4], currentMap.transform);
                                        changePortal.transform.position = curretPortaList[j].transform.position;
                                        changePortal.transform.SetSiblingIndex(curretPortaList[j].transform.GetSiblingIndex());
                                        changePortal.connectPortal = curretPortaList[j].connectPortal;
                                        curretPortaList[j].connectPortal.connectPortal = changePortal;
                                        Destroy(curretPortaList[j].gameObject);
                                        break;
                                    case "JuvaCafe":
                                        changePortal = Instantiate(portalPrefabList[5], currentMap.transform);
                                        changePortal.transform.position = curretPortaList[j].transform.position;
                                        changePortal.transform.SetSiblingIndex(curretPortaList[j].transform.GetSiblingIndex());
                                        changePortal.connectPortal = curretPortaList[j].connectPortal;
                                        curretPortaList[j].connectPortal.connectPortal = changePortal;
                                        Destroy(curretPortaList[j].gameObject);
                                        break;
                                    case "휴지통":
                                        changePortal = Instantiate(portalPrefabList[6], currentMap.transform);
                                        changePortal.transform.position = curretPortaList[j].transform.position;
                                        changePortal.transform.SetSiblingIndex(curretPortaList[j].transform.GetSiblingIndex());
                                        changePortal.connectPortal = curretPortaList[j].connectPortal;
                                        curretPortaList[j].connectPortal.connectPortal = changePortal;
                                        Destroy(curretPortaList[j].gameObject);
                                        break;
                                }
                                break;
                        }
                    }
                }
            }
            // 현재 Map의 모든 자식 Transform을 가져오기
            /*foreach (Transform child in currentMap.transform)
            {
                // 자식 객체에서 Portal 컴포넌트를 가져오기
                Portal portal = child.GetComponent<Portal>();

                // Portal이 null이 아니면 (즉, Portal 컴포넌트가 존재하면)
                if (portal != null)
                {
                    Map connectMap = portal.connectPortal.transform.GetComponentInParent<Map>();
                    Portal connectPortal = portal.connectPortal;
                    if (connectMap.Type != Map.MapType.Middle)
                    {
                        switch (connectMap.Type) 
                        {
                            case Map.MapType.Download:
                                Vector2 portalPos = portal.transform.position;
                                int portalIndex = portal.transform.GetSiblingIndex();
                                Destroy(portal);
                                Portal downloadPortal = Instantiate(portalPrefabList[0]);
                                downloadPortal.transform.parent = currentMap.transform;
                                downloadPortal.transform.SetSiblingIndex(portalIndex);
                                downloadPortal.connectPortal = connectPortal;
                                connectPortal.connectPortal = portal;
                                break;
                            case Map.MapType.Shop:
                                Portal shopPortal = Instantiate(portalPrefabList[1], portal.transform);
                                shopPortal.connectPortal = connectPortal;
                                connectPortal.connectPortal = shopPortal;
                                Destroy(portal);
                                break;
                            case Map.MapType.Boss:
                                Portal bossPortal = Instantiate(portalPrefabList[2], portal.transform);
                                bossPortal.connectPortal = connectPortal;
                                connectPortal.connectPortal = bossPortal;
                                Destroy(portal);
                                break;
                            case Map.MapType.RandomSpecial:
                                switch (connectMap.mapName) 
                                {
                                    case "전원 옵션":
                                        Portal chargePortal = Instantiate(portalPrefabList[3], portal.transform);
                                        chargePortal.connectPortal = connectPortal;
                                        connectPortal.connectPortal = chargePortal;
                                        Destroy(portal);
                                        break;
                                    case "Window 방화벽":
                                        Portal gaurdPortal = Instantiate(portalPrefabList[4], portal.transform);
                                        gaurdPortal.connectPortal = connectPortal;
                                        connectPortal.connectPortal = gaurdPortal;
                                        Destroy(portal);
                                        break;
                                    case "JuvaCafe":
                                        Portal juvaPortal = Instantiate(portalPrefabList[5], portal.transform);
                                        juvaPortal.connectPortal = connectPortal;
                                        connectPortal.connectPortal = juvaPortal;
                                        Destroy(portal);
                                        break;
                                    case "휴지통":
                                        Portal trashPortal = Instantiate(portalPrefabList[6], portal.transform);
                                        trashPortal.connectPortal = connectPortal;
                                        connectPortal.connectPortal = trashPortal;
                                        Destroy(portal);
                                        break;
                                }
                                break;

                        }
                    }
                }
            }*/
        }
    }

    //맵초기화
    public void RestMap()
    {
        int childCount = map.transform.childCount;

        // 자식들이 파괴되면서 즉시 목록에서 제거되면, 올바르게 루프가 돌지 않으므로 뒤에서부터 처리합니다.
        for (int i = childCount - 1; i >= 0; i--)
        {
            // 자식 객체를 가져옴
            Transform child = map.transform.GetChild(i);
            // 자식 객체를 파괴
            Destroy(child.gameObject);
        }
        mapList.Clear();
        StageSet(stageInfoList[0]);
        currentMapNum = 0;
        CreateMap();
        HideMap();
        telManager.StartPlayerTel();
    }
    //스테이지 정보에맞게 만들맵의 정보를 갱신
    public void StageSet(Stage stage)
    {
        maxMapNum = stage.maxMapNum;
        specialMapNum = stage.specialMapNum;
        randomSpeicalMapNum = stage.randomSpeicalMapNum;
    }
    public void CreateHiddenMap()
    {
        for(int i = 0; i < hiddenMapList.Length; i++)
        {
           Instantiate(hiddenMapList[i],hiddenMap.transform);
        }
    }
}
