using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    public Transform playerPoint;
    public Player player;
    public float StartTime;

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
        ResetPlayTime();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ResetPlayTime()
    {
        StartTime = Time.time;
    }

    public void ReStartGame()
    {
        // SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        // �� ����� ����� �־�� ��.

        Debug.Log("�� ����� ����� �־�� ��.");
        Debug.Log("���� ��ġ�� �̵���Ű�� ����� �־�� ��.");
        Debug.Log("�� ����� ����� �־�� ��.");
    }

}
