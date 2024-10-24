using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Item : MonoBehaviour
{   /*  
        - ��  = ����Ʈ ���� (byte Coin)(1,5,10,15)
        - ���� = ���� ����,.
        - ���� ŰƮ = ��2 ���� ����
        - ���α׷� �� ���� ���� �� = ���� ���α׷� ����
        0�� ���α׷�����ŰƮ : (���)���α׷� ���� ���, 
        1�� ī���� : (���) ����� ����, 
        2��  ���� : (����)����� ����� ���, 
        3�� ���α׷� ��Ȱ�� : (���)�ش� ���� ���α׷��� �ٸ� ���α׷����� �ٲ����, 
        4�� ����ŰƮ : (����)���������� ���
    */
    public enum ItemType
    {
        Coin1, Coin5, Coin10, Coin15, Key, CardPack, ProgramRemove, ProgramRecycle, Heal, TemHp, Shiled, Spark
    }

    // ��������Ʈ �� �ε����� �����ϴ� �迭. ���� ProgramRecycle ��ȣ������ ����Ǿ� ����
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
    private bool isPickedUp = false; // �������� �̹� ó���Ǿ����� ����
    //��������
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
                // �Ʒ� �͵��� �߰� ����
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
        Debug.Log("Ű ��� ���� �ȵǾ� ����");
    }

    private void CardPackItem()
    {
        AddItem();
        Debug.Log("ī���� ��� ���� �ȵǾ� ����");
    }

    private void ProgramRemoveItem()
    {
        AddItem();
        Debug.Log("������ ��� ���� �ȵǾ� ����");
    }


    // �Ʒ� �͵��� �߰� ����
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
