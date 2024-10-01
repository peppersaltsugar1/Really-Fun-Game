using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Program;

public class ProgramRoom : MonoBehaviour
{
    // Program Image
    Transform child_image;
    // Image Loader
    private SpriteRenderer spriteRenderer;
    // Program ID
    private int ID;


    // Start is called before the first frame update
    void Start()
    {
        // 랜덤으로 띄워줄 프로그램을 정함
        ID = Random.Range(0, Program.ProgramCount + 1);

        spriteRenderer = GetComponent<SpriteRenderer>();
        child_image = transform.Find("imsi");
        if (child_image != null)
        {
            Debug.Log("Find My Image Object");
            spriteRenderer = child_image.GetComponent<SpriteRenderer>();

            // 프로그램 이미지 띄우기
            // 임시 이미지 로더
            SetProgramImage(0);
            // 추후 이미지가 많아지면
            // SetProgramImage(ID);
            
            // 프로그램 수락 UI 띄우기


            // 만약 수락했다면 상태 매니저에 프로그램 추가
            //if (StatusManager.Instance != null)
            //{
            //    StatusManager.Instance.AddProgram(ID);
            //}
            //else
            //{
            //    Debug.LogError("StatusManager instance not found");
            //}
        }
        else
        {
            Debug.LogWarning("Not Find My Image Object");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetProgramImage(int id)
    {
        string spriteName = "Program_1~20";

        Sprite[] sprites = Resources.LoadAll<Sprite>(spriteName);

        if (sprites != null && id < sprites.Length)
        {
            Sprite newSprite = sprites[id];

            spriteRenderer.sprite = newSprite;
            Debug.Log("Sprite assigned: " + newSprite.name);
        }
        else
        {
            Debug.LogWarning("Not Find Image");
        }
    }
}
