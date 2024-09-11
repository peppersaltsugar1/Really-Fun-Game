using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddCoin : ChangeStatusProgram
{
    private int AddNewCoin;
    // Start is called before the first frame update
    void Start()
    {
        AddNewCoin = 4;
        Explanation = "ÄÚÀÎ È¹µæ ÇÁ·Î±×·¥";
        DeleteIsPossible = true;
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
            player.CoinUp(AddNewCoin);
        }

        Destroy(gameObject);
    }
}
