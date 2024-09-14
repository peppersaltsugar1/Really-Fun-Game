using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSpeed : ChangeStatusProgram
{
    private float newSpeed;
    public static string StaticProgramName = "Moving Speed";
    public static string StaticExplanation = "이동속도 증가 프로그램";

    // Start is called before the first frame update
    void Start()
    {
        newSpeed = 10.0f;
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
            player.SetSpeed(newSpeed);
            Debug.Log("CAS IN ITEM");
        }

        Destroy(gameObject);
    }
}
