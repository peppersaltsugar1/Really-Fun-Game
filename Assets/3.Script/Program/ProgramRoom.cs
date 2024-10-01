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
        // �������� ����� ���α׷��� ����
        ID = Random.Range(0, Program.ProgramCount + 1);

        spriteRenderer = GetComponent<SpriteRenderer>();
        child_image = transform.Find("imsi");
        if (child_image != null)
        {
            Debug.Log("Find My Image Object");
            spriteRenderer = child_image.GetComponent<SpriteRenderer>();

            // ���α׷� �̹��� ����
            // �ӽ� �̹��� �δ�
            SetProgramImage(0);
            // ���� �̹����� ��������
            // SetProgramImage(ID);
            
            // ���α׷� ���� UI ����


            // ���� �����ߴٸ� ���� �Ŵ����� ���α׷� �߰�
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
