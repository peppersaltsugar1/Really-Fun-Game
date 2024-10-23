using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Item : MonoBehaviour
{
    public enum ItemType
    {
        Coin1, Coin5, Coin10, Coin15, Heal, TemHp, Shiled, Spark
    }
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
        itemManager= ItemManager.Instance;  
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
            //switch (itemType)
            //{
            //    case ItemType.Coin:
            //        CoinItem();
            //        break;
            //    case ItemType.Heal:
            //        HealItem();
            //        break;
            //    case ItemType.TemHp:
            //        TemHpItem();
            //        break;
            //    case ItemType.Shiled:
            //        ShiledItem();
            //        break;
            //    case ItemType.Spark:
            //        SparkItem();
            //        break;
            //}
            CoinItem();
            isPickedUp = true;
        }

    }

    private void HealItem()
    {
        statusManager.Heal(itemScore);
    }
    private void CoinItem()
    {
        Debug.Log("Item CoinItem");
        statusManager.CoinUp(itemScore);
        if (itemManager != null)
        {
            if (itemManager.AddItem(this))
                Destroy(gameObject);
            else
                Debug.Log("Do not add item");
        }
        else
        {
            Debug.Log("ItemManager is not find");
        }
    }

    private void TemHpItem()
    {
        statusManager.TemHpUp(itemScore);
    }
    private void ShiledItem()
    {
        statusManager.TemHpUp(itemScore);
    }
    private void SparkItem()
    {
        statusManager.ElectUp(itemScore);
    }
}
