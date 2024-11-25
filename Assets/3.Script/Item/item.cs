using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Item : MonoBehaviour
{
    public enum ItemType
    {
        Coin1, Coin5, Coin10, Coin15, Key, CardPack, ForcedDeletion, ProgramRemove, ProgramRecycle, Heal, TemHp, Shiled, Spark
    }

    public int itemScore;
    private GameManager gameManager;
    private StatusManager statusManager;
    private ItemManager itemManager;
    public ItemType itemType;
    public string ItemName;
    public string ItemInfomation;
    public int ItemSize;
    public bool IsUsable = true;
    public bool IsDeletable = true;
    private bool isPickedUp = false;

    // 물리력
    public float PushForce;
    public float DragForce; 

    Rigidbody2D rb = null;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        statusManager = StatusManager.Instance;
        itemManager = ItemManager.Instance;
        rb = GetComponent<Rigidbody2D>();
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
            if(statusManager.MaxStorage - statusManager.CurrentStorage < ItemSize )
            {
                if (rb != null)
                {
                    Vector2 pushDirection = (transform.position - collision.transform.position).normalized;

                    // Add a small force to move the item
                    float pushForce = PushForce;
                    rb.drag = DragForce;
                    rb.AddForce(pushDirection * pushForce);
                    StartCoroutine(StopAfterDelay(0.5f));
                }
                else
                {
                    Debug.LogError("rb is null");
                }    
            }
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
                case ItemType.ForcedDeletion:
                    ForcedDeletionItem();
                    break;
                case ItemType.ProgramRemove:
                    ProgramRemoveItem();
                    break;
                case ItemType.ProgramRecycle:
                    ProgramRecycleItem();
                    break;
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

    // Item Usage Effect Section
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

    private void ForcedDeletionItem()
    {
        AddItem();
        Debug.Log("강제삭제 기능 구현 안되어 있음");
    }

    private void ProgramRemoveItem()
    {
        AddItem();
        Debug.Log("제거툴 기능 구현 안되어 있음");
    }

    private void ProgramRecycleItem()
    {
        AddItem();
        Debug.Log("프로그램 재활용 기능 구현 안되어 있음");
    }

    private void HealItem()
    {
        Debug.Log("힐 아이템 기능 구현 안되어 있음");
    }

    private void TemHpItem()
    {
        Debug.Log("임시체력 아이템 기능 구현 안되어 있음");
    }

    private void ShiledItem()
    {
        Debug.Log("실드 아이템 기능 구현 안되어 있음");
    }

    private void SparkItem()
    {
        Debug.Log("전기 아이템 기능 구현 안되어 있음");
    }


    IEnumerator StopAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        rb.velocity = Vector2.zero;
    }


}
