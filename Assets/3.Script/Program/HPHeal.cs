using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPHeal : ChangeStatusProgram
{
    private int HealingHP;
    public static string StaticProgramName = "HP Heal";
    public static string StaticExplanation = "힐링 프로그램. 체력이 참";

    // Start is called before the first frame update
    void Start()
    {
        HealingHP = 5;
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
