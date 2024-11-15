using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;
    StatusManager statusManager;
    UIManager uIManager;
    public SortedDictionary<string, List<Item>> itemList;
    private UI_0_HUD ui_0_HUD;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            itemList = new SortedDictionary<string, List<Item>>(new CoinComparer());
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        statusManager = StatusManager.Instance;
        uIManager = UIManager.Instance;
        ui_0_HUD = UI_0_HUD.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ���� ������ ���� �����ϱ� ���� �񱳱�
    public class CoinComparer : IComparer<string>
    {
        private readonly Dictionary<string, int> customOrder = new Dictionary<string, int>
        {
            { "(1) �ް� ����Ʈ ����", 1 },
            { "(5) �ް� ����Ʈ ����", 2 },
            { "(10) �ް� ����Ʈ ����", 3 },
            { "(15) �ް� ����Ʈ ����", 4 }
        };

        public int Compare(string x, string y)
        {
            // Ŀ���� ���� ������ �ִ� ���, �ش� ������ ����
            if (customOrder.ContainsKey(x) && customOrder.ContainsKey(y))
            {
                return customOrder[x].CompareTo(customOrder[y]);
            }
            // ������ �ƴ� ��� ���� ������ ����
            return x.CompareTo(y);
        }
    }



    public bool AddItem(Item item)
    {
        if (statusManager.CurrentStorage + item.ItemSize > statusManager.B_MaxStorage)
        {
            Debug.Log("������������ ������ �Ա� �Ұ���");
            return false;
        }

        Debug.Log("ItemManager AddItem"); 
        if (itemList.ContainsKey(item.ItemName))
        {
            // ���� �̸��� �������� �̹� �����ϴ� ��� ����Ʈ�� �߰�
            itemList[item.ItemName].Add(item);
        }
        else
        {
            // ���ο� �̸��� �������̸� ����Ʈ ���� �� �߰�
            itemList[item.ItemName] = new List<Item> { item };
        }

        ui_0_HUD.UpdateHUD();
        statusManager.CurrentStorage += item.ItemSize;
        return true;
    }

    public void RemoveItem(Item item)
    {
        if (itemList.ContainsKey(item.ItemName))
        {
            statusManager.CurrentStorage -= item.ItemSize;
            itemList[item.ItemName].Remove(item);
            if (itemList[item.ItemName].Count == 0)
            {
                itemList.Remove(item.ItemName);
                ui_0_HUD.UpdateHUD();
            }
        }
    }

    // �������� ������ ���� Ŭ����
    public class DescendingComparer<T> : IComparer<T> where T : IComparable<T>
    {
        public int Compare(T x, T y)
        {
            return y.CompareTo(x);
        }
    }

    public int GetKeyCount()
    {
        int KeyCount = 0;
        if (ItemManager.Instance.itemList.ContainsKey("������� �ص� Ű"))
        {
            KeyCount = ItemManager.Instance.itemList["������� �ص� Ű"].Count;
        }
        return KeyCount;
    }
    public int GetCoinCount()
    {
        int CoinCount = 0;
        if (ItemManager.Instance.itemList.ContainsKey("(1) �ް� ����Ʈ ����"))
        {
            CoinCount += ItemManager.Instance.itemList["(1) �ް� ����Ʈ ����"].Count;
        }
        if (ItemManager.Instance.itemList.ContainsKey("(5) �ް� ����Ʈ ����"))
        {
            CoinCount += 5 * ItemManager.Instance.itemList["(5) �ް� ����Ʈ ����"].Count;
        }
        if (ItemManager.Instance.itemList.ContainsKey("(10) �ް� ����Ʈ ����"))
        {
            CoinCount += 10 * ItemManager.Instance.itemList["(10) �ް� ����Ʈ ����"].Count;
        }
        if (ItemManager.Instance.itemList.ContainsKey("(15) �ް� ����Ʈ ����"))
        {
            CoinCount += 15 * ItemManager.Instance.itemList["(15) �ް� ����Ʈ ����"].Count;
        }
        return CoinCount;
    }

    public int GetBombCount()
    {
        int BombCount = 0;
        if (ItemManager.Instance.itemList.ContainsKey("���� ���� ���μ���"))
        {
            BombCount = ItemManager.Instance.itemList["���� ���� ���μ���"].Count;
        }
        return BombCount;
    }

    public bool KeyUse()
    {
        string keyName = "������� �ص� Ű"; // ã���� �ϴ� Ű�� �̸�

        // Ű �������� itemList�� �ְ�, �ش� ����Ʈ�� �������� �ϳ� �̻� �ִ��� Ȯ��
        if (itemList.ContainsKey(keyName) && itemList[keyName].Count > 0)
        {
            Item keyItem = itemList[keyName][0]; // ù ��° ������ ��������
            statusManager.CurrentStorage -= keyItem.ItemSize; // ��� �� ����� �뷮 ���̱�
            itemList[keyName].Remove(keyItem); // ����� ������ ����

            // �ش� Ű�� �� �̻� ������ ����Ʈ���� Ű ��ü�� ����
            if (itemList[keyName].Count == 0)
            {
                itemList.Remove(keyName);
            }

            ui_0_HUD.UpdateHUD(); // HUD ������Ʈ
            Debug.Log("������� �ص� Ű ����");
            return true;  // Ű ��� ����
        }
        else
        {
            Debug.Log("����� �� �ִ� ������� �ص� Ű�� �����ϴ�.");
            return false;  // Ű�� ������ ����
        }
    }
}
