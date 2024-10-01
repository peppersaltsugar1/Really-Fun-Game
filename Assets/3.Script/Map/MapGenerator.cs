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
    public List<Map> mapList = new List<Map>();
    //�� �������� �μ�
    public int maxMapNum; //���ִ밹��
    [SerializeField]
    public Transform map;
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
        for(int i = 0; i < createmapnum; i++)
        {
            InfiniteLoopDetector.Run();
            CreateNextMap();
        }


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
        mapList[0].gameObject.SetActive(true);
    }
}
