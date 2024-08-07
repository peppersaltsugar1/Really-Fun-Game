using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    public Transform playerPoint;
    public Player player;
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static GameManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        FindPlayer();
    }
    private void FindPlayer()
    {
        playerPoint = player.transform;
    }
}
