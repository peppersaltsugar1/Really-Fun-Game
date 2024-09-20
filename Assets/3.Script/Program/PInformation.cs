using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PInformation : MonoBehaviour
{
    public string ProgramName;
    public string Explanation;
    public string PowerExplanation;
    public int AddCoin = 0;
    public int HPHeal = 0;
    public int AttackPower = 0;
    public float AttackSpeed = 0;
    public float MoveSpeed = 0;
    public float BulletSpeed = 0;

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
        if (StatusManager.Instance != null)
        {
            StatusManager.Instance.AddProgramList(this);
        }
        else
        {
            Debug.LogError("StatusManager instance not found.");
        }
    }
}
