using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    public static StatusManager Instance;
    private UIManager uiManager;

    // Player Base Status
    public float MaxHp; // 최대 체력
    public float CurrentHp; // 현재 체력
    public float TemHp; // 임시 체력(아이템)
    public float Shield; // 공격 막아주는 것
    public float ShieldHp;
    public float Elect;
    public MonsterBase.MonsterType DeathSign; // 사망원인
    private float HealCoolTime = 0.2f;
    private Coroutine healing_coroutine;

    // Take Damage
    //[SerializeField]
    //private Collider2D player_coroutine;
    private float HitCoolTime = 2.0f;
    private bool IsHit = false;


    // Player Attack Status
    public float AttackPower; // 공격력
    public float AttackSpeed;  // 공격속도
    public float AttackPushPower; // 어택시 밀격
    public float WeaponDistance; // 캐릭터~무기 거리

    // public Transform sPoint;    // 공격 시작 포인트
    public float AngleRange = 35f; // 캐릭터 무기 각도 범위

    //플레이어 이동속도
    public float MoveSpeed;

    // 기타 스텟
    public int Coin;

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
        CurrentHp = MaxHp;
    }

    // =============================== Fixed Section ===============================
    public void TakeDamage(float damage, MonsterBase.MonsterType deathSign)
    {
        if (!IsHit)
        {
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
