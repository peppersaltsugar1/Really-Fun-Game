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
    public List<Program> programList = new List<Program>();
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
        Debug.Log("AddProgram");
    }

    public string GetProgramName1(int ProgramNumber)
    {
        return ProgramList[ProgramNumber].ProgramName;
    }

    public string GetProgramExplanation1(int ProgramNumber)
    {
        return ProgramList[ProgramNumber].Explanation;
    }

    public void RemoveProgram(int ProgramNumber)
    {
        // ���� ����
        if (ProgramList[ProgramNumber].AttackPower != 0)
        {
            PInstance = FindObjectOfType<PoolingManager>();

            if (Instance != null)
            {
                PInstance.RefreshBulletDamage(-ProgramList[ProgramNumber].AttackPower);
            }
        }
        else if (ProgramList[ProgramNumber].AttackSpeed != 0)
        {
            // Player_Ruko�� Tag�� Player�� �ٲ�� ��
            Player player = GameObject.FindWithTag("Player").GetComponent<Player>();

            if (player != null)
            {
                weapon = player.GetWeapon();

                if (weapon != null)
                {
                    weapon.SetAttackSpeed(-ProgramList[ProgramNumber].AttackSpeed);
                }
            }
        }
        else if (ProgramList[ProgramNumber].MoveSpeed != 0)
        {
            Player player = GameObject.FindWithTag("Player").GetComponent<Player>();
            if (player != null)
            {
                player.SetSpeed(-ProgramList[ProgramNumber].MoveSpeed);
            }
        }
        else if(ProgramList[ProgramNumber].BulletSpeed != 0)
        {
            PInstance = FindObjectOfType<PoolingManager>();

            if (Instance != null)
            {
                PInstance.RefreshBulletSpeed(-ProgramList[ProgramNumber].BulletSpeed);
            }
        }

        // Transform.GetSiblingIndex()�� ����ؼ� �� ��° ��ư������ �޾ƿ� ��
        ProgramList.RemoveAt(ProgramNumber);
    }

    // �Ʒ� �Լ����� ��Ȱ��ȭ ������.

    // ������ �������� �ż���
    public string GetProgramExplanation(int CurProgramNum)
    {
        switch (CurProgramNum)
        {
            case 0:
                return AttackPower.StaticExplanation;
                break;
            case 1:
                return AttackSpeed.StaticExplanation;
                break;
            case 2:
                return BulletSpeed.StaticExplanation;
                break;
            case 3:
                return DeleteMonster.StaticExplanation;
                break;
            case 4:
                return HPHeal.StaticExplanation;
                break;
            case 5:
                return MovingSpeed.StaticExplanation;
                break;
        }

        Debug.Log("Error");
        return "error";
    }

    // ���α׷� �̸��� �������� �ż���
    public string GetProgramName(int CurProgramNum)
    {
        switch (CurProgramNum)
        {
            case 0:
                return AttackPower.StaticProgramName;
                break;
            case 1:
                return AttackSpeed.StaticProgramName;
                break;
            case 2:
                return BulletSpeed.StaticProgramName;
                break;
            case 3:
                return DeleteMonster.StaticProgramName;
                break;
            case 4:
                return HPHeal.StaticProgramName;
                break;
            case 5:
                return MovingSpeed.StaticProgramName;
                break;
        }

        Debug.Log("Error");
        return "error";
    }

    // ���α׷��� �߰��ϴ� ����Ʈ
    public void AddProgram(int CurProgramNum)
    {
        switch (CurProgramNum)
        {
            case 0:
                AttackPower attackPower = new AttackPower();
                programList.Add(attackPower);
                break;
            case 1:
                AttackSpeed attackSpeed = new AttackSpeed();
                programList.Add(attackSpeed);
                break;
            case 2:
                BulletSpeed bulletSpeed = new BulletSpeed();
                programList.Add(bulletSpeed);
                break;
            case 3:
                DeleteMonster deleteMonster = new DeleteMonster();
                programList.Add(deleteMonster);
                break;
            case 4:
                HPHeal hpHeal = new HPHeal();
                programList.Add(hpHeal);
                break;
            case 5:
                MovingSpeed movingSpeed = new MovingSpeed();
                programList.Add(movingSpeed);
                break;
        }
    }

    public void RemoveList(string ProgName)
    {
        //bool removed = programList.Remove(ProgName);  // Remove�� ���� �� true, ���� �� false ��ȯ

        //if (removed)
        //{
        //    Debug.Log("Object " + ProgName + " deleted.");
        //}
        //else
        //{
        //    Debug.LogWarning("Object " + ProgName + " not found in the list.");

        //}

    }
}
