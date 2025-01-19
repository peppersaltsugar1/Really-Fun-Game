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

    [Header("Item Prefab")]
    public GameObject P_Coin1;
    public GameObject P_Coin5;
    public GameObject P_Coin10;
    public GameObject P_Coin15;
    public GameObject P_Coin100;
    public GameObject P_Key;
    public GameObject P_CardPack;
    public GameObject P_ForcedDeletion;
    public GameObject P_ProgramRemove;
    public GameObject P_ExpansionKit_1;
    public GameObject P_ExpansionKit_2;
    public GameObject P_ExpansionKit_3;
    public GameObject P_ProgramRecycle;
    public GameObject P_Card_Clover;
    public GameObject P_Card_Spade;
    public GameObject P_Card_Hearth;
    public GameObject P_Card_Dia;
    public GameObject P_Ticket_Random;
    public GameObject P_Ticket_Down;
    public GameObject P_Ticket_Shop;
    public GameObject P_Ticket_Special;
    public GameObject P_Ticket_BlackShop;
    public GameObject P_Ticket_Boss;

    public GameObject P_Heal;
    public GameObject P_TemHp;
    public GameObject P_Shiled;
    public GameObject P_Spark;


    GameObject SpawnObject = null;
    GameObject droppedItem = null;

    // ������ �̸�
    private string KeyName;
    private string ForcedDeletionName;
    private string Coin1_Name;
    private string Coin5_Name;
    private string Coin10_Name;
    private string Coin15_Name;
    private string Coin100_Name;

    // ItemType�� Ű��, �̹��� �ε����� ������ �����ϴ� Dictionary
    public static Dictionary<ItemType, int> ImageIndexMap = new Dictionary<ItemType, int>()
    {
        { ItemType.Coin1, 0 },
        { ItemType.Coin5, 1 },
        { ItemType.Coin10, 6 },
        { ItemType.Coin15, 11 },
        { ItemType.Coin100, 16 },
        { ItemType.Key, 2 },
        { ItemType.CardPack, 1 },
        { ItemType.ForcedDeletion, 4 },
        { ItemType.ProgramRemove, 0 },
        { ItemType.ExpansionKit_1, 17 },
        { ItemType.ExpansionKit_2, 18 },
        { ItemType.ExpansionKit_3, 19 },
        { ItemType.ProgramRecycle, 3 },
        { ItemType.Card_Spade, 5 },
        { ItemType.Card_Clover, 6 },
        { ItemType.Card_Hearth, 7 },
        { ItemType.Card_Dia, 8 },
        { ItemType.Ticket_Shop, 9 },
        { ItemType.Ticket_Down, 10 },
        { ItemType.Ticket_Special, 11 },
        { ItemType.Ticket_Random, 12 },
        { ItemType.Ticket_BlackShop, 13 },
        { ItemType.Ticket_Boss, 14 }


    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // �̸� ����
        KeyName = P_Key.GetComponent<Item>().ItemName;
        ForcedDeletionName = P_ForcedDeletion.GetComponent<Item>().ItemName;
        Coin1_Name = P_Coin1.GetComponent<Item>().ItemName;
        Coin5_Name = P_Coin5.GetComponent<Item>().ItemName;
        Coin10_Name = P_Coin10.GetComponent<Item>().ItemName;
        Coin15_Name = P_Coin15.GetComponent<Item>().ItemName;
        Coin100_Name = P_Coin100.GetComponent<Item>().ItemName;

        if (itemList == null)
        {
            itemList = new SortedDictionary<string, List<Item>>(new ItemManager.CoinComparer(this));
        }
    }

    void Start()
    {
        statusManager = StatusManager.Instance;
        uIManager = UIManager.Instance;
        ui_0_HUD = UI_0_HUD.Instance;
        player = GameManager.Instance.GetPlayer();

    }

    // ���� ������ ���� �����ϱ� ���� �񱳱�
    public class CoinComparer : IComparer<string>
    {
        private Dictionary<string, int> customOrder;

        public CoinComparer(ItemManager manager)
        {
            customOrder = new Dictionary<string, int>
            {
                { manager.Coin1_Name, 1 },
                { manager.Coin5_Name, 2 },
                { manager.Coin10_Name, 3 },
                { manager.Coin15_Name, 4 },
                { manager.Coin100_Name, 5 }
            };
        }

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

        // Debug.Log("ItemManager AddItem"); 
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
        droppedItem = Instantiate(SpawnObject, offset, Quaternion.identity);

        droppedItem.GetComponent<Item>().isDroped = true;

        Vector3 pushDirection = (droppedItem.transform.position - player.transform.position).normalized;
        rb = droppedItem.GetComponent<Rigidbody2D>();
        rb.drag = statusManager.DragForce;
        rb.AddForce(pushDirection * statusManager.DropForce, ForceMode2D.Impulse);
        StartCoroutine(StopAfterDelay(0.3f));
    }

    public void DropDownItem(Item item)
    {
        RemoveItem(item);

        DesignateGameObject(item.itemType);

        Vector3 offset = player.transform.position + new Vector3(0, -1.2f, 0); 
        droppedItem = Instantiate(SpawnObject, offset, Quaternion.identity);

        droppedItem.GetComponent<Item>().isDroped = true;

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
            case Item.ItemType.Coin15: SpawnObject = P_Coin15; break;
            case Item.ItemType.Coin100: SpawnObject = P_Coin100; break;
            case Item.ItemType.Key: SpawnObject = P_Key; break;
            case Item.ItemType.CardPack: SpawnObject = P_CardPack; break;
            case Item.ItemType.ForcedDeletion: SpawnObject = P_ForcedDeletion; break;
            case Item.ItemType.ProgramRemove: SpawnObject = P_ProgramRemove; break;
            case Item.ItemType.ExpansionKit_1: SpawnObject = P_ExpansionKit_1; break;
            case Item.ItemType.ExpansionKit_2: SpawnObject = P_ExpansionKit_2; break;
            case Item.ItemType.ExpansionKit_3: SpawnObject = P_ExpansionKit_3; break;
            case Item.ItemType.ProgramRecycle: SpawnObject = P_ProgramRecycle; break;
            case Item.ItemType.Card_Clover: SpawnObject = P_Card_Clover; break;
            case Item.ItemType.Card_Spade: SpawnObject = P_Card_Spade; break;
            case Item.ItemType.Card_Hearth: SpawnObject = P_Card_Hearth; break;
            case Item.ItemType.Card_Dia: SpawnObject = P_Card_Dia; break;
            case Item.ItemType.Ticket_Random: SpawnObject = P_Ticket_Random; break;
            case Item.ItemType.Ticket_Down: SpawnObject = P_Ticket_Down; break;
            case Item.ItemType.Ticket_Shop: SpawnObject = P_Ticket_Shop; break;
            case Item.ItemType.Ticket_Special: SpawnObject = P_Ticket_Special; break;
            case Item.ItemType.Ticket_BlackShop: SpawnObject = P_Ticket_BlackShop; break;
            case Item.ItemType.Ticket_Boss: SpawnObject = P_Ticket_Boss; break;
            case Item.ItemType.Heal: SpawnObject = P_Heal; break;
            case Item.ItemType.TemHp: SpawnObject = P_TemHp; break;
            case Item.ItemType.Shiled: SpawnObject = P_Shiled; break;
            case Item.ItemType.Spark: SpawnObject = P_Spark; break;


        }
    }

    IEnumerator StopAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        droppedItem.GetComponent<Item>().isDroped = false;
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
        if (itemList.ContainsKey(KeyName))
        {
            KeyCount = itemList[KeyName].Count;
        }
        return KeyCount;
    }

    public int GetCoinCount()
    {
        int CoinCount = 0;
        if (itemList.ContainsKey(Coin1_Name))
        {
            CoinCount += itemList[Coin1_Name].Count;
        }
        if (itemList.ContainsKey(Coin5_Name))
        {
            CoinCount += 5 * itemList[Coin5_Name].Count;
        }
        if (itemList.ContainsKey(Coin10_Name))
        {
            CoinCount += 10 * itemList[Coin10_Name].Count;
        }
        if (itemList.ContainsKey(Coin15_Name))
        {
            CoinCount += 15 * itemList[Coin15_Name].Count;
        }
        if (itemList.ContainsKey(Coin100_Name))
        {
            CoinCount += 100 * itemList[Coin100_Name].Count;
        }
        return CoinCount;
    }

    public int GetBombCount()
    {
        int BombCount = 0;

        if (itemList.ContainsKey(ForcedDeletionName))
        {
            BombCount = itemList[ForcedDeletionName].Count;
        }
        return BombCount;
    }

    public bool KeyUse()
    {
        // Ű �������� itemList�� �ְ�, �ش� ����Ʈ�� �������� �ϳ� �̻� �ִ��� Ȯ��
        if (itemList.ContainsKey(KeyName) && itemList[KeyName].Count > 0)
        {
            Item keyItem = itemList[KeyName][0]; // ù ��° ������ ��������
            statusManager.CurrentStorage -= keyItem.ItemSize; // ��� �� ����� �뷮 ���̱�
            itemList[KeyName].Remove(keyItem); // ����� ������ ����

            // �ش� Ű�� �� �̻� ������ ����Ʈ���� Ű ��ü�� ����
            if (itemList[KeyName].Count == 0)
            {
                itemList.Remove(KeyName);
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

    public bool ForcedDeletionUse()
    {
        // �������� itemList�� �ְ�, �ش� ����Ʈ�� �������� �ϳ� �̻� �ִ��� Ȯ��
        if (itemList.ContainsKey(ForcedDeletionName) && itemList[ForcedDeletionName].Count > 0)
        {
            Item ForcedItem = itemList[ForcedDeletionName][0]; // ù ��° ������ ��������
            statusManager.CurrentStorage -= ForcedItem.ItemSize; // ��� �� ����� �뷮 ���̱�
            itemList[ForcedDeletionName].Remove(ForcedItem); // ����� ������ ����

            // �ش� �������� �� �̻� ������ ����Ʈ���� ������ ��ü�� ����
            if (itemList[ForcedDeletionName].Count == 0)
            {
                itemList.Remove(ForcedDeletionName);
            }

            ui_0_HUD.UpdateHUD(); // HUD ������Ʈ
            Debug.Log("�������� ������ ����");
            return true;  // ��� ����
        }
        else
        {
            Debug.Log("����� �� �ִ� �������� �������� �����ϴ�.");
            return false;  // ��� ����
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
            case Item.ItemType.Coin100:
            case Item.ItemType.Key:
            case Item.ItemType.CardPack:
            case Item.ItemType.ForcedDeletion:
            case Item.ItemType.ProgramRemove:
            case Item.ItemType.ExpansionKit_1:
            case Item.ItemType.ExpansionKit_2:
            case Item.ItemType.ExpansionKit_3:
            case Item.ItemType.ProgramRecycle:
            case Item.ItemType.Card_Clover:
            case Item.ItemType.Card_Spade:
            case Item.ItemType.Card_Hearth:
            case Item.ItemType.Card_Dia:
            case Item.ItemType.Ticket_Random:
            case Item.ItemType.Ticket_Down:
            case Item.ItemType.Ticket_Shop:
            case Item.ItemType.Ticket_Special:
            case Item.ItemType.Ticket_BlackShop:
            case Item.ItemType.Ticket_Boss:
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
