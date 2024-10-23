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
            Debug.Log("공간부족으로 아이템 먹기 불가능");
            return false;
        }

        Debug.Log("ItemManager AddItem"); 
        if (itemList.ContainsKey(item.ItemName))
        {
            // 같은 이름의 아이템이 이미 존재하는 경우 리스트에 추가
            itemList[item.ItemName].Add(item);
        }
        else
        {
            // 새로운 이름의 아이템이면 리스트 생성 후 추가
            itemList[item.ItemName] = new List<Item> { item };
        }

        statusManager.CurrentStorage += item.ItemSize;
        return true;
    }

    // 내림차순 정렬을 위한 클래스
    public class DescendingComparer<T> : IComparer<T> where T : IComparable<T>
    {
        public int Compare(T x, T y)
        {
            return y.CompareTo(x);
        }
    }
}
