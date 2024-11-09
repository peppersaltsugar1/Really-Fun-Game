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
        uIManager = UIManager.Instance;
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

        uIManager.UpdateHUD();
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
                uIManager.UpdateHUD();
            }
        }
    }

    // 내림차순 정렬을 위한 클래스
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
        if (ItemManager.Instance.itemList.ContainsKey("잠금파일 해독 키"))
        {
            KeyCount = ItemManager.Instance.itemList["잠금파일 해독 키"].Count;
        }
        return KeyCount;
    }
    public int GetCoinCount()
    {
        int CoinCount = 0;
        if (ItemManager.Instance.itemList.ContainsKey("(1) 메가 바이트 코인"))
        {
            CoinCount += ItemManager.Instance.itemList["(1) 메가 바이트 코인"].Count;
        }
        if (ItemManager.Instance.itemList.ContainsKey("(5) 메가 바이트 코인"))
        {
            CoinCount += 5 * ItemManager.Instance.itemList["(5) 메가 바이트 코인"].Count;
        }
        if (ItemManager.Instance.itemList.ContainsKey("(10) 메가 바이트 코인"))
        {
            CoinCount += 10 * ItemManager.Instance.itemList["(10) 메가 바이트 코인"].Count;
        }
        if (ItemManager.Instance.itemList.ContainsKey("(15) 메가 바이트 코인"))
        {
            CoinCount += 15 * ItemManager.Instance.itemList["(15) 메가 바이트 코인"].Count;
        }
        return CoinCount;
    }

    public int GetBombCount()
    {
        int BombCount = 0;
        if (ItemManager.Instance.itemList.ContainsKey("강제 삭제 프로세스"))
        {
            BombCount = ItemManager.Instance.itemList["강제 삭제 프로세스"].Count;
        }
        return BombCount;
    }
}
