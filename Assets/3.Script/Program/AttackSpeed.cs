using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSpeed : ChangeStatusProgram
{
    private float ChangeValue;
    private Weapon weapon;

    // Start is called before the first frame update
    void Start()
    {
        // ���� �ӵ� ����.
        // ���� ���� ��� �ӵ� up
        ChangeValue = -1.5f;
        Explanation = "���ݼӵ� ���� ���α׷�";
        DeleteIsPossible = true;
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
