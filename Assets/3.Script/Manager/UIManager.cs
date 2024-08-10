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
    private List<GameObject> hpList = new();
    [SerializeField]
    private Canvas canvas;
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
        //hp ü�¹� ����
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
        //�÷��̾��� ü�� ��Ȳ������ ü�¹� �����
        if (player.maxHp > 0)
        {
            //�ִ�ü�� 3�� ü�º��͸� 1�� ������ ����Ʈ�� �߰�
            for (int i = 0; i < player.maxHp / 3; i++)
            {
                GameObject newHp = Instantiate(hpPrefabsList[0], canvas.transform);
                newHp.transform.SetParent(canvas.transform, false);
                RectTransform rectTransform = newHp.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(i * 115, 0); // ��ġ ���� (���Ƿ� ����)
                hpList.Add(newHp);
                hpNum += 1;
            }
            //�ӽ�ü�� 3�� �ӽ�ü�º��͸� 1�� ������ ����Ʈ�� �߰�
            if (player.temHp > 0)
            {
                for (int i = 0; i < player.temHp / 3; i++)
                {
                    GameObject newTemHp = Instantiate(hpPrefabsList[1], canvas.transform);
                    newTemHp.transform.SetParent(canvas.transform, false);
                    RectTransform rectTransform = newTemHp.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = new Vector2(hpNum * 115, 0); // ��ġ ���� (���Ƿ� ����)
                    hpList.Add(newTemHp);
                    hpNum += 1;

                }
            }
            //����1 ���⺣�͸� 1�� ������ ����Ʈ�� �߰�

            if (player.elect > 0)
            {
                for (int i = 0; i < player.elect; i++)
                {
                    GameObject spark = Instantiate(hpPrefabsList[2], canvas.transform);
                    spark.transform.SetParent(canvas.transform, false);
                    RectTransform rectTransform = spark.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = new Vector2(hpNum * 115, 0); // ��ġ ���� (���Ƿ� ����)
                    hpList.Add(spark);
                    hpNum += 1;

                }
            }
            //����ü��1�� ü�º��͸� 1�� ������ ����Ʈ�� �߰�
            if (player.shieldHp > 0)
            {
                for (int i = 0; i < player.shieldHp; i++)
                {
                    GameObject newShildHp = Instantiate(hpPrefabsList[3], canvas.transform);
                    newShildHp.transform.SetParent(canvas.transform, false);
                    RectTransform rectTransform = newShildHp.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = new Vector2(hpNum * 115, 0); // ��ġ ���� (���Ƿ� ����)
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
        //����ü���� �Ҹ�ɶ� ����ü���� ����
        for (int i = hpNum - 1; i >= 0; i--)
        {
            Debug.Log("�������");
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
        //Hpü�¹��� ���带 Ȱ��ȭ
        for (int i = 0; i < hpNum; i++)
        {
            if (hpList[i].name == "R_Heart(Clone)")
            {
                // ù ��° �ڽ��� ������ Ȱ��ȭ
                if (hpList[i].transform.childCount > 0)
                {
                    hpList[i].transform.GetChild(0).gameObject.SetActive(true);
                }
            }
        }
    }

    public void ShiledOff()
    {
        //hpü�¹��� ���带 ��Ȱ��ȭ
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
        //�ӽ�ü�� ���� 
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
    

}
