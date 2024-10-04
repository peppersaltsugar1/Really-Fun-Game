using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    public static StatusManager Instance;
    private UIManager uiManager;

    // Player Base Status
    public float MaxHp; // �ִ� ü��
    public float CurrentHp; // ���� ü��
    public float TemHp; // �ӽ� ü��(������)
    public float Shield; // ���� �����ִ� ��
    public float ShieldHp;
    public float Elect;

    private float HealCoolTime = 0.2f;
    private Coroutine healing_coroutine;

    // Take Damage
    [SerializeField]
    private Collider2D player_coroutine;
    private float HitCoolTime = 2.0f;
    private bool IsHit = false;

    // Player Attack Status
    public float AttackPower; // ���ݷ�
    public float AttackSpeed;  // ���ݼӵ�
    public float AttackPushPower; // ���ý� �а�
    public float WeaponDistance; // ĳ����~���� �Ÿ�

    // public Transform sPoint;    // ���� ���� ����Ʈ
    public float AngleRange = 35f; // ĳ���� ���� ���� ����

    //�÷��̾� �̵��ӵ�
    public float MoveSpeed;

    // ��Ÿ ����
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
    public void TakeDamage(float damage)
    {
        Debug.Log("Player Take Damage");

        if (HandleDefense(ref ShieldHp, damage, uiManager.ShiledSet, uiManager.ShiledOff)) return;

        if (HandleDefense(ref Elect, damage, uiManager.ElectDel)) return;

        if (HandleDefense(ref TemHp, damage, uiManager.TemHpSet, uiManager.TemHpDel)) return;

        if (Shield > 0 && Shield * 3 >= CurrentHp)
        {
            Shield -= 1;
            uiManager.ShiledOff(); 
            
            if (!IsHit)
            {
                StartCoroutine(Hit_Coroutine());
            }

            if (Shield < 0)
            {
                Shield = 0;
            }
            return;
        }

        if (CurrentHp > 0)
        {

            if (!IsHit)
            {
                StartCoroutine(Hit_Coroutine());
                CurrentHp -= damage;
                uiManager.HpSet();
            }

            if (CurrentHp <= 0)
            {
                // Die(); // ��� ó��
            }
        }
    }

    private bool HandleDefense(ref float defenseValue, float damage, System.Action onHitAction, System.Action onDepleteAction = null)
    {
        if (defenseValue > 0)
        {
            defenseValue -= damage;
            onHitAction.Invoke(); 
            
            if (!IsHit)
            {
                StartCoroutine(Hit_Coroutine());
            }

            if (defenseValue <= 0)
            {
                defenseValue = 0;
                onDepleteAction?.Invoke(); // ��� ������ �� �����Ǿ��� �� ó��
            }
            return true;
        }
        return false;
    }

    private IEnumerator Hit_Coroutine()
    {
        IsHit = true;

        yield return new WaitForSeconds(HitCoolTime);

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
        Debug.Log("Player Is Dead");
        Debug.Log("Die Function is not defined");
    }

    public void ElectShieldExplode()
    {
        Debug.Log("ElectShieldExplode Is Not Defined");
    }
}
