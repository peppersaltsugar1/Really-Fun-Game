using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    public static StatusManager Instance;
    private UIManager uiManager;

    // When game is started, this base status used to Initializing Status based on this list.
    // Player Base Status
    public float B_MaxHp;
    public float B_CurrentHp;
    public float B_TemHp;
    public float B_Shield;
    public float B_ShieldHp;
    public float B_Elect;
    private float B_HealCoolTime = 0.2f; // Playher Heal CoolTime
    private float B_HitCoolTime = 2.0f; // Player Attacked CoolTime

    // Player Attack Status
    public float B_AttackPower; // Player Attack Power
    public float B_AttackSpeed;  // Player Attack Speed
    public float B_AttackPushPower; // When Player Attacking, Push Power
    public float B_WeaponDistance; // Distance Between Player and Weapon
    public float B_AngleRange = 35f; // Player Weapon Angle Range

    // Other Status
    public float B_MoveSpeed; // 이동속도
    public int B_Coin; // 코인 개수
    public int B_CurrentStorage;


    // When player in game, playing with this status
    // This status are no need to reset.
    // Player Dynamic Status
    public float MaxHp; // 최대 체력
    public float CurrentHp; // 현재 체력
    public float TemHp; // 임시 체력(아이템)
    public float Shield; // 공격 막아주는 것
    public float ShieldHp;
    public float Elect;
    public MonsterBase.MonsterType DeathSign; // 사망원인
    private float HealCoolTime;
    private Coroutine healing_coroutine;
    private float HitCoolTime;
    private bool IsHit = false;
    public float AttackPower; // 공격력
    public float AttackSpeed;  // 공격속도
    public float AttackPushPower; // 어택시 밀격
    public float WeaponDistance; // 캐릭터~무기 거리
    public float AngleRange; // 캐릭터 무기 각도 범위
    public float MoveSpeed;
    public int Coin;
    public int CurrentStorage;


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
        MoveSpeed = B_MoveSpeed;
        Coin = B_Coin;
        CurrentStorage = B_CurrentStorage;
    }

    // =============================== Fixed Section ===============================
    public void TakeDamage(float damage, MonsterBase.MonsterType deathSign)
    {
        if (!IsHit)
        {
            // Debug.Log("Player Take Damage");
            DeathSign = deathSign;
            StartCoroutine(Hit_Coroutine(damage));
        }
    }

    private IEnumerator Hit_Coroutine(float damage)
    {
        IsHit = true;

        if (ShieldHp > 0)
        {
            ShieldHp -= damage;
            uiManager.ShiledSet();

            if (ShieldHp <= 0)
            {
                ShieldHp = 0;
            }

            yield return new WaitForSeconds(HitCoolTime);
            IsHit = false;
            yield break;
        }

        if (Elect > 0)
        {
            Elect -= damage;
            uiManager.ElectDel();

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
            TemHp -= damage;
            uiManager.HpSet();

            if (TemHp <= 0)
            {
                uiManager.TemHpDel();
                TemHp = 0;
            }

            yield return new WaitForSeconds(HitCoolTime);
            IsHit = false;
            yield break;
        }

        if (Shield > 0 && Shield * 3 >= CurrentHp)
        {
            Shield -= 1;
            uiManager.ShiledOff();

            if (Shield <= 0)
            {
                Shield = 0;
            }

            yield return new WaitForSeconds(HitCoolTime);
            IsHit = false;
            yield break;
        }


        if (CurrentHp > 0)
        {
            CurrentHp -= damage;
            uiManager.HpSet();

            if (CurrentHp <= 0)
            {
                Die(); // 사망 처리
            }

            yield return new WaitForSeconds(HitCoolTime);
        }

        IsHit = false;
    }

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
    }

    public void ShieldHpUp(int shieldNum)
    {
        ShieldHp += shieldNum;
    }

    public void TemHpUp(int temHpNum)
    {
        TemHp += temHpNum;
        uiManager.TemHpSet();
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
            yield return new WaitForSeconds(HealCoolTime);
        }

        healing_coroutine = null;
    }

    private void Die()
    {
        uiManager.PlayerIsDead();
    }

    public void ElectShieldExplode()
    {
        Debug.Log("ElectShieldExplode Is Not Defined");
    }
}
