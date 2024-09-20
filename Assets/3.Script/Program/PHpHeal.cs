using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PHpHeal : MonoBehaviour
{
    public int i_AddHPValue;

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
            player.Heal(i_AddHPValue);
        }

        Destroy(gameObject);
    }
}
