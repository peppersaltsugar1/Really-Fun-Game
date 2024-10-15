using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalDiskUI : MonoBehaviour
{
    UIManager uiManager;
    public GameObject telMap;
    TeleportManager telManger;
    // Start is called before the first frame update
    private void Awake()
    {
        uiManager = UIManager.Instance;
        telManger = TeleportManager.Instance;
    }

    // Update is called once per frame
 
    private void OnEnable()
    {
        uiManager.RoomUISet();
    }
    private void OnDisable()
    {
        telManger.LocalDisckTel(telMap);
    }
    public void LocalDiskTel()
    {
        uiManager.SetWindowUI();
    }
}
