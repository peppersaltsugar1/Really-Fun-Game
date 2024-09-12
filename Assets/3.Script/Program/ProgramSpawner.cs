using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgramSpawner : MonoBehaviour
{
    Program.program ProgramType;
    public bool[] SpawnProgramList = new bool[Program.ProgramCount];
    public GameObject[] objectToSpawn = new GameObject[Program.ProgramCount];
    public Transform spawnPoint;

    int randomIndex;

    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = transform; 
    }

    // Update is called once per frame
   void Update()
   {
       
   }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        //do{
        //    randomIndex = Random.Range(0, Program.ProgramCount - 1);
        //} while (SpawnProgramList[randomIndex]);

        randomIndex = Random.Range(0, Program.ProgramCount - 1);
        Debug.Log("Random number: " + randomIndex);

        Instantiate(objectToSpawn[randomIndex], spawnPoint.position, spawnPoint.rotation);

        Destroy(gameObject);
    }

}
