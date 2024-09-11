using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPHeal : ChangeStatusProgram
{
    private int HealingHP;

    // Start is called before the first frame update
    void Start()
    {
        HealingHP = 5;
        Explanation = "���� ���α׷�. ü���� ��";
        DeleteIsPossible = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            player.Heal(HealingHP);
            Debug.Log("EAT THE ITEM");
        }

        Destroy(gameObject);
    }
}
