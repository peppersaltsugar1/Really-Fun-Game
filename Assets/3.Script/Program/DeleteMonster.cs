using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DeleteMonster : ChangeStatusProgram
{
    // Start is called before the first frame update
    void Start()
    {
        Explanation = "���� ���� ���α׷�. ���� �ֱ⸶�� �ֺ� ���Ͱ� �����ȴ�.";
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
