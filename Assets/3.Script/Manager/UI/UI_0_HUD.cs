using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_0_HUD : MonoBehaviour
{
    private static UI_0_HUD instance = null;

    // UI Window
    public GameObject UI_W_HUD = null;

    // Detail
    // HP
    [SerializeField]
    private List<GameObject> hpPrefabsList;
    private List<GameObject> hpList = new();
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private int interval;
    
    private int hpNum = 0;

    // Left side text
    public Text KeyCount;
    public Text CoinCount;
    public Text BombCount;
    public Text MonsterCount;

    public GameObject HUDGroup;
    public GameObject MiniMap;

    // Manager
    private StatusManager statusManager = null;
    private FolderManager folderManager = null;
    private ItemManager itemManager = null;
    public static UI_0_HUD Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UI_0_HUD>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(UI_0_HUD).Name);
                    instance = singletonObject.AddComponent<UI_0_HUD>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        statusManager = StatusManager.Instance;
        folderManager = FolderManager.Instance;
        itemManager = ItemManager.Instance;
        HpBarSet();
        UpdateHpUI();
    }

    public void OpenUI()
    {
        if (UI_W_HUD != null)
        {
            UI_W_HUD.SetActive(true);
            HUDGroup.SetActive(true);
            MiniMap.SetActive(true);
            foreach (GameObject HPList in hpList)
            {
                HPList.SetActive(true);
            }
        }
    }

    public void CloseUI()
    {
        if (UI_W_HUD != null)
        {
            UI_W_HUD.SetActive(false);
            HUDGroup.SetActive(false);
            MiniMap.SetActive(false);
            foreach (GameObject HPList in hpList)
            {
                HPList.SetActive(false);
            }
        }
    }

    public void UpdateHUD()
    {
        if (folderManager == null || itemManager == null)
        {
            Debug.Log("UpdateHUD find null");
            return;
        }

        KeyCount.text = itemManager.GetKeyCount().ToString();
        CoinCount.text = itemManager.GetCoinCount().ToString();
        BombCount.text = itemManager.GetBombCount().ToString();
        MonsterCount.text = folderManager.CurrentFolder.GetMonsterCount().ToString();
    }

    // HP

    // HP UI�� �ʱ�ȭ���ִ� �Լ�
    public void HpBarSet()
    {
        foreach (var hp in hpList)
        {
            Destroy(hp);
        }
        hpList.Clear();
        hpNum = 0;

        if (hpPrefabsList.Count < 4)
        {
            Debug.LogError("hpPrefabsList�� �ʿ��� �������� �����մϴ�.");
            return;
        }

        // �÷��̾��� ü�� ��Ȳ������ ü�¹� �����
        // ����ȭ �ڵ�
        // ü�� ���� �����͸� �迭�� ���� - ���ο�� �߰��Ǹ� ���⿡ �߰��ؾ� ��.
        var hpData = new (int count, GameObject prefab)[]
        {
        ((int)statusManager.MaxHp / 3, hpPrefabsList[0]), // �Ϲ� ü��
        ((int)statusManager.TemHp / 3, hpPrefabsList[1]), // �ӽ� ü��
        ((int)statusManager.Elect, hpPrefabsList[2]),     // ����
        ((int)statusManager.ShieldHp, hpPrefabsList[3])   // ����
        };

        // ü�¹� ����
        foreach (var (count, prefab) in hpData)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject newHp = Instantiate(prefab, canvas.transform);
                RectTransform rectTransform = newHp.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(hpNum * interval, 0); // ��ġ ����
                hpList.Add(newHp);
                hpNum++;
            }
        }

        // ���� ������ �ڵ�
        /*
        if (statusManager.MaxHp > 0)
        {
            //�ִ�ü�� 3�� ü�º��͸� 1�� ������ ����Ʈ�� �߰�
            for (int i = 0; i < statusManager.MaxHp / 3; i++)
            {
                GameObject newHp = Instantiate(hpPrefabsList[0], canvas.transform);
                newHp.transform.SetParent(canvas.transform, false);
                RectTransform rectTransform = newHp.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(i * interval, 0); // ��ġ ���� (���Ƿ� ����)
                hpList.Add(newHp);
                hpNum += 1;
            }
            //�ӽ�ü�� 3�� �ӽ�ü�º��͸� 1�� ������ ����Ʈ�� �߰�
            if (statusManager.TemHp > 0)
            {
                for (int i = 0; i < statusManager.TemHp / 3; i++)
                {
                    GameObject newTemHp = Instantiate(hpPrefabsList[1], canvas.transform);
                    newTemHp.transform.SetParent(canvas.transform, false);
                    RectTransform rectTransform = newTemHp.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = new Vector2(hpNum * interval, 0); // ��ġ ���� (���Ƿ� ����)
                    hpList.Add(newTemHp);
                    hpNum += 1;

                }
            }
            //����1 ���⺣�͸� 1�� ������ ����Ʈ�� �߰�

            if (statusManager.Elect > 0)
            {
                for (int i = 0; i < statusManager.Elect; i++)
                {
                    GameObject spark = Instantiate(hpPrefabsList[2], canvas.transform);
                    spark.transform.SetParent(canvas.transform, false);
                    RectTransform rectTransform = spark.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = new Vector2(hpNum * interval, 0); // ��ġ ���� (���Ƿ� ����)
                    hpList.Add(spark);
                    hpNum += 1;

                }
            }
            //����ü��1�� ü�º��͸� 1�� ������ ����Ʈ�� �߰�
            if (statusManager.ShieldHp > 0)
            {
                for (int i = 0; i < statusManager.ShieldHp; i++)
                {
                    GameObject newShildHp = Instantiate(hpPrefabsList[3], canvas.transform);
                    newShildHp.transform.SetParent(canvas.transform, false);
                    RectTransform rectTransform = newShildHp.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = new Vector2(hpNum * interval, 0); // ��ġ ���� (���Ƿ� ����)
                    hpList.Add(newShildHp);
                    hpNum += 1;

                }
            }
        }
        */

    }

    // HP UI�� �ֽ�ȭ���ִ� �Լ�
    public void UpdateHpUI()
    {
        // ���� ü�� UI�� ���¸� ������Ʈ
        for (int i = 0; i < hpList.Count; i++)
        {
            Transform hpSegment = hpList[i].transform;
            hpSegment.GetChild(1).gameObject.SetActive(false); // ù ĭ
            hpSegment.GetChild(2).gameObject.SetActive(false); // �� ��° ĭ
            hpSegment.GetChild(3).gameObject.SetActive(false); // �� ��° ĭ
        }

        // ü���� 3�� ���� �� ���׸�Ʈ�� Ȱ��ȭ
        int remainingHp = (int)statusManager.CurrentHp;
        for (int i = 0; i < hpList.Count && remainingHp > 0; i++)
        {
            // �⺻ ü�¿� ��쿡�� ó��
            if (hpList[i].name != "R_Heart(Clone)")
                return;

            Transform hpSegment = hpList[i].transform;

            if (remainingHp >= 3)
            {
                hpSegment.GetChild(1).gameObject.SetActive(true);
                hpSegment.GetChild(2).gameObject.SetActive(true);
                hpSegment.GetChild(3).gameObject.SetActive(true);
                remainingHp -= 3;
            }
            else if (remainingHp == 2)
            {
                hpSegment.GetChild(1).gameObject.SetActive(true);
                hpSegment.GetChild(2).gameObject.SetActive(true);
                remainingHp -= 2;
            }
            else if (remainingHp == 1)
            {
                hpSegment.GetChild(1).gameObject.SetActive(true);
                remainingHp -= 1;
            }
        }
    }

    // ShiledHP�� ���� ���°� ��Ʈ ���°� �� �� HP�� Shiled�� �����ִ� �Լ�
    public void ShiledSet()
    {
        // ShiledHP�� ���� ������(����Ʈ�� ��)�� ����.
        int index = hpNum - 1;
        if (index >= 0 && hpList[index].name == "Shield_Heart(Clone)")
        {
            GameObject removeHp = hpList[index];
            hpList.RemoveAt(index);
            Destroy(removeHp);
            hpNum--;
            ShiledOn();
            return;
        }
    }

    // HP�� Shiled�� �����ִ� �Լ�
    private void ShiledOn()
    {
        int RandomShiledNum = UnityEngine.Random.Range(0, (int)statusManager.CurrentHp % 3);

        //Hpü�¹��� ���带 Ȱ��ȭ
        for (int i = hpNum - 1; i >= 0 && RandomShiledNum > 0; i--)
        {
            if (hpList[i].name == "R_Heart(Clone)")
            {
                // ù ��° �ڽ��� ������ Ȱ��ȭ
                if (hpList[i].transform.childCount > 0)
                {
                    hpList[i].transform.GetChild(0).gameObject.SetActive(true);
                    RandomShiledNum--;
                }
            }
        }
    }

    // ShiledHP�� �ִ� ���¿��� �ǰݽ� ShiledHP�� �������ִ� �Լ�
    public void ShiledOff()
    {
        // ���� ������ ShiledHP�� ��Ȱ��ȭ
        for (int i = hpNum - 1; i >= statusManager.Shield; i--)
        {
            if (hpList[i].name == "R_Heart(Clone)" && hpList[i].transform.GetChild(0).gameObject.activeSelf)
            {
                hpList[i].transform.GetChild(0).gameObject.SetActive(false);
                return;
            }
        }
    }

    public void TemHpSet()
    {
        //�ӽ�ü�� ���� 
        for (int i = hpNum - 1; i >= 0; i--)
        {
            if (hpList[i].name == "RDis_Heart(Clone)")
            {
                switch (statusManager.TemHp % 3)
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

}
