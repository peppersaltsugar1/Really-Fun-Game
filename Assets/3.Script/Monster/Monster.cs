using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public enum MonsterType
    {
        TypeA,
        TypeB,
        TypeC
    }
    public MonsterType monsterType; // 몬스터 타입
    public float speed;
    public float atk;
    public float hp;
    private GameManager gameManager;
    void Start()
    {
        gameManager = GameManager.Instance;
        InitializeMonster();
    }

    void Update()
    {
        MonsterAction();
    }
    public void InitializeMonster()//몬스터 타입에따라 스텟 부여
    {
        switch (monsterType)
        {
            case MonsterType.TypeA:
                speed = 3f;
                hp = 100;
                atk = 10;
                break;
            case MonsterType.TypeB:
                speed = 2f;
                hp = 150;
                atk = 20;
                break;
            case MonsterType.TypeC:
                speed = 1f;
                hp = 200;
                atk = 30;
                break;
        }
    }
    private void MoveTypeA()
    {
        // 플레이어 위치를 찾아서 플레이어를 추적하는 기능
        if (gameManager.playerPoint!= null)
        {
            Vector3 direction = (gameManager.playerPoint.transform.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }
    private void MoveTypeB()
    {
        // TypeB의 이동 로직 (예: 좌우로 패트롤)
        transform.Translate(Vector3.left * speed * Time.deltaTime);
    }

    private void MoveTypeC()
    {
        // TypeC의 이동 로직 (예: 일정한 패턴으로 이동)
        float y = Mathf.Sin(Time.time * speed) * 0.5f;
        transform.Translate(new Vector3(speed * Time.deltaTime, y, 0));
    }
    private void MonsterAction()
    {
        switch (monsterType)
        {
            case MonsterType.TypeA:
                MoveTypeA();
                break;
            case MonsterType.TypeB:
                MoveTypeB();
                break;
            case MonsterType.TypeC:
                MoveTypeC();
                break;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if(player != null)
            {
                player.TakeDamage(atk);
            }
        }
    }
}
