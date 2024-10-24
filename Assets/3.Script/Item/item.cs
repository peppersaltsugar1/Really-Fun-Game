using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Item : MonoBehaviour
{   /*  
        - 돈  = 바이트 코인 (byte Coin)(1,5,10,15)
        - 열쇠 = 압축 폴더,.
        - 지뢰 키트 = 제2 공격 수단
        - 프로그램 및 제거 제거 툴 = 소지 프로그램 삭제
        0번 프로그램제거키트 : (사용)프로그램 제거 기능, 
        1번 카드팩 : (사용) 히든방 입장, 
        2번  열쇠 : (소지)열쇠방 입장시 사용, 
        3번 프로그램 재활용 : (사용)해당 방의 프로그램을 다른 프로그램으로 바꿔버림, 
        4번 제거키트 : (소지)강제삭제시 사용
    */
    public enum ItemType
    {
        Coin1, Coin5, Coin10, Coin15, Key, CardPack, ProgramRemove, ProgramRecycle, Heal, TemHp, Shiled, Spark
    }

    // 스프라이트 내 인덱스를 저장하는 배열. 현재 ProgramRecycle 번호까지만 저장되어 있음
    public static int[] ImageNumber = {0, 1, 6, 11, 2, 1, 4, 3 };
    

    public int itemScore;
    private GameManager gameManager;
    private StatusManager statusManager;
    private ItemManager itemManager;
    public ItemType itemType;
    public string ItemName;
    public string ItemInfomation;
    public int ItemSize;
    public bool IsAvailable;
    private bool isPickedUp = false; // 아이템이 이미 처리되었는지 여부
    //아이템종
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        statusManager = StatusManager.Instance;
        itemManager = ItemManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPickedUp) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            switch (itemType)
            {
                case ItemType.Coin1:
                case ItemType.Coin5:
                case ItemType.Coin10:
                case ItemType.Coin15:
                    CoinItem();
                    break;
                case ItemType.Key:
                    KeyItem();
                    break;
                case ItemType.CardPack:
                    CardPackItem();
                    break;
                case ItemType.ProgramRemove:
                    ProgramRemoveItem();
                    break;
                // 아래 것들은 추가 예정
                /*
                case ItemType.Heal:
                    HealItem();
                    break;
                case ItemType.TemHp:
                    TemHpItem();
                    break;
                case ItemType.Shiled:
                    ShiledItem();
                    break;
                case ItemType.Spark:
                    SparkItem();
                    break;
                */
            }
        }
    }

    private void AddItem()
    {
        if (itemManager != null)
        {
            if (itemManager.AddItem(this))
            {
                Destroy(gameObject);
                isPickedUp = true;
            }
            else
                Debug.Log("Do not add item");
        }
        else
        {
            Debug.Log("ItemManager is not find");
        }
    }

    private void CoinItem()
    {
        Debug.Log("Item CoinItem");
        statusManager.CoinUp(itemScore);
        if (itemManager != null)
        {
            if (itemManager.AddItem(this))
            {
                Destroy(gameObject);
                isPickedUp = true;
            }
            else
                Debug.Log("Do not add item");
        }
        else
        {
            Debug.Log("ItemManager is not find");
        }
    }
    private void KeyItem()
    {
        AddItem();
        Debug.Log("키 기능 구현 안되어 있음");
    }

    private void CardPackItem()
    {
        AddItem();
        Debug.Log("카드팩 기능 구현 안되어 있음");
    }

    private void ProgramRemoveItem()
    {
        AddItem();
        Debug.Log("제거툴 기능 구현 안되어 있음");
    }


    // 아래 것들은 추가 예정
    /*
    private void HealItem()
    {

    }

    private void TemHpItem()
    {

    }

    private void ShiledItem()
    {

    }

    private void SparkItem()
    {

    }
    */



}
