using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardBullet : MonoBehaviour
{
    public float rotationSpeed = 1000000f;  // 회전 속도

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
