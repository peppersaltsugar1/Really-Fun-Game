using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAttackPower : MonoBehaviour
{
    public int i_ChangePower;
    // private Bullet bullet;
    private PoolingManager Instance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        // bullet = collision.gameObject.GetComponent<Bullet>();

        Instance = FindObjectOfType<PoolingManager>();
        if (Instance != null)
        {
            Instance.RefreshBulletDamage(i_ChangePower);
        }

        Destroy(gameObject);
    }
}
