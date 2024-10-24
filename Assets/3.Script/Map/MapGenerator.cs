using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    //�� ����Ʈ
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
    //�� �������� �μ�
    public int maxMapNum; //���ִ밹��
    //�θ�ü ��
    [SerializeField]
    public Transform map;
    //����� �θ�ü
    [SerializeField]
    public GameObject hiddenMap;
    private int createMapNum; //������ ���� ����
    private int currentMapNum; //������ ���ǰ���
    private int makeMapNum; //������ �� ���ǰ���(��Ż�̸������ ����)
    private int specialMapNum; //������ ����ȸ��� ����
    private int randomSpeicalMapNum; //������ ��������ȸ��� ����
    //�� ���� Ȯ���μ�
    [SerializeField]
    private int specialPercent;
    [SerializeField]
    private int randomSpecialPercent;
    //�������� ���� ��ũ��Ʈ
    [SerializeField]
    List<Stage> stageInfoList = new();

    public bool currentMapClear;
    //���̰���   
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
            if (map.gameObject.activeSelf) // Ȱ��ȭ�� ���� ã��
            {
                currentMapClear = map.isClear; // Ȱ��ȭ�� ���� isClear ���� currentMapClear�� ����
                break; // ù ��° Ȱ��ȭ�� �ʸ� Ȯ�� �� ����
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
    private void CreateFirstMap()//�ʱ�ʻ���
    {
        while (true)
        {
            //�����и��� �������߿� �������� ����
            int mapIndex = Random.Range(0, firstMapPrefabList.Length);
            //�� ���� Ÿ���� ���۸��Ͻ� if������
            if (firstMapPrefabList[mapIndex].Type == Map.MapType.Start)
            {
                //�ʻ���
                Map firstMap = Instantiate(firstMapPrefabList[mapIndex]);
                // ������ Map ������Ʈ�� map�� �ڽ����� �߰�
                firstMap.transform.SetParent(map, false);
                // ������ Map ������Ʈ�� map�� �ڽ� ��Ͽ��� ���������� ��ġ��Ű��
                firstMap.transform.SetAsLastSibling();
                //������� �ʸ���Ʈ���߰�
                mapList.Add(firstMap);
                //������� ��Ż����Ʈ�� ������
                TakePortal(firstMap, portalList);
                //����� ��Ż�� ������ Ȯ���� ����� ������ Ȯ��
                MapCheck();
                //��Ż ������ ���� ��������
                switch (portalList.Count)
                {
                    //��Ż�� 1�ϰ��
                    case 1:
                        while (true)
                        {
                            //�߰��� ����Ʈ���� �����ϰ� ��Ż4��¥�� ���� ���ö����� �ݺ�
                            int middleMapIndex = Random.Range(0, normalMapPrefabList.Length);
                            if (normalMapPrefabList[middleMapIndex].PortalNum == 4)
                            {
                                //��������
                                Map middleMap = Instantiate(normalMapPrefabList[middleMapIndex]);
                                //���� �ʿ�����Ʈ�� �ڽİ�ü�� ����
                                middleMap.transform.SetParent(map, false);
                                //���� �ʸ���Ʈ������
                                mapList.Add(middleMap);
                                //������� ��Ż��������
                                TakePortal(middleMap, connectPortalList);
                                //����ʰ� �������� ��Ż��������
                                ConnectPortal();
                                break;
                            }
                        }
                        break;
                        //��Ż��2���ϰ��
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
                        //3���ϰ��
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
        //�������� ����Ż Ȯ��
        int createMapIndex = CheckPortal();
        for (int i = 0; i < createMapIndex; i++)
        {
            if (maxMapNum - mapList.Count == 1)
            {
                int lastMapIndex = Random.Range(0, bossMapPrefabList.Length);
                Map makeMap = Instantiate(bossMapPrefabList[lastMapIndex]);
                // ������ Map ������Ʈ�� map�� �ڽ����� �߰�
                makeMap.transform.SetParent(map, false);
                // ������ Map ������Ʈ�� map�� �ڽ� ��Ͽ��� ���������� ��ġ��Ű��
                makeMap.transform.SetAsLastSibling();
                mapList.Add(makeMap);
                TakePortal(makeMap, connectPortalList);
                ConnectPortal();
                MapCheck();
                break;
            }
            //�������� ���ǰ���������
            MapCheck();
            //�ʻ����� Ȯ�����
            int mapNum = PersentCheck();
            //Ư������ �������� �Ұ������� Ȯ��
            if (AbleMapCheck(mapNum))
            {
                switch (mapNum)
                {
                    //����Ư���ʻ���
                    case 1: CreateRandomSpecialMap();
                        continue;
                        //Ư���ʻ���
                    case 2: CreateSpecialMap();
                        continue;
                        //�Ϲݸʻ���
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
    
    //��ŻȮ��
    private int CheckPortal()
    {
        //�ʸ���Ʈ ��ü�� Ȯ���ϸ鼭 �����̾ȵ� ��Ż�� �ִ¸���ã��
        for (int i = 0; i < mapList.Count; i++)
        {
            TakePortal(mapList[i], portalList);
            if (portalList.Count > 0)
            {
                //����ȵ� ��Ż�� �Ѱ��������� ����ȵ� ��Ż������ ������ ��Ż����Ʈ�� �ְ� ����
                return portalList.Count;
            }
        }
        return 0;
    }
    //��Ż ��������
    private void TakePortal(Map map, List<Portal> portalList)
    {
        //������Ż����Ʈ�� �ʱ�ȭ
        portalList.Clear();
        //���� ��Ż�� ���ΰ�����
        Portal[] portals = map.GetComponentsInChildren<Portal>();
        // ��Ż���� ��Ż ����Ʈ�� �߰�
        foreach (Portal portal in portals)
        {
            portalList.Add(portal);
        }
        //��Ż�� ����� ��Ż�������� ����Ʈ���� ����
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

    //��Ȯ��
    private void MapCheck()
    {
        //���� ���� ������ ����
        currentMapNum = mapList.Count;
        //�������� ���� ������ 0���μ���
        makeMapNum = 0;
        //�ʸ���Ʈ�˻�
        for(int i = 0; i < mapList.Count; i++)
        {
            // �� ���� �ڽ� ��ü�� �ִ� ��Ż ��������
            Portal[] portals = mapList[i].GetComponentsInChildren<Portal>();
            
            foreach (Portal portal in portals)
            {
                // ��Ż�� connectPortal�� null�� ���
                if (portal.connectPortal == null)
                {
                    makeMapNum++; // ������ �� ���� ����
                }
            }
        }
    }

    private int PersentCheck()//Ȯ���ǵ��� int���� ��ȯ
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
                    // ������ Map ������Ʈ�� map�� �ڽ����� �߰�
                    makeMap.transform.SetParent(map, false);
                    // ������ Map ������Ʈ�� map�� �ڽ� ��Ͽ��� ���������� ��ġ��Ű��
                    makeMap.transform.SetAsLastSibling();
                    //�ʸ���� ������̸��� ��ũ��Ʈ�� ������ ���� ���̸����� ����
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
                        // ������ Map ������Ʈ�� map�� �ڽ����� �߰�
                        makeMap.transform.SetParent(map, false);
                        // ������ Map ������Ʈ�� map�� �ڽ� ��Ͽ��� ���������� ��ġ��Ű��
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
                    // ������ Map ������Ʈ�� map�� �ڽ����� �߰�
                    makeMap.transform.SetParent(map, false);
                    // ������ Map ������Ʈ�� map�� �ڽ� ��Ͽ��� ���������� ��ġ��Ű��
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
                            // ������ Map ������Ʈ�� map�� �ڽ����� �߰�
                            makeMap.transform.SetParent(map, false);
                            // ������ Map ������Ʈ�� map�� �ڽ� ��Ͽ��� ���������� ��ġ��Ű��
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
            bool isDuplicate = false;  // �ߺ� ���θ� Ȯ���� ����

            // ���� mapList�� ���� Ÿ���� ���� �ִ��� Ȯ��
            for (int i = 0; i < mapList.Count; i++)
            {
                if (specialMapPrefabList[mapIndex].Type == mapList[i].Type)
                {
                    isDuplicate = true; // ���� Ÿ���� ���� ������ ��� true
                    i=mapList.Count; // �ߺ��� �߰ߵǸ� �� �̻� üũ���� �ʰ� �ݺ��� ����
                }
            }
            // �ߺ����� �ʴ� ��쿡�� ���� ����
            if (!isDuplicate)
            {
                Map makeMap = Instantiate(specialMapPrefabList[mapIndex]);
                // ������ Map ������Ʈ�� map�� �ڽ����� �߰�
                makeMap.transform.SetParent(map, false);
                // ������ Map ������Ʈ�� map�� �ڽ� ��Ͽ��� ���������� ��ġ��Ű��
                makeMap.transform.SetAsLastSibling();
                // mapList�� ���Ӱ� ������ ���� �߰�
                mapList.Add(makeMap);
                // ��Ż ����Ʈ ���� �� Ȯ��
                TakePortal(makeMap, connectPortalList);
                ConnectPortal();
                MapCheck();
                specialMapNum--;
                break; // �ߺ����� �ʴ� ���� ���������Ƿ� while�� ����
            }
            // �ߺ��Ǹ� �ٽ� while �� �ݺ�
        }
    }
    private void CreateRandomSpecialMap()
    {
        while (true)
        {
            int mapIndex = Random.Range(0, randomSpecialMapPrefabList.Length);
            bool isDuplicate = false;  // �ߺ� ���θ� Ȯ���� ����

            // ���� mapList�� ���� Ÿ���� ���� �ִ��� Ȯ��
            for (int i = 0; i < mapList.Count; i++)
            {
                if (randomSpecialMapPrefabList[mapIndex].mapName == mapList[i].mapName)
                {
                    isDuplicate = true; // ���� Ÿ���� ���� ������ ��� true
                    continue; // �ߺ��� �߰ߵǸ� �� �̻� üũ���� �ʰ� �ݺ��� ����
                }
            }
            // �ߺ����� �ʴ� ��쿡�� ���� ����
            if (!isDuplicate)
            {
                Map makeMap = Instantiate(randomSpecialMapPrefabList[mapIndex]);
                // ������ Map ������Ʈ�� map�� �ڽ����� �߰�
                makeMap.transform.SetParent(map, false);
                // ������ Map ������Ʈ�� map�� �ڽ� ��Ͽ��� ���������� ��ġ��Ű��
                makeMap.transform.SetAsLastSibling();
                // mapList�� ���Ӱ� ������ ���� �߰�
                mapList.Add(makeMap);
                // ��Ż ����Ʈ ���� �� Ȯ��
                TakePortal(makeMap, connectPortalList);
                ConnectPortal();
                MapCheck();
                randomSpeicalMapNum--;
                break; // �ߺ����� �ʴ� ���� ���������Ƿ� while�� ����
            }

            // �ߺ��Ǹ� �ٽ� while �� �ݺ�
        }
    }
    //�ش���� ������ֳ� üũ
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
    
    //��Ż ����ȸ��� ������ ���� ����
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
                                    case "���� �ɼ�":
                                        changePortal = Instantiate(portalPrefabList[3], currentMap.transform);
                                        changePortal.transform.position = curretPortaList[j].transform.position;
                                        changePortal.transform.SetSiblingIndex(curretPortaList[j].transform.GetSiblingIndex());
                                        changePortal.connectPortal = curretPortaList[j].connectPortal;
                                        curretPortaList[j].connectPortal.connectPortal = changePortal;
                                        Destroy(curretPortaList[j].gameObject);
                                        break;
                                    case "Window ��ȭ��":
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
                                    case "������":
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
            // ���� Map�� ��� �ڽ� Transform�� ��������
            /*foreach (Transform child in currentMap.transform)
            {
                // �ڽ� ��ü���� Portal ������Ʈ�� ��������
                Portal portal = child.GetComponent<Portal>();

                // Portal�� null�� �ƴϸ� (��, Portal ������Ʈ�� �����ϸ�)
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
                                    case "���� �ɼ�":
                                        Portal chargePortal = Instantiate(portalPrefabList[3], portal.transform);
                                        chargePortal.connectPortal = connectPortal;
                                        connectPortal.connectPortal = chargePortal;
                                        Destroy(portal);
                                        break;
                                    case "Window ��ȭ��":
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
                                    case "������":
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

    //���ʱ�ȭ
    public void RestMap()
    {
        int childCount = map.transform.childCount;

        // �ڽĵ��� �ı��Ǹ鼭 ��� ��Ͽ��� ���ŵǸ�, �ùٸ��� ������ ���� �����Ƿ� �ڿ������� ó���մϴ�.
        for (int i = childCount - 1; i >= 0; i--)
        {
            // �ڽ� ��ü�� ������
            Transform child = map.transform.GetChild(i);
            // �ڽ� ��ü�� �ı�
            Destroy(child.gameObject);
        }
        mapList.Clear();
        StageSet(stageInfoList[0]);
        currentMapNum = 0;
        CreateMap();
        HideMap();
        telManager.StartPlayerTel();
    }
    //�������� �������°� ������� ������ ����
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
