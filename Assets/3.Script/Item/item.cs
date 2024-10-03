using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class item : MonoBehaviour
{
    public enum ItemType
    {
        Coin, Heal, TemHp, Shiled, Spark
    }
    public int itemScore;
    private GameManager gameManager;
    public ItemType itemType;
    //아이템종
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            switch (itemType)
            {
                case ItemType.Coin:
                    CoinItem();
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

    private void HealItem()
    {
        gameManager.player.Heal(itemScore);
    }
    private void CoinItem()
    {
        gameManager.player.CoinUp(itemScore);
    }

    private void TemHpItem()
    {
        gameManager.player.TemHpUp(itemScore);
    }
    private void ShiledItem()
    {
        gameManager.player.TemHpUp(itemScore);
    }
    private void SparkItem()
    {
        gameManager.player.ElectUp(itemScore);
    }
}
