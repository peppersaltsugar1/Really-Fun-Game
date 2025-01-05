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
    public List<GameObject> hpList = new();
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
        int HPTotal = ((int)statusManager.MaxHp / 3) + ((int)statusManager.MaxHp % 3 > 0 ? 1 : 0);
        int TempTotal = ((int)statusManager.TemHp / 3) + ((int)statusManager.TemHp % 3 > 0 ? 1 : 0);
        var hpData = new (int count, GameObject prefab)[]
        {
        ( HPTotal, hpPrefabsList[0]), // �Ϲ� ü��
        ( TempTotal, hpPrefabsList[1]), // �ӽ� ü��
        ( (int)statusManager.Elect, hpPrefabsList[2]),     // ����
        ( (int)statusManager.Shield, hpPrefabsList[3])   // ����
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

        // UpdateHpUI();
    }

    // HP UI�� �ֽ�ȭ���ִ� �Լ�
    public void UpdateHpUI()
    {
        // ���� ü�� UI�� ���¸� ������Ʈ
        for (int i = 0; i < hpList.Count; i++)
        {
            if (hpList[i].name == "R_Heart(Clone)")
            {
                Transform hpSegment = hpList[i].transform;
                hpSegment.GetChild(1).gameObject.SetActive(false); // ù ĭ
                hpSegment.GetChild(2).gameObject.SetActive(false); // �� ��° ĭ
                hpSegment.GetChild(3).gameObject.SetActive(false); // �� ��° ĭ
            }
            if (hpList[i].name == "RDis_Heart(Clone)")
            {
                Transform hpSegment = hpList[i].transform;
                hpSegment.GetChild(0).gameObject.SetActive(false); // ù ĭ
                hpSegment.GetChild(1).gameObject.SetActive(false); // �� ��° ĭ
                hpSegment.GetChild(2).gameObject.SetActive(false); // �� ��° ĭ
            }
        }

        // ü���� 3�� ���� �� ���׸�Ʈ�� Ȱ��ȭ
        int remainingHp = (int)statusManager.CurrentHp;
        for (int i = 0; i < hpList.Count && remainingHp > 0; i++)
        {
            // �⺻ ü�¿� ��쿡�� ó��
            if (hpList[i].name != "R_Heart(Clone)")
                continue;

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

        // �ӽ� ü���� 3�� ���� �� ���׸�Ʈ�� Ȱ��ȭ
        int remainingTemp = (int)statusManager.TemHp;
        for (int i = 0; i < hpList.Count && remainingTemp > 0; i++)
        {
            // �ӽ�ü���� ��쿡�� ó��
            if (hpList[i].name != "RDis_Heart(Clone)")
                continue;

            Transform hpSegment = hpList[i].transform;

            if (remainingTemp >= 3)
            {
                hpSegment.GetChild(0).gameObject.SetActive(true);
                hpSegment.GetChild(1).gameObject.SetActive(true);
                hpSegment.GetChild(2).gameObject.SetActive(true);
                remainingTemp -= 3;
            }
            else if (remainingTemp == 2)
            {
                hpSegment.GetChild(0).gameObject.SetActive(true);
                hpSegment.GetChild(1).gameObject.SetActive(true);
                remainingTemp -= 2;
            }
            else if (remainingTemp == 1)
            {
                hpSegment.GetChild(0).gameObject.SetActive(true);
                remainingTemp -= 1;
            }
        }

        // ���� ����
        ShiledOn();
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
            SetRandomShiledNum();
            ShiledOn();
            return;
        }
    }

    // ���尡 ������ ������ �������ִ� �Լ�
    private void SetRandomShiledNum()
    {
        int RandomShiledNum = UnityEngine.Random.Range(1, (int)statusManager.CurrentHp / 3 + 1);

        Debug.Log($"���͸� ���� : {RandomShiledNum}");
        statusManager.ShieldHp = RandomShiledNum;
    }

    // HP�� Shiled�� �����ִ� �Լ�
    private void ShiledOn()
    {
        int remainingShields = (int)statusManager.ShieldHp; // Ȱ��ȭ�� ���� ����
        int currentHp = (int)statusManager.CurrentHp;       // ���� HP

        int ActiveShield = currentHp / 3 - 1;
        // ActiveShield = ActiveShield >= 
        // �����ʺ��� ���� ����
        for (int i = ActiveShield; i >= 0 && remainingShields > 0; i--)
        {
            if (hpList[i].name == "R_Heart(Clone)")
            {
                Transform segments = hpList[i].transform;
                if (hpList[i].transform.childCount > 0)
                {
                    hpList[i].transform.GetChild(0).gameObject.SetActive(true);
                    remainingShields--;
                }
            }
        }
    }

    // ShiledHP�� �ִ� ���¿��� �ǰݽ� ShiledHP�� �������ִ� �Լ�
    public void ShiledOff()
    {
        // ���� ������ ShiledHP�� ��Ȱ��ȭ
        for (int i = hpNum - 1; i >= 0; i--)
        {
            if (hpList[i].name == "R_Heart(Clone)" && hpList[i].transform.GetChild(0).gameObject.activeSelf)
            {
                hpList[i].transform.GetChild(0).gameObject.SetActive(false);
                return;
            }
        }
    }


    // ShiledHP�� �Ϲ�(����)HP�� ó���ϴ� �Լ�
    public void DamagedHP()
    {
        for (int i = hpList.Count - 1; i >= 0; i--)
        {
            if (hpList[i].name == "R_Heart(Clone)")
            {
                Transform segments = hpList[i].transform;

                if (segments.GetChild(0).gameObject.activeSelf)
                {
                    Debug.Log("�Ϲ� ü�� �Ҹ�");
                    segments.GetChild(0).gameObject.SetActive(false); // �Ϲ� ü�� ����
                    statusManager.CurrentHp--;
                }
            }
        }
    }
}
