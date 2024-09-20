using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PAttackSpeed : MonoBehaviour
{
    public float f_NewSpeedValue;
    private Weapon weapon;

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
            weapon = player.GetWeapon();

            if (weapon != null)
            {
                weapon.SetAttackSpeed(f_NewSpeedValue);
            }
        }

        Destroy(gameObject);
    }
}
