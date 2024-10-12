using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramRoom : MonoBehaviour
{
    // DownLoad Room���� ���� �� �ִ� ���α׷� ����Ʈ
    public List<PInformation> ProgramList = new List<PInformation>();
    public int ProgramID;
    public SpriteRenderer spriteRenderer;
    ProgramManager programManager;

    // Start is called before the first frame update
    void Start()
    {
        programManager = ProgramManager.Instance;

        ProgramID = Random.Range(0, ProgramList.Count);
        SetProgramImage(ProgramID);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // UI Open
            Debug.Log("UI Open is not be implemented!!");

            // Add Program
            programManager.AddProgramList(ProgramList[ProgramID]);

            // DeleteImage
            spriteRenderer.sprite = null;
        }
    }

    public void SetProgramImage(int id)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(ProgramList[ProgramID].spriteSheetName);

        if (sprites != null && ProgramList[ProgramID].spriteIndex >= 0 && ProgramList[ProgramID].spriteIndex < sprites.Length)
        {
            spriteRenderer.sprite = sprites[ProgramList[ProgramID].spriteIndex];
        }
        else
        {
            Debug.LogError("Sprite not found or invalid index.");
        }
    }

    // ���� ���α׷� ���� �����ۿ�
    public void ChangeDownloadRoomProgram()
    {
        int NewID;

        do
        {
            NewID = Random.Range(0, ProgramList.Count);

            // ProgramID�� NewID�� �ٸ���, programManager.ProgramList�� ��� ProgramName�� �ٸ� ������ �ݺ�
        } while (NewID == ProgramID ||
                 programManager.ProgramList.Exists(program => program.ProgramName == ProgramList[ProgramID].ProgramName));

        ProgramID = NewID;
        SetProgramImage(ProgramID);
    }
}
