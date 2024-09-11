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
        // 공격 속도 증가.
        // 음수 값일 경우 속도 up
        ChangeValue = -1.5f;
        Explanation = "공격속도 증가 프로그램";
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
