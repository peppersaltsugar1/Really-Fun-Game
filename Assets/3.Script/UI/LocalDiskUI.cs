using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalDiskUI : MonoBehaviour
{
    UIManager uiManager;
    TeleportManager teleportManager;
    public int currentLocakDiskMapIndex;
    // Start is called before the first frame update
    private void Awake()
    {
        uiManager = UIManager.Instance;
        teleportManager = TeleportManager.Instance;


    }
    

    // Update is called once per frame
    void Update()
    {

    }
    private void OnEnable()
    {
        Debug.Log("È°¼ºÈ­µÊ");
        uiManager.RoomUISet();
    }
    private void OnDisable()
    {
        teleportManager.LocalDisckTel(currentLocakDiskMapIndex);
    }


}
