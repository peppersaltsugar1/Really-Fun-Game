using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    #region Manager

    public static StatusManager Instance;
    private FolderManager folderManager;
    private UIManager uiManager;
    private UI_0_HUD ui_0_HUD;

    #endregion

    #region Player Base Status

    // When game is started, this base status used to Initializing Status based on this list.
    [Header("Player Base Status")]
    public float B_MaxHp;
    public float B_CurrentHp;
    public float B_TemHp;
    public float B_Shield;
    public float B_ShieldHp;
    public float B_Elect;
    public float B_HealCoolTime = 0.2f; // Playher Heal CoolTime
    private float B_HitCoolTime = 2.0f; // Player Attacked CoolTime

    // Player Attack Status
    public float B_AttackPower; // Player Attack Power
    public float B_AttackSpeed;  // Player Attack Speed
    public float B_AttackPushPower; // When Player Attacking, Push Power
    public float B_WeaponDistance; // Distance Between Player and Weapon
    public float B_AngleRange = 35f; // Player Weapon Angle Range
    public float B_BulletSpeed = 5.0f; // Bullet Speed
    public float B_BulletMaximumRange; // �Ѿ� ��Ÿ�

    // Other Status
    public float B_MoveSpeed; // �̵��ӵ�
    public int B_Coin; // ���� ����
    public int B_CurrentStorage;
    public int B_MaxStorage;

    #endregion

    #region Dynamic Status

    // When player in game, playing with this status
    // This status are no need to reset.
    [Header("Player Dynamic Status")]
    public float MaxHp; // �ִ� ü��
    public float CurrentHp; // ���� ü��
    public float TemHp; // �ӽ� ü��(������)
    public float Shield; // ���� �����ִ� ��
    public float ShieldHp;
    public float Elect;
    public bool HPisFull = false;

    public MonsterBase.MonsterType DeathSign; // �������
    public float HealCoolTime;

    private Coroutine healing_coroutine;
    private Coroutine TempHealing_coroutine;

    private float HitCoolTime;
    private bool IsHit = false;

    public float AttackPower; // ���ݷ�
    public float AttackSpeed;  // ���ݼӵ�
    public float AttackPushPower; // ���ý� �а�
    public float WeaponDistance; // ĳ����~���� �Ÿ�
    public float AngleRange; // ĳ���� ���� ���� ����
    public float BulletSpeed;
    public float BulletMaximumRange;

    public float MoveSpeed;
    public int Coin;
    public int CurrentStorage;
    public int MaxStorage;

    #endregion

    #region Item

    [Header("Item Drop Physical Force")]
    public float DragForce;
    public float DropForce;
    public float AbsorptionSpeed;
    public float GetDistance;
    public float MaxDistance;

    [Header("Item Effect")]
    public float ElectValue;
    #endregion

    #region Default / Init Function

    void Awake()
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

    void Start()
    {
        uiManager = UIManager.Instance;
        ui_0_HUD = UI_0_HUD.Instance;
        folderManager = FolderManager.Instance;
        InitializeStatus();
    }

    public void InitializeStatus()
    {
        MaxHp = B_MaxHp;
        CurrentHp = B_CurrentHp;
        TemHp = B_TemHp;
        Shield = B_Shield;
        B_ShieldHp = ShieldHp;
        Elect = B_Elect;
        HealCoolTime = B_HealCoolTime;
        HitCoolTime = B_HitCoolTime;

        AttackPower = B_AttackPower;
        AttackSpeed = B_AttackSpeed;
        AttackPushPower = B_AttackPushPower;
        WeaponDistance = B_WeaponDistance;
        AngleRange = B_AngleRange;
        BulletSpeed = B_BulletSpeed;
        BulletMaximumRange = B_BulletMaximumRange;

        MoveSpeed = B_MoveSpeed;
        Coin = B_Coin;
        CurrentStorage = B_CurrentStorage;
        MaxStorage = B_MaxStorage;
    }

    #endregion

    #region Damage Logic

    public void TakeDamage(float damage, MonsterBase.MonsterType deathSign)
    {
        if (!IsHit)
        {
            Debug.Log("Player Take Damage");
            DeathSign = deathSign;
            StartCoroutine(Hit_Coroutine(damage));
        }
    }

    private IEnumerator Hit_Coroutine(float damage)
    {
        IsHit = true;

        if ((int)Shield > 0)
        {
            Debug.Log("���� ���͸� ����");
            Shield--;   // ���� ���� �ϳ� ���̰�
            ui_0_HUD.ShiledSet(); // HP�� ���� ����

            yield return new WaitForSeconds(HitCoolTime);
            IsHit = false;
            yield break;
        }

        if (Elect > 0)
        {
            Debug.Log("���� ���͸� ����");
            Elect -= damage;
            ui_0_HUD.ExceptHp_BarSet();
            ui_0_HUD.UpdateHpUI();

            if (Elect <= 0)
            {
                Elect = 0;
                ElectShieldExplode();
            }

            yield return new WaitForSeconds(HitCoolTime);
            IsHit = false;
            yield break;
        }

        if (TemHp > 0)
        {
            Debug.Log("�ӽ� ü�� �Ҹ�");
            TemHp -= damage;
            ui_0_HUD.ExceptHp_BarSet();
            ui_0_HUD.UpdateHpUI();

            yield return new WaitForSeconds(HitCoolTime);
            IsHit = false;
            yield break;
        }

        if (ShieldHp > 0 || CurrentHp > 0)
        {
            Debug.Log("HP + ���� �Ҹ�");
            ui_0_HUD.DamagedHP();

            if (CurrentHp <= 0)
            {
                Die(); // ��� ó��
            }

            yield return new WaitForSeconds(HitCoolTime);
            IsHit = false;
            yield break;
        }

        IsHit = false;
    }

    private void Die()
    {
        uiManager.PlayerIsDead();
    }

    #endregion

    #region ItmeEffects

    public void SetSpeed(float newSpeed)
    {
        this.MoveSpeed += newSpeed;
    }

    public void CoinUp(int coinNum)
    {
        Coin += coinNum;
    }

    public void ElectUp(int electNum)
    {
        Elect += electNum;
        ui_0_HUD.ExceptHp_BarSet();
        ui_0_HUD.UpdateHpUI();
    }

    public void ShieldHpUp(int shieldNum)
    {
        Shield += shieldNum;
        ui_0_HUD.ExceptHp_BarSet();
        ui_0_HUD.UpdateHpUI();
    }

    public void TemHpUp(int temHpNum)
    {
        if (TempHealing_coroutine != null)
        {
            StopCoroutine(TemHpCoroutine(temHpNum));
        }
        TempHealing_coroutine = StartCoroutine(TemHpCoroutine(temHpNum));

        Debug.Log("TempHP+");
    }

    private IEnumerator TemHpCoroutine(int temHpNum)
    {
        for (int i = 0; i < temHpNum; i++)
        {
            TemHp++;
            ui_0_HUD.ExceptHp_BarSet();
            ui_0_HUD.UpdateHpUI();
            yield return new WaitForSeconds(HealCoolTime);
        }
        TempHealing_coroutine = null;

        HPisFull = false;
    }

    public void Heal(int healNum)
    {
        if (healing_coroutine != null)
        {
            StopCoroutine(HealingCoroutine(healNum));
        }
        healing_coroutine = StartCoroutine(HealingCoroutine(healNum));

        Debug.Log("HP+");
    }

    private IEnumerator HealingCoroutine(int healNum)
    {
        for (int i = 0; i < healNum; i++)
        {
            if (CurrentHp < MaxHp)
            {
                CurrentHp += 1;
                CurrentHp = Mathf.Min(CurrentHp, MaxHp);
            }
            ui_0_HUD.UpdateHpUI();
            yield return new WaitForSeconds(HealCoolTime);
        }
        healing_coroutine = null;

        HPisFull = false;
    }

    public void ElectShieldExplode()
    {
        List<MonsterBase> monsterList = folderManager?.CurrentFolder.FindAndReturnMonster();
        foreach (MonsterBase monster in monsterList)
        {
            monster.Damaged(ElectValue);
        }
    }

    public void MaxStorageUp(int Value)
    {
        MaxStorage += Value;
    }

    #endregion

}
