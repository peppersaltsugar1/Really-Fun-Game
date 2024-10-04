using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ProgramManager : MonoBehaviour
{
    public static ProgramManager Instance;
    private PoolingManager PInstance;
    private StatusManager statusManager;
    private Weapon weapon;

    // 내가 가지고 있는 프로그램 리스트
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
        statusManager = StatusManager.Instance;

        if (statusManager == null)
        {
            Debug.Log("statusManager is not find");
            return;
        }

        if (NewProgram.AddCoin != 0)
        {
            statusManager.CoinUp(NewProgram.AddCoin);
        }

        if (NewProgram.HPHeal != 0)
        {
            statusManager.Heal(NewProgram.HPHeal);
        }
        if (NewProgram.AttackPower != 0)
        {
            PInstance = FindObjectOfType<PoolingManager>();
            if (PInstance != null)
            {
                PInstance.RefreshBulletDamage(NewProgram.AttackPower);
            }
        }

        if (NewProgram.AttackSpeed != 0)
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

        if (NewProgram.MoveSpeed != 0)
        {
            statusManager.SetSpeed(NewProgram.MoveSpeed);
        }

        if (NewProgram.BulletSpeed != 0)
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
        Player player = GameObject.FindWithTag("Player").GetComponent<Player>();
        statusManager = StatusManager.Instance;

        if (statusManager == null)
        {
            Debug.Log("statusManager is not find");
            return;
        }

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
            statusManager.SetSpeed(-ProgramList[ProgramNumber].MoveSpeed);
        }

        if (ProgramList[ProgramNumber].BulletSpeed != 0)
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
