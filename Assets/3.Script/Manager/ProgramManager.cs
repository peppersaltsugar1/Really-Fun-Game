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

    // ���� ������ �ִ� ���α׷� ����Ʈ
    public List<PInformation> ProgramList = new List<PInformation>();
    [Header("��������Ʈ ����")]
    [SerializeField]
    Material bulletMa;
    [SerializeField]
    float interval;
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
                //PInstance.RefreshBulletDamage(NewProgram.AttackPower);
                statusManager.AttackPower += NewProgram.AttackPower;
            }
        }

        if (NewProgram.AttackSpeed != 0)
        {
            if (player != null)
            {
                weapon = player.GetWeapon();

                if (weapon != null)
                {
                    statusManager.AttackSpeed += NewProgram.AttackSpeed;
                    //weapon.SetAttackSpeed(NewProgram.AttackSpeed);
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
                statusManager.BulletSpeed += NewProgram.BulletSpeed;
            }
        }
        if (NewProgram.AttackPerUp != 0)
        {
            PInstance = FindObjectOfType<PoolingManager>();

            if (PInstance != null)
            {
                statusManager.AttackPower += statusManager.AttackPower*NewProgram.AttackPerUp;
            }
        }
        if (NewProgram.AttackSpeedPerUp != 0)
        {
            PInstance = FindObjectOfType<PoolingManager>();

            if (PInstance != null)
            {
                statusManager.AttackSpeed += statusManager.AttackSpeed * NewProgram.AttackSpeedPerUp;
            }
        }
        if (NewProgram.MoveSpeedPerUp != 0)
        {
            PInstance = FindObjectOfType<PoolingManager>();

            if (PInstance != null)
            {
                statusManager.MoveSpeed += statusManager.MoveSpeed * NewProgram.MoveSpeedPerUp;
            }
        }
        if (NewProgram.bulletSpeedPerUp != 0)
        {
            PInstance = FindObjectOfType<PoolingManager>();

            if (PInstance != null)
            {
                statusManager.BulletSpeed += statusManager.BulletSpeed * NewProgram.bulletSpeedPerUp;
            }
        }
        if (NewProgram.AttackPerDown != 0)
        {
            PInstance = FindObjectOfType<PoolingManager>();

            if (PInstance != null)
            {
                statusManager.AttackPower *= NewProgram.AttackPerDown;
            }
        }
        if (NewProgram.AttackSpeedPerDown != 0)
        {
            PInstance = FindObjectOfType<PoolingManager>();

            if (PInstance != null)
            {
                statusManager.AttackSpeed *= NewProgram.AttackSpeedPerDown;
            }
        }
        if (NewProgram.MoveSpeedPerDown != 0)
        {
            PInstance = FindObjectOfType<PoolingManager>();

            if (PInstance != null)
            {
                statusManager.MoveSpeed *= NewProgram.MoveSpeedPerDown;
            }
        }
        if (NewProgram.bulletSpeedPerDown != 0)
        {
            PInstance = FindObjectOfType<PoolingManager>();

            if (PInstance != null)
            {
                statusManager.BulletSpeed *=  NewProgram.bulletSpeedPerDown;
            }
        }
        if (NewProgram.ProgramName =="���� ����Ʈ")
        {
            StartCoroutine(AttackEffect_co());
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
                statusManager.AttackPower -= ProgramList[ProgramNumber].AttackPower;
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
                statusManager.BulletSpeed -= ProgramList[ProgramNumber].BulletSpeed;
            }
        }
      
        if (ProgramList[ProgramNumber].AttackPerUp != 0)
        {
            PInstance = FindObjectOfType<PoolingManager>();

            if (PInstance != null)
            {
                statusManager.AttackPower -= statusManager.AttackPower * ProgramList[ProgramNumber].AttackPerUp;
            }
        }
        if (ProgramList[ProgramNumber].AttackSpeedPerUp != 0)
        {
            PInstance = FindObjectOfType<PoolingManager>();

            if (PInstance != null)
            {
                statusManager.AttackSpeed -= statusManager.AttackSpeed * ProgramList[ProgramNumber].AttackSpeedPerUp;
            }
        }
        if (ProgramList[ProgramNumber].MoveSpeedPerUp != 0)
        {
            PInstance = FindObjectOfType<PoolingManager>();

            if (PInstance != null)
            {
                statusManager.MoveSpeed -= statusManager.MoveSpeed * ProgramList[ProgramNumber].MoveSpeedPerUp;
            }
        }
        if (ProgramList[ProgramNumber].bulletSpeedPerUp != 0)
        {
            PInstance = FindObjectOfType<PoolingManager>();

            if (PInstance != null)
            {
                statusManager.BulletSpeed -= statusManager.BulletSpeed * ProgramList[ProgramNumber].bulletSpeedPerUp;
            }
        }
        if (ProgramList[ProgramNumber].ProgramName == "���� ����Ʈ")
        {
            StopCoroutine(AttackEffect_co());
        }
        ProgramList.RemoveAt(ProgramNumber);
    }
    private IEnumerator AttackEffect_co()
    {

        // Bullet ������Ʈ�� ���� ���׸����� ��������

        while (true) // ���� ����
        {
            // ���� ���� 0.5�� 1 ���̷� �����ϰ� ����
            float alpha = Random.Range(0.5f, 1f);

            // ���� ���� (���� ���İ� ����)
            Color color = bulletMa.color;
            color.a = alpha;
            bulletMa.color = color;

            // ���� ������� ��� (interval��ŭ ���)
            yield return new WaitForSeconds(interval);
        }
    } 
    
}
