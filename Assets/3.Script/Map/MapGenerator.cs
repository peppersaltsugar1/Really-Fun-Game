using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    private Map[] mapPrefabList;
    private List<Map> mapList = new List<Map>();
    public int maxMapNum; //x�� �ִ밹��
    private Transform map;
    private int createMapNum;
    private int currentMapNum;
    private List<Portal> portalList = new List<Portal>();
    private List<Portal> connectPortalList = new List<Portal>();
    private List<List<Portal>> temPortalList = new List<List<Portal>>();

    // Start is called before the first frame update
    void Start()
    {
        currentMapNum = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateMap()
    {
        CreateFirstMap();
        CreateMiddleMap();
    }
    private void CreateFirstMap()
    {
        while (true)
        {
            int mapIndex = Random.RandomRange(0, maxMapNum);
            if (mapPrefabList[mapIndex].Type == Map.MapType.Start)
            {
                Map firstMap = Instantiate(mapPrefabList[mapIndex]);
                // ������ Map ������Ʈ�� map�� �ڽ����� �߰�
                firstMap.transform.SetParent(map, false);

                // ������ Map ������Ʈ�� map�� �ڽ� ��Ͽ��� ���������� ��ġ��Ű��
                firstMap.transform.SetAsLastSibling();
                mapList.Add(firstMap);
                TakePortal(firstMap, portalList);
                MapCheck(portalList);
                break;
            }
        }
    }
    private void CreateMiddleMap()
    {
        if (currentMapNum < 3)
        {
            for (int i = 0; i < portalList.Count; i++)
            {
                int mapIndex = Random.RandomRange(0, mapList.Count);
                if (mapPrefabList[mapIndex].Type == Map.MapType.Middle)
                {
                    if (mapPrefabList[mapIndex].PortalNum - 1 < createMapNum - currentMapNum)
                    {
                        return;
                    }
                    Map middleMap = Instantiate(mapPrefabList[mapIndex]);
                    middleMap.transform.SetParent(map, false);
                    mapList.Add(middleMap);
                    TakePortal(middleMap, connectPortalList);
                }
                ConnectPortal();
            }
            TakePortal(mapList[mapList.Count - 1], portalList);
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
        portalList[0].connectPortal = connectPortalList[0];
        portalList.RemoveAt(0);
        connectPortalList.RemoveAt(0);
        MapCheck(connectPortalList);
    }

    private void MapCheck(List<Portal> portalList)
    {
        currentMapNum += portalList.Count;
    }

    private void CreateSpecialMap()
    {
        while (true)
        {
            int mapIndex = Random.RandomRange(0, maxMapNum);
            if (mapPrefabList[mapIndex].Type == Map.MapType.Shop)
            {
                Map firstMap = Instantiate(mapPrefabList[mapIndex]);
                // ������ Map ������Ʈ�� map�� �ڽ����� �߰�
                firstMap.transform.SetParent(map, false);

                // ������ Map ������Ʈ�� map�� �ڽ� ��Ͽ��� ���������� ��ġ��Ű��
                firstMap.transform.SetAsLastSibling();
                mapList.Add(firstMap);
                TakePortal(firstMap, portalList);
                MapCheck(portalList);
                break;
            }
        }
    }
}
