using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPower : ChangeStatusProgram
{

    private int ChangePower;
    // private Bullet bullet;
    private PoolingManager Instance;
    public static string StaticProgramName = "Attack Power";
    public static string StaticExplanation = "공격력 증가 프로그램";
    // Start is called before the first frame update
    void Start()
    {
        ChangePower = 100;
        DeleteIsPossible = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        // bullet = collision.gameObject.GetComponent<Bullet>();

        Instance = FindObjectOfType<PoolingManager>();
        if(Instance != null)
        {
            Instance.RefreshBulletDamage(ChangePower);
            Debug.Log("EAT THE ITEM");
        }

        Destroy(gameObject);
    }
}
