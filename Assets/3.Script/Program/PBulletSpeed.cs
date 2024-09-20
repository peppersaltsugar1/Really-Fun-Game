using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PBulletSpeed : MonoBehaviour
{
    public float f_NewSpeedValue;
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
        Instance = FindObjectOfType<PoolingManager>();

        if (Instance != null)
        {
            Instance.RefreshBulletSpeed(f_NewSpeedValue);
        }

        Destroy(gameObject);
    }
}
