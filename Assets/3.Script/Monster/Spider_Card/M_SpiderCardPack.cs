using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class M_SpiderCardPack : MonsterBase
{
    public Transform FirePoint;      // ÃÑ¾Ë ¹ß»ç À§Ä¡
    public float BulletSpeed = 10f;  // ÃÑ¾Ë ¼Óµµ
    // ÇÁ¸®ÆÕ ¸®½ºÆ® 
    public GameObject SpiderPrefab1;
    public GameObject SpiderPrefab2;
    public GameObject SpiderPrefab3;
    public GameObject SpiderPrefab4;

    protected override void Start()
    {
        GameManager = GameManager.Instance;
        statusManager = StatusManager.Instance;
        SpriteRenderer = GetComponent<SpriteRenderer>();
        MAnimator = GetComponentInChildren<Animator>();
        StartCoroutine(MonsterRoutine());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override IEnumerator MonsterRoutine()
    {
        while (true)
        {
            yield return AttackPreparation();
            yield return new WaitForSeconds(SearchingCoolTime);
        }
    }

    public override IEnumerator AttackPreparation()
    {
        int SpiderColor = Random.Range(0, 4); // 0, 1 Èò»ö. 2, 3 »¡°­»ö
        switch (SpiderColor)
        {
            case 0:
            case 1:
                MAnimator.SetTrigger("White_Spawn");
                break;
            case 2:
            case 3:
                MAnimator.SetTrigger("Red_Spawn");
                break;
        }
        // Animation Loading
        yield return new WaitForSeconds(10.6f);

        switch (SpiderColor)
        {
            case 0:
               Instantiate(SpiderPrefab1, FirePoint.position, FirePoint.rotation);
                break;
            case 1:
                Instantiate(SpiderPrefab2, FirePoint.position, FirePoint.rotation);
                break;
            case 2:
                Instantiate(SpiderPrefab3, FirePoint.position, FirePoint.rotation);
                break;
            case 3:
                Instantiate(SpiderPrefab4, FirePoint.position, FirePoint.rotation);
                break;
        }

        yield return new WaitForSeconds(AttackCoolTime);
    }
}
