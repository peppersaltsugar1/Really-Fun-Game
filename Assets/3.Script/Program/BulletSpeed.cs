using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpeed : ChangeStatusProgram
{
    private float ChangeSpeed;
    private PoolingManager Instance;

    // Start is called before the first frame update
    void Start()
    {
        ChangeSpeed = 50.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        Instance = FindObjectOfType<PoolingManager>();
     
        if (Instance != null)
        {
            Instance.RefreshBulletSpeed(ChangeSpeed);
            Debug.Log("EAT THE ITEM");
        }

        Destroy(gameObject);
    }
}
