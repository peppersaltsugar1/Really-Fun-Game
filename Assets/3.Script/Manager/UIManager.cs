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
        hpUI.Add("기본체력", GameObject.Find("R_Heart"));
        hpUI.Add("추가체력", GameObject.Find("RDis_Heart"));
        hpUI.Add("베터리체력", GameObject.Find("Elect_Heart"));
        hpUI.Add("쉴드체력", GameObject.Find("Shield_Heart"));
    }

    void Update()
    {

    }
    public void HpSet()
    {
       
    }
}
