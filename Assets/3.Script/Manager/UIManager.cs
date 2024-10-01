using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager instance = null;
    [SerializeField]
    private Player player;
    [SerializeField]
    private List<GameObject> hpPrefabsList;
    [SerializeField]
    private List<GameObject> uiPortalList;
    [SerializeField]
    private List<GameObject> coinList;
    private List<GameObject> hpList = new();
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private Canvas localDiskContent;
    [SerializeField]
    private int interval;
    [SerializeField]
    MapGenerator mapGenerator;

    private int hpNum = 0;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public static UIManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    void Start()
    {
        HpBarSet();
    }

    void Update()
    {

    }
    public void HpBarSet()
    {
        Debug.Log("UI생성함");
        //hp 체력바 리셋
        if (hpList.Count > 0)
        {
            for (int i = hpList.Count - 1; i >= 0; i++)
            {
                GameObject removeHp = hpList[i];
                hpList.RemoveAt(i);
                Destroy(removeHp);
            }
            hpNum = 0;
        }
        //플레이어의 체력 상황에따라 체력바 재생성
        if (player.maxHp > 0)
        {
            //최대체력 3당 체력베터리 1개 생성후 리스트에 추가
            for (int i = 0; i < player.maxHp / 3; i++)
            {
                GameObject newHp = Instantiate(hpPrefabsList[0], canvas.transform);
                newHp.transform.SetParent(canvas.transform, false);
                RectTransform rectTransform = newHp.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(i * interval, 0); // 위치 조정 (임의로 설정)
                hpList.Add(newHp);
                hpNum += 1;
            }
            //임시체력 3당 임시체력베터리 1개 생성후 리스트에 추가
            if (player.temHp > 0)
            {
                for (int i = 0; i < player.temHp / 3; i++)
                {
                    GameObject newTemHp = Instantiate(hpPrefabsList[1], canvas.transform);
                    newTemHp.transform.SetParent(canvas.transform, false);
                    RectTransform rectTransform = newTemHp.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = new Vector2(hpNum * interval, 0); // 위치 조정 (임의로 설정)
                    hpList.Add(newTemHp);
                    hpNum += 1;

                }
            }
            //전기1 전기베터리 1개 생성후 리스트에 추가

            if (player.elect > 0)
            {
                for (int i = 0; i < player.elect; i++)
                {
                    GameObject spark = Instantiate(hpPrefabsList[2], canvas.transform);
                    spark.transform.SetParent(canvas.transform, false);
                    RectTransform rectTransform = spark.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = new Vector2(hpNum * interval, 0); // 위치 조정 (임의로 설정)
                    hpList.Add(spark);
                    hpNum += 1;

                }
            }
            //쉴드체력1당 체력베터리 1개 생성후 리스트에 추가
            if (player.shieldHp > 0)
            {
                for (int i = 0; i < player.shieldHp; i++)
                {
                    GameObject newShildHp = Instantiate(hpPrefabsList[3], canvas.transform);
                    newShildHp.transform.SetParent(canvas.transform, false);
                    RectTransform rectTransform = newShildHp.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = new Vector2(hpNum * interval, 0); // 위치 조정 (임의로 설정)
                    hpList.Add(newShildHp);
                    hpNum += 1;

                }
            }
        }
    }
    
    public void HpBarPlus()
    {

    }
    public void ShiledSet()
    {
        //쉴드체력이 소모될때 쉴드체력을 삭제
        for (int i = hpNum - 1; i >= 0; i--)
        {
            Debug.Log("여기들어옴");
            if (hpList[i].name == "Shield_Heart(Clone)")
            {
                GameObject removeHp = hpList[i];
                hpList.RemoveAt(i);
                Destroy(removeHp);
                hpNum--;
                ShiledOn();
                return;
            }
        }
    }
    private void ShiledOn()
    {
        //Hp체력바의 쉴드를 활성화
        for (int i = 0; i < hpNum; i++)
        {
            if (hpList[i].name == "R_Heart(Clone)")
            {
                // 첫 번째 자식을 가져와 활성화
                if (hpList[i].transform.childCount > 0)
                {
                    hpList[i].transform.GetChild(0).gameObject.SetActive(true);
                }
            }
        }
    }

    public void ShiledOff()
    {
        //hp체력바의 쉴드를 비활성화
        for (int i = hpNum - 1; i >= player.shield; i--)
        {
            if (hpList[i].name == "R_Heart(Clone)" && hpList[i].transform.GetChild(0).gameObject.activeSelf)
            {
                hpList[i].transform.GetChild(0).gameObject.SetActive(false);
                return;
            }
        }
    }
    public void HpSet()
    {
        if (player.currentHp <= 3)
        {
            switch (player.currentHp)
            {
                case 1:
                    hpList[0].transform.GetChild(1).gameObject.SetActive(true);
                    hpList[0].transform.GetChild(2).gameObject.SetActive(false);
                    hpList[0].transform.GetChild(3).gameObject.SetActive(false);
                    return;
                case 2:
                    hpList[0].transform.GetChild(1).gameObject.SetActive(true);
                    hpList[0].transform.GetChild(2).gameObject.SetActive(true);
                    hpList[0].transform.GetChild(3).gameObject.SetActive(false);
                    return;
                case 3:
                    hpList[0].transform.GetChild(1).gameObject.SetActive(true);
                    hpList[1].transform.GetChild(1).gameObject.SetActive(false);
                    hpList[0].transform.GetChild(2).gameObject.SetActive(true);
                    hpList[0].transform.GetChild(3).gameObject.SetActive(true);
                    return;
            }
        }
        switch (player.currentHp % 3)
        {
            case 0:
                for (int i = 0; i < player.maxHp / 3; i++)
                {
                    hpList[i].transform.GetChild(1).gameObject.SetActive(false);
                    hpList[i].transform.GetChild(2).gameObject.SetActive(false);
                    hpList[i].transform.GetChild(3).gameObject.SetActive(false);
                }
                for (int i = 0; i < player.currentHp / 3; i++)
                {
                    hpList[i].transform.GetChild(1).gameObject.SetActive(true);
                    hpList[i].transform.GetChild(2).gameObject.SetActive(true);
                    hpList[i].transform.GetChild(3).gameObject.SetActive(true);
                }
                break;
            case 1:
                hpList[((int)player.currentHp / 3)].transform.GetChild(1).gameObject.SetActive(true);
                hpList[((int)player.currentHp / 3)].transform.GetChild(2).gameObject.SetActive(false);
                hpList[((int)player.currentHp / 3)].transform.GetChild(3).gameObject.SetActive(false);

                break;
            case 2:
                hpList[((int)player.currentHp / 3)].transform.GetChild(1).gameObject.SetActive(true);
                hpList[((int)player.currentHp / 3)].transform.GetChild(2).gameObject.SetActive(true);
                hpList[((int)player.currentHp / 3)].transform.GetChild(3).gameObject.SetActive(false);

                break;
        }
    }

    public void TemHpSet()
    {
        //임시체력 관리 
        for (int i = hpNum - 1; i >= 0; i--)
        {
            if (hpList[i].name == "RDis_Heart(Clone)")
            {
                switch (player.temHp % 3)
                {
                    case 0:
                        hpList[i].transform.GetChild(0).gameObject.SetActive(true);
                        hpList[i].transform.GetChild(1).gameObject.SetActive(true);
                        hpList[i].transform.GetChild(2).gameObject.SetActive(true);
                        break;

                    case 1:
                        hpList[i].transform.GetChild(0).gameObject.SetActive(true);
                        hpList[i].transform.GetChild(1).gameObject.SetActive(false);
                        hpList[i].transform.GetChild(2).gameObject.SetActive(false);
                        break;

                    case 2:
                        hpList[i].transform.GetChild(0).gameObject.SetActive(true);
                        hpList[i].transform.GetChild(1).gameObject.SetActive(true);
                        hpList[i].transform.GetChild(2).gameObject.SetActive(false);
                        break;
                }
                return;
            }
        }
    }
    public void TemHpDel()
    {
        for (int i = hpNum - 1; i >= 0; i--)
        {
            if (hpList[i].name == "RDis_Heart(Clone)")
            {
                GameObject removeHp = hpList[i];
                hpList.RemoveAt(i);
                Destroy(removeHp);
                hpNum--;
                return;
            }
        }
    }
    public void HpDel()
    {
        for (int i = hpNum - 1; i >= 0; i--)
        {
            if (hpList[i].name == "R_Heart(Clone)")
            {
                GameObject removeHp = hpList[i];
                hpList.RemoveAt(i);
                Destroy(removeHp);
                hpNum--;
                return;
            }
        }
    }

    public void ElectDel()
    {
        for (int i = hpNum - 1; i >= 0; i--)
        {
            if (hpList[i].name == "Elect_Heart(Clone)")
            {
                GameObject removeHp = hpList[i];
                hpList.RemoveAt(i);
                Destroy(removeHp);
                hpNum--;
                return;
            }
        }
    }
    
    public void RoomUISet()
    {
        int mapIndex = 0;
        for(int i =0; i < mapGenerator.mapList.Count; i++)
        {
            Map map = mapGenerator.mapList[i];

            // 현재 활성화된 맵인지 확인
            if (map.transform.gameObject.activeSelf)
            {
                mapIndex = i;
                continue ; // 활성화된 맵의 인덱스 반환
            }
        }

        GameObject currentMap = mapGenerator.mapList[mapIndex].transform.gameObject;
        if(mapIndex == 0)
        {
            foreach (Transform child in currentMap.transform)
            {
                if (child.GetComponent<Portal>() != null)
                {
                    GameObject localDiscUI = Instantiate(uiPortalList[0]);
                    localDiscUI.transform.SetParent(localDiskContent.transform);
                    localDiscUI.transform.SetAsLastSibling();

                }

            }
            foreach (Transform child in currentMap.transform)
            {
                if (child.GetComponent<item>() != null)
                {
                    // 자식 객체의 부모를 localDiskContent로 설정
                    child.SetParent(localDiskContent.transform);
                }

            }
        }
        else
        {
            // 첫 번째 자식을 제외한 나머지 자식 처리
            for (int i = 1; i < currentMap.transform.childCount; i++) // i = 1 로 시작하여 첫 번째 자식 제외
            {
                Transform child = currentMap.transform.GetChild(i);
                if (child.GetComponent<Portal>() != null)
                {
                    // 자식 객체의 부모를 localDiskContent로 설정
                    child.SetParent(localDiskContent.transform);
                }
            }
            foreach (Transform child in currentMap.transform)
            {
                if (child.GetComponent<item>() != null)
                {
                    // 자식 객체의 부모를 localDiskContent로 설정
                    child.SetParent(localDiskContent.transform);
                }

            }
        }
    }
}
