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
    private int middlePercent;
    private int specialPercent;
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
        for (int i = 0; i < createmapnum; i++)
        {
            CreateMiddleMap();
        }
    }
    private void CreateFirstMap()
    {
        while (true)
        {
            int mapIndex = Random.RandomRange(0, firstMapPrefabList.Length);
            if (firstMapPrefabList[mapIndex].Type == Map.MapType.Start)
            {
                Map firstMap = Instantiate(firstMapPrefabList[mapIndex]);
                // ������ Map ������Ʈ�� map�� �ڽ����� �߰�
                firstMap.transform.SetParent(map, false);
                // ������ Map ������Ʈ�� map�� �ڽ� ��Ͽ��� ���������� ��ġ��Ű��
                firstMap.transform.SetAsLastSibling();
                mapList.Add(firstMap);
                TakePortal(firstMap, portalList);
                MapCheck(portalList);
                switch (portalList.Count)
                {
                    case 1:
                        while (true)
                        {
                            int middleMapIndex = Random.RandomRange(0, normalMapPrefabList.Length);
                            if (normalMapPrefabList[middleMapIndex].PortalNum == 4)
                            {
                                Map middleMap = Instantiate(normalMapPrefabList[middleMapIndex]);
                                middleMap.transform.SetParent(map, false);
                                mapList.Add(middleMap);
                                TakePortal(middleMap, connectPortalList);
                                ConnectPortal();
                            }
                            else
                            {
                                return;
                            }
                            break;
                        }
                        break;
                    case 2:
                        for (int i = 0; i < 2; i++)
                        {
                            int middleMapIndex = Random.RandomRange(0, normalMapPrefabList.Length);
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
                            int middleMapIndex = Random.RandomRange(0, normalMapPrefabList.Length);
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
    private void CreateMiddleMap()
    {
        for(int i = 0; i < mapList.Count; i++)
        {
            TakePortal(mapList[i], portalList);
            if (portalList.Count >= 1)
            {
                switch (PersentCheck())
                {
                    case 1:
                        if (AbleMapCheck(PersentCheck()))
                        {
                            CreateRandomSpecialMap();
                        }
                        else
                        {
                            CreateNomalMap();
                        }

                        break;
                    case 2:
                        if (AbleMapCheck(PersentCheck()))
                        {
                            CreateRandomSpecialMap();
                        }
                        else
                        {
                            CreateSpecialMap();
                        }
                        break;
                    case 3: CreateNomalMap();
                        break;
                }
            }

        }
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
        for (int i = 0; i < portalList.Count; i++)
        {
            if (portalList[i].connectPortal != null)
            {
                portalList.RemoveAt(i);
                i--;
            }
        }
        return;
    }
    private void ConnectPortal()
    {
        Debug.Log(portalList.Count+"��Ż����Ʈ ����");
        Debug.Log(connectPortalList.Count+"��������Ż����Ʈ ����");
        portalList[0].connectPortal = connectPortalList[0];
        connectPortalList[0].connectPortal = portalList[0];
        portalList.RemoveAt(0);
        connectPortalList.RemoveAt(0);
        MapCheck(connectPortalList);
    }

    private void MapCheck(List<Portal> portalList)
    {
        currentMapNum = mapList.Count;
        makeMapNum = currentMapNum;
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
            int mapIndex = Random.RandomRange(0, normalMapPrefabList.Length);
            if (normalMapPrefabList[mapIndex].PortalNum-1+currentMapNum <= maxMapNum)
            {
                Map makeMap = Instantiate(firstMapPrefabList[mapIndex]);
                // ������ Map ������Ʈ�� map�� �ڽ����� �߰�
                makeMap.transform.SetParent(map, false);
                // ������ Map ������Ʈ�� map�� �ڽ� ��Ͽ��� ���������� ��ġ��Ű��
                makeMap.transform.SetAsLastSibling();
                mapList.Add(makeMap);
                TakePortal(makeMap, portalList);
                MapCheck(portalList);
                break;
            }
           
        }

    }
    private void CreateSpecialMap()
    {
        while (true)
        {
            int mapIndex = Random.RandomRange(0, specialMapPrefabList.Length);
            bool isDuplicate = false;  // �ߺ� ���θ� Ȯ���� ����

            // ���� mapList�� ���� Ÿ���� ���� �ִ��� Ȯ��
            for (int i = 0; i < mapList.Count; i++)
            {
                if (specialMapPrefabList[mapIndex].Type == mapList[i].Type)
                {
                    isDuplicate = true; // ���� Ÿ���� ���� ������ ��� true
                    break; // �ߺ��� �߰ߵǸ� �� �̻� üũ���� �ʰ� �ݺ��� ����
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
                TakePortal(makeMap, portalList);
                MapCheck(portalList);
                break; // �ߺ����� �ʴ� ���� ���������Ƿ� while�� ����
            }

            // �ߺ��Ǹ� �ٽ� while �� �ݺ�
        }
    }
    private void CreateRandomSpecialMap()
    {
        while (true)
        {
            int mapIndex = Random.RandomRange(0, randomSpecialMapPrefabList.Length);
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
                TakePortal(makeMap, portalList);
                MapCheck(portalList);
                break; // �ߺ����� �ʴ� ���� ���������Ƿ� while�� ����
            }

            // �ߺ��Ǹ� �ٽ� while �� �ݺ�
        }
    }
    private bool AbleMapCheck(int i)
    {
        if (currentMapNum + specialMapNum + randomSpeicalMapNum <= createMapNum)
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
                    /*if (downloadMapNum > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }*/
                    return true;
            }
            return true;
        }
        return false;

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
