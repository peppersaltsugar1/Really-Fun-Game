using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager instance = null;
    [SerializeField]
    private Player player;
    [SerializeField]
    private Dictionary<string, GameObject> hpUI = new();
    [SerializeField]
    private Canvas canvas;

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
    public static UIManager Instance
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
        hpUI.Add("�⺻ü��", GameObject.Find("R_Heart"));
        hpUI.Add("�߰�ü��", GameObject.Find("RDis_Heart"));
        hpUI.Add("���͸�ü��", GameObject.Find("Elect_Heart"));
        hpUI.Add("����ü��", GameObject.Find("Shield_Heart"));
    }

    void Update()
    {

    }
    public void HpSet()
    {
       
    }
}
