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

    // ������
    public float PushForce;
    public float DragForce; 

    Rigidbody2D rb = null;

    // ������ ��� ȿ��
    public float followSpeed = 10f;
    public float GetDistance = 1f; // ��� �Ÿ� (Destroy ����)
    public float maxDistance = 10f; // �ִ� ���� �Ÿ�
    private Transform playerTransform;
    private bool isTracking = false;
    public bool isDroped = false;

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
        if (isTracking && playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer <= GetDistance)
            {
                // ��� ���� �̳�: ������ ȹ��
                ItemTypeToFun();
            }
            else if (distanceToPlayer > maxDistance)
            {
                // �ִ� �Ÿ� �̻�: ���� �ߴ�
                isTracking = false;
            }
            else
            {
                // ����: �÷��̾� ����
                Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
                transform.position += directionToPlayer * followSpeed * Time.deltaTime;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPickedUp) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            // �뷮 ������ ���
            if (statusManager.MaxStorage - statusManager.CurrentStorage < ItemSize)
            {
                isTracking = false;
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
            else
            {
                Debug.Log("��� ȿ�� on");
                // ��� ȿ�� 
                playerTransform = collision.transform;
                if(isDroped == false)
                    isTracking = true;
            }
        }
    }

    private void ItemTypeToFun()
    {
        // ������ ���
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
        Debug.Log("Ű ��� ���� �ȵǾ� ����");
    }

    private void CardPackItem()
    {
        AddItem();
        Debug.Log("ī���� ��� ���� �ȵǾ� ����");
    }

    private void ForcedDeletionItem()
    {
        AddItem();
        Debug.Log("�������� ��� ���� �ȵǾ� ����");
    }

    private void ProgramRemoveItem()
    {
        AddItem();
        Debug.Log("������ ��� ���� �ȵǾ� ����");
    }

    private void ProgramRecycleItem()
    {
        AddItem();
        Debug.Log("���α׷� ��Ȱ�� ��� ���� �ȵǾ� ����");
    }

    private void HealItem()
    {
        Debug.Log("�� ������ ��� ���� �ȵǾ� ����");
    }

    private void TemHpItem()
    {
        Debug.Log("�ӽ�ü�� ������ ��� ���� �ȵǾ� ����");
    }

    private void ShiledItem()
    {
        Debug.Log("�ǵ� ������ ��� ���� �ȵǾ� ����");
    }

    private void SparkItem()
    {
        Debug.Log("���� ������ ��� ���� �ȵǾ� ����");
    }


    IEnumerator StopAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        rb.velocity = Vector2.zero;
    }


}
