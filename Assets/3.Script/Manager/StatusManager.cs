using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    public static StatusManager Instance;

    // 내가 가지고 있는 프로그램 리스트
    public List<Program> programList = new List<Program>();


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

    // 설명을 가져오는 매서드
    public string GetProgramExplanation(int CurProgramNum)
    {
        switch(CurProgramNum)
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

    // 프로그램 이름을 가져오는 매서드
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

    // 프로그램을 추가하는 리스트
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

}
