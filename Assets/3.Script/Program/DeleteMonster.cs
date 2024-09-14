using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DeleteMonster : ChangeStatusProgram
{
    public static string StaticProgramName = "Delete Monster";
    public static string StaticExplanation = "몬스터 삭제 프로그램. 일정 주기마다 주변 몬스터가 삭제된다.";

    // Start is called before the first frame update
    void Start()
    {
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
            player.EnomyDelete = true;
        }

        Destroy(gameObject);
    }
}
