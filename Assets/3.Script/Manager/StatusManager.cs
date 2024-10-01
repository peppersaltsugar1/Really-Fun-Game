using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    public static StatusManager Instance;
    private PoolingManager PInstance;
    private Weapon weapon;

    // ���� ������ �ִ� ���α׷� ����Ʈ
    public List<PInformation> ProgramList = new List<PInformation>();

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
    }

    public void AddProgramList(PInformation NewProgram)
    {
        ProgramList.Add(NewProgram);

        Player player = GameObject.FindWithTag("Player").GetComponent<Player>();

        if (NewProgram.AddCoin != 0)
        {
            if (player != null)
            {
                player.CoinUp(NewProgram.AddCoin);
            }
        }

        if (NewProgram.HPHeal != 0)
        {
            if (player != null)
            {
                player.Heal(NewProgram.HPHeal);
            }
        }
        if(NewProgram.AttackPower != 0)
        {
            PInstance = FindObjectOfType<PoolingManager>();
            if (PInstance != null)
            {
                PInstance.RefreshBulletDamage(NewProgram.AttackPower);
            }
        }

        if(NewProgram.AttackSpeed != 0)
        {
            if (player != null)
            {
                weapon = player.GetWeapon();

                if (weapon != null)
                {
                    weapon.SetAttackSpeed(NewProgram.AttackSpeed);
                }
            }
        }

        if(NewProgram.MoveSpeed != 0)
        {
            if (player != null)
            {
                player.SetSpeed(NewProgram.MoveSpeed);
            }
        }

        if(NewProgram.BulletSpeed != 0)
        {
            PInstance = FindObjectOfType<PoolingManager>();

            if (PInstance != null)
            {
                PInstance.RefreshBulletSpeed(NewProgram.BulletSpeed);
            }
        }

        // Debug.Log("AddProgram");
    }

    public void RemoveProgram(int ProgramNumber)
    {
        // Player_Ruko�� Tag�� Player�� �ٲ�� ��
        Player player = GameObject.FindWithTag("Player").GetComponent<Player>();

        // ���� ����
        if (ProgramList[ProgramNumber].AttackPower != 0)
        {
            PInstance = FindObjectOfType<PoolingManager>();

            if (Instance != null)
            {
                PInstance.RefreshBulletDamage(-ProgramList[ProgramNumber].AttackPower);
            }
        }

        if (ProgramList[ProgramNumber].AttackSpeed != 0)
        {

            if (player != null)
            {
                weapon = player.GetWeapon();

                if (weapon != null)
                {
                    weapon.SetAttackSpeed(-ProgramList[ProgramNumber].AttackSpeed);
                }
            }
        }

        if (ProgramList[ProgramNumber].MoveSpeed != 0)
        {
            if (player != null)
            {
                player.SetSpeed(-ProgramList[ProgramNumber].MoveSpeed);
            }
        }
        
        if(ProgramList[ProgramNumber].BulletSpeed != 0)
        {
            PInstance = FindObjectOfType<PoolingManager>();

            if (Instance != null)
            {
                PInstance.RefreshBulletSpeed(-ProgramList[ProgramNumber].BulletSpeed);
            }
        }

        ProgramList.RemoveAt(ProgramNumber);
    }
}
