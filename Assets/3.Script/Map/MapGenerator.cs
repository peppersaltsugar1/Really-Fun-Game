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
    private List<Map> mapList = new List<Map>();
    //�� �������� �μ�
    public int maxMapNum; //���ִ밹��
    private Transform map;
    [SerializeField]
    private int createMapNum; //������ ���� ����
    private int currentMapNum; //������ ���ǰ���
    private int makeMapNum; //������ �� ���ǰ���(��Ż�̸������ ����)
    [SerializeField]
    private int specialMapNum; //������ ����ȸ��� ����
    [SerializeField]
    private int randomSpeicalMapNum; //������ ��������ȸ��� ����
    //�� ���� Ȯ���μ�
    [SerializeField]
    private int specialPercent;
    [SerializeField]
    private int randomSpecialPercent;
    //���̰���   
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
                // ������ Map ������Ʈ�� map�� �ڽ����� �߰�
                firstMap.transform.SetParent(map, false);
                // ������ Map ������Ʈ�� map�� �ڽ� ��Ͽ��� ���������� ��ġ��Ű��
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
        // ��Ż���� ��Ż ����Ʈ�� �߰�
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
        Debug.Log("����ȸʻ���������");
        while (true)
        {
            int mapIndex = Random.Range(0, specialMapPrefabList.Length);  
            bool isDuplicate = false;  // �ߺ� ���θ� Ȯ���� ����

            // ���� mapList�� ���� Ÿ���� ���� �ִ��� Ȯ��
            for (int i = 0; i < mapList.Count; i++)
            {
                Debug.Log(isDuplicate);
                Debug.Log(specialMapPrefabList[mapIndex].Type);
                Debug.Log(mapList[i].Type);
                if (specialMapPrefabList[mapIndex].Type == mapList[i].Type)
                {
                    Debug.Log("��Ÿ�԰��� üũ��");
                    isDuplicate = true; // ���� Ÿ���� ���� ������ ��� true
                    i=mapList.Count; // �ߺ��� �߰ߵǸ� �� �̻� üũ���� �ʰ� �ݺ��� ����
                }
            }
            // �ߺ����� �ʴ� ��쿡�� ���� ����
            if (!isDuplicate)
            {
                Debug.Log("����ȸʸ��鷯����");
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
                if (randomSpecialMapPrefabList[mapIndex].name == mapList[i].name)
                {
                    isDuplicate = true; // ���� Ÿ���� ���� ������ ��� true
                    break; // �ߺ��� �߰ߵǸ� �� �̻� üũ���� �ʰ� �ݺ��� ����
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
