using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;
using static Item;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    public SortedDictionary<string, List<Item>> itemList;

    private StatusManager statusManager;
    private UIManager uIManager;
    private UI_0_HUD ui_0_HUD;
    private Player player;
    Rigidbody2D rb;

    // ������ ������
    public GameObject P_Coin1;
    public GameObject P_Coin5;
    public GameObject P_Coin10;
    public GameObject P_Coin15;
    public GameObject P_Key;
    public GameObject P_CardPack;
    public GameObject P_ForcedDeletion;
    public GameObject P_ProgramRemove;
    public GameObject P_ProgramRecycle;
    public GameObject P_Heal;
    public GameObject P_TemHp;
    public GameObject P_Shiled;
    public GameObject P_Spark;

    GameObject SpawnObject = null;
    GameObject droppedItem = null;

    // ItemType�� Ű��, �̹��� �ε����� ������ �����ϴ� Dictionary
    public static Dictionary<ItemType, int> ImageIndexMap = new Dictionary<ItemType, int>()
    {
        { ItemType.Coin1, 0 },
        { ItemType.Coin5, 1 },
        { ItemType.Coin10, 6 },
        { ItemType.Coin15, 11 },
        { ItemType.Key, 2 },
        { ItemType.CardPack, 1 },
        { ItemType.ForcedDeletion, 4 },
        { ItemType.ProgramRemove, 0 },
        { ItemType.ProgramRecycle, 3 },
        { ItemType.Heal, 7 }

    };

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
        player = GameManager.Instance.GetPlayer();
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
        if (statusManager.CurrentStorage + item.ItemSize > statusManager.MaxStorage)
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

    public void DropItem(Item item, bool isRight)
    {
        RemoveItem(item);

        DesignateGameObject(item.itemType);

        Vector3 offset = player.transform.position + (isRight ? new Vector3(0.9f, 0, 0) : new Vector3(-0.8f, 0, 0));
        GameObject droppedItem = Instantiate(SpawnObject, offset, Quaternion.identity);

        Vector3 pushDirection = (droppedItem.transform.position - player.transform.position).normalized;
        rb = droppedItem.GetComponent<Rigidbody2D>();
        rb.drag = item.DragForce;
        rb.AddForce(pushDirection * item.PushForce, ForceMode2D.Impulse);
        StartCoroutine(StopAfterDelay(0.3f));
    }

    public void DropDownItem(Item item)
    {
        RemoveItem(item);

        DesignateGameObject(item.itemType);

        Vector3 offset = player.transform.position + new Vector3(0, -1.2f, 0); 
        droppedItem = Instantiate(SpawnObject, offset, Quaternion.identity);

        Vector3 pushDirection = (droppedItem.transform.position - player.transform.position).normalized;
        rb = droppedItem.GetComponent<Rigidbody2D>();
        rb.AddForce(pushDirection * 1.5f, ForceMode2D.Impulse);
        
        StartCoroutine(StopAfterDelay(0.3f));
    }

    private void DesignateGameObject(Item.ItemType itemType)
    {
        switch (itemType)
        {
            case Item.ItemType.Coin1: SpawnObject = P_Coin1; break;
            case Item.ItemType.Coin5: SpawnObject = P_Coin5; break;
            case Item.ItemType.Coin10: SpawnObject = P_Coin10; break;
            case Item.ItemType.Key: SpawnObject = P_Key; break;
            case Item.ItemType.CardPack: SpawnObject = P_CardPack; break;
            case Item.ItemType.ForcedDeletion: SpawnObject = P_ForcedDeletion; break;
            case Item.ItemType.ProgramRemove: SpawnObject = P_ProgramRemove; break;
            case Item.ItemType.ProgramRecycle: SpawnObject = P_ProgramRecycle; break;
            case Item.ItemType.Heal: SpawnObject = P_Heal; break;
            case Item.ItemType.TemHp: SpawnObject = P_TemHp; break;
            case Item.ItemType.Shiled: SpawnObject = P_Shiled; break;
            case Item.ItemType.Spark: SpawnObject = P_Spark; break;
        }
    }

    IEnumerator StopAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        rb.velocity = Vector2.zero;
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
        if (itemList.ContainsKey("�������� ��������"))
        {
            BombCount = itemList["�������� ��������"].Count;
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

    public int GetImageIndex(ItemType itemType)
    {
        if (ImageIndexMap.TryGetValue(itemType, out int index))
        {
            return index;
        }

        Debug.LogWarning($"Image index for {itemType} not found.");
        return -1; // �⺻�� ��ȯ
    }

    public string GetSpriteSheetName(ItemType itemType)
    {
        string spriteSheetName = "";
        switch (itemType)
        {
            case Item.ItemType.Coin1:
            case Item.ItemType.Coin5:
            case Item.ItemType.Coin10:
            case Item.ItemType.Coin15:
                spriteSheetName = "Item/use_Coin";
                break;
            case Item.ItemType.Key:
            case Item.ItemType.CardPack:
            case Item.ItemType.ForcedDeletion:
            case Item.ItemType.ProgramRemove:
            case Item.ItemType.ProgramRecycle:
                spriteSheetName = "Item/use_DropItem";
                break;
            case Item.ItemType.Heal:
            case Item.ItemType.TemHp:
            case Item.ItemType.Shiled:
            case Item.ItemType.Spark:
                spriteSheetName = "Sprites/Items/Heal";
                break;

            default:
                Debug.LogError("Unknown item type.");
                break;
        }
        return spriteSheetName;
    }

}
