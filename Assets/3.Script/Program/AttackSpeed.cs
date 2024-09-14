using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackSpeed : ChangeStatusProgram
{
    private float ChangeValue;
    private Weapon weapon;
    public static string StaticProgramName = "Attack Speed";
    public static string StaticExplanation = "���ݼӵ� ���� ���α׷�";
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        // ���� �ӵ� ����.
        // ���� ���� ��� �ӵ� up
        ChangeValue = -1.5f;
        DeleteIsPossible = true;
        SetSprite("Program_1~20", 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSpeed(float Value)
    {
        weapon.SetAttackSpeed(ChangeValue);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            weapon = player.GetWeapon();

            if (weapon != null)
            {
                SetSpeed(ChangeValue);
                Debug.Log("CAS IN ITEM");
            }
        }

        Destroy(gameObject);
    }
}
