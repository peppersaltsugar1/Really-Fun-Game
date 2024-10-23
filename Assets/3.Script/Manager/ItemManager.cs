using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;
    StatusManager statusManager;
    public SortedDictionary<string, List<Item>> itemList;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            itemList = new SortedDictionary<string, List<Item>>(); 
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
    }

    // Update is called once per frame
    void Update()
    {
        
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

        statusManager.CurrentStorage += item.ItemSize;
        return true;
    }

    // �������� ������ ���� Ŭ����
    public class DescendingComparer<T> : IComparer<T> where T : IComparable<T>
    {
        public int Compare(T x, T y)
        {
            return y.CompareTo(x);
        }
    }
}
