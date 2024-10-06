using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalDiskUI : MonoBehaviour
{
    UIManager uiManager;
    // Start is called before the first frame update
    private void Awake()
    {
        uiManager = UIManager.Instance;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnEnable()
    {
        uiManager.RoomUISet();
    }
}
