using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PMoveSpeed : MonoBehaviour
{
    public float f_NewSpeedValue;

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
        Player player = collision.gameObject.GetComponent<Player>();

        if (player != null)
        {
            player.SetSpeed(f_NewSpeedValue);
        }

        Destroy(gameObject);
    }
}
